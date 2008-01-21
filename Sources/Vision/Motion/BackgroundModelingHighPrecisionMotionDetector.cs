// AForge Vision Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//
namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Motion detector based on background modeling (high precision version).
    /// </summary>
    /// 
    /// <remarks><para>The motion detector is based on difference of current video frame
    /// (specified frame) with background frame, which is modeled internally. As the
    /// first approximation of background frame, the first frame of video stream is used.
    /// During further video processing the background frame is changing in the direction
    /// to decrease difference with current video frame (the background frame is moving
    /// towards current frame).</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector with noise suppresion and motion highligh
    /// BackgroundModelingHighPrecisionMotionDetector detector =
    ///     new BackgroundModelingHighPrecisionMotionDetector( true, true );
    /// // highligh motion edges only
    /// detector.HighlightMotionEdges = true;
    /// // feed first image to the detector
    /// detector.ProcessFrame( image );
    /// // ...
    /// // feed next image
    /// detector.ProcessFrame( nextImage );
    /// // check amount motion
    /// if ( detector.MotionLevel > 0.005 )
    /// {
    ///     // motion amount is greater then 0.5% - fire alarm
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class BackgroundModelingHighPrecisionMotionDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // background frame of video stream
        private IntPtr backgroundFrame;
        // current frame of video sream
        private IntPtr currentFrame;
        // temporary buffer used for suppressing noise
        private IntPtr tempFrame;
        // pixel changed in the new frame of video stream
        private int pixelsChanged;

        // the flag determines if it is required to highlight motion regions
        private bool highlightMotionRegions = false;
        private bool highlighMotionEdges = true;
        // suppress noise
        private bool suppressNoise = false;
        private bool keepObjectEdges = false;

        // threshold values
        private int differenceThreshold = 15;
        private int differenceThresholdNeg = -15;

        private int framesPerBackgroundUpdate = 2;
        private int framesCounter = 0;

        /// <summary>
        /// Highlight motion regions or not.
        /// </summary>
        /// 
        /// <remarks><para>Specifies if regions, where motion has occured, should be highlighted.</para>
        /// <para>Turning the value on leads to more processing time of video frame. Default values
        /// is <b>false</b>.</para>
        /// </remarks>
        /// 
        public bool HighlightMotionRegions
        {
            get { return highlightMotionRegions; }
            set { highlightMotionRegions = value; }
        }

        /// <summary>
        /// Highligh motion edges or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if only edges of motion regions should be highlighted.
        /// The value does not have any effect until <see cref="HighlightMotionRegions">highlight
        /// motion regions</see> is requested.</para>
        /// <para>Turning the value on leads to more processing time of video frame. Default values
        /// is <b>true</b>.</para>
        /// </remarks>
        /// 
        public bool HighlightMotionEdges
        {
            get { return highlighMotionEdges; }
            set { highlighMotionEdges = value; }
        }

        /// <summary>
        /// Suppress noise in video frames or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be
        /// done to suppress standalone noisy pixels.</para>
        /// <para>Turning the value on leads to more processing time of video frame. Default values
        /// is <b>false</b>.</para>
        /// </remarks>
        /// 
        public bool SuppressNoise
        {
            get { return suppressNoise; }
            set
            {
                suppressNoise = value;

                // allocate temporary frame if required
                if ( ( suppressNoise ) && ( tempFrame == IntPtr.Zero ) && ( backgroundFrame != IntPtr.Zero ) )
                {
                    tempFrame = Marshal.AllocHGlobal( width * height );
                }

                // frame temporary frame if required
                if ( ( !suppressNoise ) && ( tempFrame != IntPtr.Zero ) )
                {
                    Marshal.FreeHGlobal( tempFrame );
                }
            }
        }

        /// <summary>
        /// Restore objects edges after noise suppression or not.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies if additional filtering should be done
        /// to restore objects edges after noise suppression.</para>
        /// <para>Turning the value on leads to more processing time of video frame. Default values
        /// is <b>false</b>.</para>
        /// </remarks>
        /// 
        public bool KeepObjectsEdges
        {
            get { return keepObjectEdges; }
            set { keepObjectEdges = true; }
        }

        /// <summary>
        /// Difference threshold value.
        /// </summary>
        /// 
        /// <remarks>The value specifies the amount off difference between pixels, which is treated
        /// as motion pixel. Default value is <b>15</b>.</remarks>
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
        /// Frames per background update.
        /// </summary>
        /// 
        /// <remarks><para>The value controls the speed of background adaptation to scene changes. After
        /// each specified amount of frames the background frame is updated in the direction
        /// to decrease difference with current processing frame.</para>
        /// <para>The default value is 2. The value may be set in the range of [1, 50].</para>
        /// </remarks>
        /// 
        public int FramesPerBackgroundUpdate
        {
            get { return framesPerBackgroundUpdate; }
            set { framesPerBackgroundUpdate = Math.Max( 1, Math.Min( 50, value ) ); }
        }

        /// <summary>
        /// Motion level value.
        /// </summary>
        /// 
        /// <remarks>Amount of changes in the last processed frame in percents.</remarks>
        /// 
        public double MotionLevel
        {
            get { return (double) pixelsChanged / ( width * height ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundModelingHighPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        public BackgroundModelingHighPrecisionMotionDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundModelingHighPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// 
        public BackgroundModelingHighPrecisionMotionDetector( bool highlightMotionRegions )
        {
            this.highlightMotionRegions = highlightMotionRegions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundModelingHighPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// <param name="suppressNoise">Suppress noise in video frames or not.</param>
        /// 
        public BackgroundModelingHighPrecisionMotionDetector( bool highlightMotionRegions, bool suppressNoise )
        {
            this.highlightMotionRegions = highlightMotionRegions;
            this.suppressNoise = suppressNoise;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundModelingHighPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// <param name="suppressNoise">Suppress noise in video frames or not.</param>
        /// <param name="keepObjectEdges">Restore objects edges after noise suppression or not.</param>
        /// 
        public BackgroundModelingHighPrecisionMotionDetector( bool highlightMotionRegions, bool suppressNoise, bool keepObjectEdges )
        {
            this.highlightMotionRegions = highlightMotionRegions;
            this.suppressNoise = suppressNoise;
            this.keepObjectEdges = keepObjectEdges;
        }

        /// <summary>
        /// Process new frame.
        /// </summary>
        /// 
        /// <remarks>Process new frame of video source and detect motion.</remarks>
        /// 
        public unsafe void ProcessFrame( Bitmap image )
        {
            BitmapData imageData = null;

            // check previous frame
            if ( backgroundFrame == IntPtr.Zero )
            {
                // save image dimension
                width = image.Width;
                height = image.Height;
                frameSize = width * height;

                // alocate memory for background and current frames
                backgroundFrame = Marshal.AllocHGlobal( frameSize );
                currentFrame = Marshal.AllocHGlobal( frameSize );
                // temporary buffer
                if ( suppressNoise )
                {
                    tempFrame = Marshal.AllocHGlobal( frameSize );
                }

                // lock source image
                imageData = image.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

                // convert source frame to grayscale
                ImageProcessingTools.GrayscaleImage( imageData, backgroundFrame );

                // unlock source image
                image.UnlockBits( imageData );

                return;
            }

            // check image dimension
            if ( ( image.Width != width ) || ( image.Height != height ) )
                return;

            // lock source image
            imageData = image.LockBits(
                new Rectangle( 0, 0, width, height ),
                ( highlightMotionRegions ) ? ImageLockMode.ReadWrite : ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb );

            // convert current image to grayscale
            ImageProcessingTools.GrayscaleImage( imageData, currentFrame );

            // pointers to background and current frames
            byte* backFrame;
            byte* currFrame;
            int diff;

            // update background frame
            if ( ++framesCounter == framesPerBackgroundUpdate )
            {
                framesCounter = 0;

                backFrame = (byte*) backgroundFrame.ToPointer( );
                currFrame = (byte*) currentFrame.ToPointer( );

                for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
                {
                    diff = *currFrame - *backFrame;
                    if ( diff > 0 )
                    {
                        (*backFrame)++;
                    }
                    else if ( diff < 0 )
                    {
                        (*backFrame)--;
                    }
                }
            }

            backFrame = (byte*) backgroundFrame.ToPointer( );
            currFrame = (byte*) currentFrame.ToPointer( );

            // 1 - get difference between frames
            // 2 - threshold the difference
            for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
            {
                // difference
                diff = (int) *currFrame - (int) *backFrame;
                // treshold
                *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 255 : (byte) 0;
            }

            // calculate amount of motion pixels
            pixelsChanged = 0;

            if ( suppressNoise )
            {
                // suppress noise and calculate motion amount
                AForge.Win32.memcpy( tempFrame, currentFrame, frameSize );

                byte* motion = (byte*) currentFrame.ToPointer( ) + width + 1;
                byte* temp = (byte*) tempFrame.ToPointer( ) + width + 1;

                int widthM1 = width - 1;
                int heightM1 = height - 1;

                // erosion is used to suppress noise
                for ( int y = 1; y < heightM1; y++ )
                {
                    for ( int x = 1; x < widthM1; x++, motion++, temp++ )
                    {
                        // check if it is motion pixel
                        if ( *motion != 0 )
                        {
                            *motion = (byte) ( temp[-width - 1] & temp[-width] & temp[-width + 1] &
                                temp[width - 1] & temp[width] & temp[width + 1] &
                                temp[1] & temp[-1] );

                            pixelsChanged += ( *motion & 1 );
                        }
                    }
                    motion += 2;
                    temp += 2;
                }

                // check if we need to restore objects edges
                if ( keepObjectEdges )
                {
                    pixelsChanged = 0;

                    AForge.Win32.memcpy( tempFrame, currentFrame, frameSize );

                    motion = (byte*) currentFrame.ToPointer( ) + width + 1;
                    temp = (byte*) tempFrame.ToPointer( ) + width + 1;

                    // dilatation is used to restore objects edges
                    for ( int y = 1; y < heightM1; y++ )
                    {
                        for ( int x = 1; x < widthM1; x++, motion++, temp++ )
                        {
                            *motion = (byte) ( temp[-width - 1] | temp[-width] | temp[-width + 1] |
                                temp[width - 1] | temp[width] | temp[width + 1] |
                                temp[1] | temp[-1] );

                            pixelsChanged += ( *motion & 1 );
                        }
                        motion += 2;
                        temp += 2;
                    }
                }
            }
            else
            {
                // calculate motion without suppressing noise
                byte* motion = (byte*) currentFrame.ToPointer( );

                for ( int i = 0; i < frameSize; i++, motion++ )
                {
                    pixelsChanged += ( *motion & 1 );
                }
            }

            // highlight motion regions
            if ( highlightMotionRegions )
            {
                byte* motion = (byte*) currentFrame.ToPointer( );

                // is it required to highligh edges only
                if ( highlighMotionEdges )
                {
                    AForge.Win32.memcpy( tempFrame, currentFrame, frameSize );

                    motion += width + 1;
                    byte* temp = (byte*) tempFrame.ToPointer( ) + width + 1;

                    int widthM1 = width - 1;
                    int heightM1 = height - 1;

                    // apply difference edge detector
                    for ( int y = 1; y < heightM1; y++ )
                    {
                        for ( int x = 1; x < widthM1; x++, motion++, temp++ )
                        {
                            *motion = (byte) ( 4 * *temp - temp[-width] - temp[width] - temp[1] - temp[-1] );
                        }
                        motion += 2;
                        temp += 2;
                    }

                    // restore motion pointer
                    motion = (byte*) currentFrame.ToPointer( );
                }

                byte* src = (byte*) imageData.Scan0.ToPointer( );
                int srcOffset = imageData.Stride - width * 3;

                // shift to the red channel
                src += 2;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, motion++, src += 3 )
                    {
                        *src |= *motion;
                    }
                    src += srcOffset;
                }
            }

            // unlock source image
            image.UnlockBits( imageData );
        }

        /// <summary>
        /// Reset detector to initial state.
        /// </summary>
        /// 
        /// <remarks>Resets internal state and variables of motion detection algorithm.</remarks>
        /// 
        public void Reset( )
        {
            if ( backgroundFrame != IntPtr.Zero )
            {
                Marshal.FreeHGlobal( backgroundFrame );
                backgroundFrame = IntPtr.Zero;
            }

            if ( currentFrame != IntPtr.Zero )
            {
                Marshal.FreeHGlobal( currentFrame );
                currentFrame = IntPtr.Zero;
            }

            if ( tempFrame != IntPtr.Zero )
            {
                Marshal.FreeHGlobal( tempFrame );
                tempFrame = IntPtr.Zero;
            }

            framesCounter = 0;
        }
    }
}
