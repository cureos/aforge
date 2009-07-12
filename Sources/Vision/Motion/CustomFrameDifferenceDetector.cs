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

    public class CustomFrameDifferenceDetector : IMotionDetector
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
        private bool suppressNoise = true;
        private bool keepObjectEdges = true;

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

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
                differenceThreshold = Math.Max( 1, Math.Min( 255, value ) );
                differenceThresholdNeg = -differenceThreshold;
            }
        }

        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Amount of changes in the last processed frame. For example, if value of
        /// this property equals to 0.1, then it means that last processed frame has 10% difference
        /// with defined background frame.</para>
        /// </remarks>
        /// 
        public double MotionLevel
        {
            get { return (double) pixelsChanged / ( width * height ); }
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
        /// video frame by the algorithm in the case if custom background frame was not set manually.</note></para>
        /// </remarks>
        ///
        public UnmanagedImage MotionFrame
        {
            get { return motionFrame; }
        }

        /// <summary>
        /// Suppress noise in video frames or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be
        /// done to suppress standalone noisy pixels by applying 3x3 erosion image processing
        /// filter.</para>
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
                lock ( erosionFilter )
                {
                    suppressNoise = value;

                    // allocate temporary frame if required
                    if ( ( suppressNoise ) && ( tempFrame == null ) && ( motionFrame != null ) )
                    {
                        tempFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                    }

                    // frame temporary frame if required
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
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// 
        /// <para><note>Turning the value on leads to more processing time of video frame.</note></para>
        /// </remarks>
        /// 
        public bool KeepObjectsEdges
        {
            get { return keepObjectEdges; }
            set { keepObjectEdges = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFrameDifferenceDetector"/> class.
        /// </summary>
        public CustomFrameDifferenceDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFrameDifferenceDetector"/> class.
        /// </summary>
        /// 
        /// <param name="suppressNoise">Suppress noise in video frames or not (see <see cref="SuppressNoise"/> property).</param>
        /// 
        public CustomFrameDifferenceDetector( bool suppressNoise )
        {
            this.suppressNoise = suppressNoise;
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
            // check background frame
            if ( motionFrame == null )
            {
                // save image dimension
                width  = videoFrame.Width;
                height = videoFrame.Height;

                // alocate memory for previous and current frames
                backgroundFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                motionFrame     = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );

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

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>Resets internal state and variables of motion detection algorithm.
        /// Usually this is required to do before processing new video source, but
        /// may be also done at any time to restart motion detection algorithm.</para>
        /// </remarks>
        /// 
        public void Reset( )
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
        }

        public void SetBackgroundFrame( UnmanagedImage backgroundFrame )
        {
            lock ( this )
            {
                // reset motion detection algorithm
                Reset( );

                // save image dimension
                width  = backgroundFrame.Width;
                height = backgroundFrame.Height;

                // alocate memory for previous and current frames
                this.backgroundFrame = UnmanagedImage.Create( width, height, PixelFormat.Format8bppIndexed );
                backgroundFrame.Copy( this.backgroundFrame );

                frameSize = this.backgroundFrame.Stride * height;
            }
        }
    }
}
