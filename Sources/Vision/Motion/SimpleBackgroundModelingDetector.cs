// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing.Imaging;

    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    /// Motion detector based on simple background modeling.
    /// </summary>
    /// 
    /// <remarks><para>The class implements motion detection algorithm, which is based on
    /// difference of current video frame with modeled background frame.
    /// The <see cref="MotionFrame">difference frame</see> is thresholded and the
    /// <see cref="MotionLevel">amount of difference pixels</see> is calculated.
    /// To suppress stand-alone noisy pixels erosion morphological operator may be applied, which
    /// is controlled by <see cref="SuppressNoise"/> property.</para>
    /// 
    /// <para><note>In the case if precise motion area's borders are required (for example,
    /// for further motion post processing), then <see cref="KeepObjectsEdges"/> property
    /// may be used to restore borders after noise suppression.</note></para>
    /// 
    /// <para>As the first approximation of background frame, the first frame of video stream is taken.
    /// During further video processing the background frame is constantly updated, so it
    /// changes in the direction to decrease difference with current video frame (the background
    /// frame is moved towards current frame). See <see cref="FramesPerBackgroundUpdate"/>
    /// property, which controls the rate of background frame update.</para>
    /// 
    /// <para>Unlike <see cref="TwoFramesDifferenceDetector"/> motion detection algorithm, this algorithm
    /// allows to identify quite clearly all objects, which are not part of the background (scene) -
    /// most likely moving objects. And unlike <see cref="CustomFrameDifferenceDetector"/> motion
    /// detection algorithm, this algorithm includes background adaptation feature, which allows it
    /// to update its modeled background frame in order to take scene changes into account.</para>
    /// 
    /// <para><note>Because of the adaptation feature of the algorithm, the algorithm may adopt
    /// to background changes, what <see cref="CustomFrameDifferenceDetector"/> algorithm can not do.
    /// However, if moving object stays on the scene for a while (so algorithm adopts to it and does
    /// not treat it as a new moving object any more) and then starts to move again, the algorithm may
    /// find two moving objects - the true one, which is really moving, and the false one, which does not (the
    /// place, where the object stayed for a while).</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector(
    ///     new SimpleBackgroundModelingDetector( ),
    ///     new MotionAreaHighlighting( ) );
    /// 
    /// // continuously feed video frames to motion detector
    /// while ( ... )
    /// {
    ///     // process new video frame and check motion level
    ///     if ( detector.ProcessFrame( videoFrame ) > 0.02 )
    ///     {
    ///         // ring alarm or do somethng else
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// 
    public class SimpleBackgroundModelingDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // previous frame of video stream
        private UnmanagedImage backgroundFrame;
        // current frame of video sream
        private UnmanagedImage motionFrame;
        // temporary buffer used for suppressing noise
        private UnmanagedImage tempFrame;
        // number of pixels changed in the new frame of video stream
        private int pixelsChanged;

        // suppress noise
        private bool suppressNoise   = true;
        private bool keepObjectEdges = false;

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

        private int framesPerBackgroundUpdate = 2;
        private int framesCounter = 0;

        // grayscale filter
        private GrayscaleBT709 grayFilter = new GrayscaleBT709( );
        // binary erosion filter
        private BinaryErosion3x3 erosionFilter = new BinaryErosion3x3( );
        // binary dilatation filter
        private BinaryDilatation3x3 dilatationFilter = new BinaryDilatation3x3( );

        /// <summary>
        /// Difference threshold value, [1, 255].
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the amount off difference between pixels, which is treated
        /// as motion pixel.</para>
        /// 
        /// <para>Default value is set to <b>15</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set
            {
                lock ( this )
                {
                    differenceThreshold = Math.Max( 1, Math.Min( 255, value ) );
                    differenceThresholdNeg = -differenceThreshold;
                }
            }
        }

        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Amount of changes in the last processed frame. For example, if value of
        /// this property equals to 0.1, then it means that last processed frame has 10% difference
        /// with modeled background frame.</para>
        /// </remarks>
        /// 
        public float MotionLevel
        {
            get
            {
                lock ( this )
                {
                    return (float) pixelsChanged / ( width * height );
                }
            }
        }

        /// <summary>
        /// Motion frame containing detected areas of motion.
        /// </summary>
        /// 
        /// <remarks><para>Motion frame is a grayscale image, which shows areas of detected motion.
        /// All black pixels in the motion frame correspond to areas, where no motion is
        /// detected. But white pixels correspond to areas, where motion is detected.</para>
        /// 
        /// <para><note>The property is set to <see langword="null"/> after processing of the first
        /// video frame by the algorithm.</note></para>
        /// </remarks>
        ///
        public UnmanagedImage MotionFrame
        {
            get
            {
                lock ( this )
                {
                    return motionFrame;
                }
            }
        }

        /// <summary>
        /// Suppress noise in video frames or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be
        /// done to suppress standalone noisy pixels by applying 3x3 erosion image processing
        /// filter. See <see cref="KeepObjectsEdges"/> property, if it is required to restore
        /// edges of objects, which are not noise.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// 
        /// <para><note>Turning the value on leads to more processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool SuppressNoise
        {
            get { return suppressNoise; }
            set
            {
                lock ( this )
                {
                    suppressNoise = value;

                    // allocate temporary frame if required
                    if ( ( suppressNoise ) && ( tempFrame == null ) && ( motionFrame != null ) )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // check if temporary frame is not required
                    if ( ( !suppressNoise ) && ( tempFrame != null ) )
                    {
                        tempFrame.Dispose( );
                        tempFrame = null;
                    }
                }
            }
        }

        /// <summary>
        /// Restore objects edges after noise suppression or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be done
        /// to restore objects' edges after noise suppression by applying 3x3 dilatation
        /// image processing filter.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// 
        /// <para><note>Turning the value on leads to more processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool KeepObjectsEdges
        {
            get { return keepObjectEdges; }
            set
            {
                lock ( this )
                {
                    keepObjectEdges = value;
                }
            }
        }

        /// <summary>
        /// Frames per background update, [1, 50].
        /// </summary>
        /// 
        /// <remarks><para>The value controls the speed of modeled background adaptation to
        /// scene changes. After each specified amount of frames the background frame is updated
        /// in the direction to decrease difference with current processing frame.</para>
        /// 
        /// <para>Default value is set to <b>2</b>.</para>
        /// </remarks>
        /// 
        public int FramesPerBackgroundUpdate
        {
            get { return framesPerBackgroundUpdate; }
            set { framesPerBackgroundUpdate = Math.Max( 1, Math.Min( 50, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        public SimpleBackgroundModelingDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// 
        public SimpleBackgroundModelingDetector( bool suppressNoise )
        {
            this.suppressNoise = suppressNoise;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBackgroundModelingDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// <param name="keepObjectEdges">Restore objects edges after noise suppression or not (see <see cref="KeepObjectsEdges"/> property).</param>
        /// 
        public SimpleBackgroundModelingDetector( bool suppressNoise, bool keepObjectEdges )
        {
            this.suppressNoise   = suppressNoise;
            this.keepObjectEdges = keepObjectEdges;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <remarks><para>Processes new frame from video source and detects motion in it.</para>
        /// 
        /// <para>Check <see cref="MotionLevel"/> property to get information about amount of motion
        /// (changes) in the processed frame.</para>
        /// </remarks>
        ///
        public unsafe void ProcessFrame( UnmanagedImage videoFrame )
        {
            lock ( this )
            {
                // check background frame
                if ( backgroundFrame == null )
                {
                    // save image dimension
                    width = videoFrame.Width;
                    height = videoFrame.Height;

                    // alocate memory for previous and current frames
                    backgroundFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    motionFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );

                    frameSize = motionFrame.Stride * height;

                    // temporary buffer
                    if ( suppressNoise )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // convert source frame to grayscale
                    grayFilter.Apply( videoFrame, backgroundFrame );

                    return;
                }

                // check image dimension
                if ( ( videoFrame.Width != width ) || ( videoFrame.Height != height ) )
                    return;

                // convert current image to grayscale
                grayFilter.Apply( videoFrame, motionFrame );

                // pointers to background and current frames
                byte* backFrame;
                byte* currFrame;
                int diff;

                // update background frame
                if ( ++framesCounter == framesPerBackgroundUpdate )
                {
                    framesCounter = 0;

                    backFrame = (byte*) backgroundFrame.ImageData.ToPointer( );
                    currFrame = (byte*) motionFrame.ImageData.ToPointer( );

                    for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                    {
                        diff = *currFrame - *backFrame;
                        if ( diff > 0 )
                        {
                            ( *backFrame )++;
                        }
                        else if ( diff < 0 )
                        {
                            ( *backFrame )--;
                        }
                    }
                }

                backFrame = (byte*) backgroundFrame.ImageData.ToPointer( );
                currFrame = (byte*) motionFrame.ImageData.ToPointer( );

                // 1 - get difference between frames
                // 2 - threshold the difference
                for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                {
                    // difference
                    diff = (int) *currFrame - (int) *backFrame;
                    // treshold
                    *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 255 : (byte) 0;
                }

                if ( suppressNoise )
                {
                    // suppress noise and calculate motion amount
                    AForge.SystemTools.CopyUnmanagedMemory( tempFrame.ImageData, motionFrame.ImageData, frameSize );
                    erosionFilter.Apply( tempFrame, motionFrame );

                    if ( keepObjectEdges )
                    {
                        AForge.SystemTools.CopyUnmanagedMemory( tempFrame.ImageData, motionFrame.ImageData, frameSize );
                        dilatationFilter.Apply( tempFrame, motionFrame );
                    }
                }

                // calculate amount of motion pixels
                pixelsChanged = 0;
                byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

                for ( int i = 0; i < frameSize; i++, motion++ )
                {
                    pixelsChanged += ( *motion & 1 );
                }
            }
        }

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>Resets internal state and variables of motion detection algorithm.
        /// Usually this is required to be done before processing new video source, but
        /// may be also done at any time to restart motion detection algorithm.</para>
        /// </remarks>
        /// 
        public void Reset( )
        {
            lock ( this )
            {
                if ( backgroundFrame != null )
                {
                    backgroundFrame.Dispose( );
                    backgroundFrame = null;
                }

                if ( motionFrame != null )
                {
                    motionFrame.Dispose( );
                    motionFrame = null;
                }

                if ( tempFrame != null )
                {
                    tempFrame.Dispose( );
                    tempFrame = null;
                }

                framesCounter = 0;
            }
        }
    }
}
