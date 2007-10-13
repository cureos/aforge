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
    /// Motion detector based on background modeling (low precision version).
    /// </summary>
    /// 
    /// <remarks><para>The motion detector is similar to the <see cref="BackgroundModelingHighPrecisionMotionDetector">
    /// motion detector with high precision background modeling</see>. The difference between these two
    /// motion detectors is that the low precision version resizes background image making it smaller by 64
    /// times. This reduces the amount of data to proceed and improves motion detection speed a lot. For resizing
    /// each 64 pixels of source image are averaged and represent single pixel of background frame. The algorithms
    /// not only reduces the amount of data, but also removes noise.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector with motion highligh
    /// BackgroundModelingLowPrecisionMotionDetector detector =
    ///     new BackgroundModelingLowPrecisionMotionDetector( true );
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
    public class BackgroundModelingLowPrecisionMotionDetector : IMotionDetector
    {
        // source frame's dimension
        private int width;
        private int height;
        // background's frame dimension
        private int backgroundWidth;
        private int backgroundHeight;
        private int frameSize;

        // background frame of video stream
        private IntPtr backgroundFrame;
        // current frame of video sream
        private IntPtr currentFrame;
        // pixel changed in the new frame of video stream
        private int pixelsChanged;

        // the flag determines if it is required to highlight motion regions
        private bool highlightMotionRegions = false;
        private bool highlighMotionEdges = true;

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
        /// Initializes a new instance of the <see cref="BackgroundModelingLowPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        public BackgroundModelingLowPrecisionMotionDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundModelingLowPrecisionMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// 
        public BackgroundModelingLowPrecisionMotionDetector( bool highlightMotionRegions )
        {
            this.highlightMotionRegions = highlightMotionRegions;
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

                // calculate dimension of background image (64 times smaller)
                // each pixel of the background image represent 8x8 region of source iimage
                backgroundWidth = ( ( width - 1 ) / 8 ) + 1;
                backgroundHeight = ( ( height - 1 ) / 8 ) + 1;
                frameSize = backgroundWidth * backgroundHeight;

                // alocate memory for previous and current frames
                backgroundFrame = Marshal.AllocHGlobal( frameSize );
                currentFrame = Marshal.AllocHGlobal( frameSize );

                // lock source image
                imageData = image.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

                // conver the source image
                GrayscaleAndCompressImage( imageData, backgroundFrame );

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

            // convert current image
            GrayscaleAndCompressImage( imageData, currentFrame );

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
                        ( *backFrame )++;
                    }
                    else if ( diff < 0 )
                    {
                        ( *backFrame )--;
                    }
                }
            }

            backFrame = (byte*) backgroundFrame.ToPointer( );
            currFrame = (byte*) currentFrame.ToPointer( );
            pixelsChanged = 0;

            // 1 - get difference between frames
            // 2 - threshold the difference
            // 3 - calculate amount of motion pixels
            for ( int i = 0; i < frameSize; i++, backFrame++, currFrame++ )
            {
                // difference
                diff = (int) *currFrame - (int) *backFrame;
                // treshold
                *currFrame = ( ( diff >= differenceThreshold ) || ( diff <= differenceThresholdNeg ) ) ? (byte) 1 : (byte) 0;
                // amount of motion
                pixelsChanged += *currFrame;
            }
            pixelsChanged *= 64;

            // highlight motion regions
            if ( highlightMotionRegions )
            {
                byte* motion = (byte*) currentFrame.ToPointer( );
                byte* src = (byte*) imageData.Scan0.ToPointer( );
                int srcOffset = imageData.Stride - width * 3;

                if ( highlighMotionEdges )
                {
                    // highlight only motion edges
                    int backgroundWidthM1 = backgroundWidth - 1;
                    int backgroundHeightM1 = backgroundHeight - 1;

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        int i = ( y / 8 );

                        // for each pixel
                        for ( int x = 0; x < width; x++, src += 3 )
                        {
                            int j = x / 8;
                            int k = i * backgroundWidth + j;

                            // check if we need to highlight moving object
                            if ( motion[k] == 1 )
                            {
							    // check for border
                                if (
                                    ( ( x % 8 == 0 ) && ( ( j == 0 ) || ( motion[k - 1] == 0 ) ) ) ||
                                    ( ( x % 8 == 7 ) && ( ( j == backgroundWidthM1 ) || ( motion[k + 1] == 0 ) ) ) ||
                                    ( ( y % 8 == 0 ) && ( ( i == 0 ) || ( motion[k - backgroundWidth] == 0 ) ) ) ||
                                    ( ( y % 8 == 7 ) && ( ( i == backgroundHeightM1 ) || ( motion[k + backgroundWidth] == 0 ) ) )
                                    )
                                {
                                    src[2] = 255;
                                }
                            }
                        }
                        src += srcOffset;
                    }
                }
                else
                {
                    // highlight entire motion regions

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        int i = ( y / 8 );

                        // for each pixel
                        for ( int x = 0; x < width; x++, src += 3 )
                        {
                            int j = x / 8;
                            int k = i * backgroundWidth + j;

                            // check if we need to highlight moving object
                            if ( motion[k] == 1 )
                            {
                                src[2] = 255;
                            }
                        }
                        src += srcOffset;
                    }
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

            framesCounter = 0;
        }

        /// <summary>
        /// Grayscale and compress source image.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source color image.</param>
        /// <param name="destBuffer">Destination buffer for grayscale image.</param>
        /// 
        private unsafe void GrayscaleAndCompressImage( BitmapData sourceImage, IntPtr destBuffer )
        {
            int offset = sourceImage.Stride - width * 3;
            // line length measured in 8 pixels
            int len = ( ( width - 1 ) / 8 ) + 1;
            int lenM1 = len - 1;
            // remainder pixels in stride
            int rem = ( ( width - 1 ) % 8 ) + 1;
            // line intencities
            int[] lineIntencities = new int[len];

            // do the job
            byte* src = (byte*) sourceImage.Scan0.ToPointer( );
            byte* dst = (byte*) destBuffer.ToPointer( );

            for ( int y = 0; y < height; )
            {
                Array.Clear( lineIntencities, 0, len );

                // number of lines to proceed
                int n = height - y;
                if ( n > 8 )
                    n = 8;

                // calculate destination pixel values
                for ( int i = 0; i < n; i++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src += 3 )
                    {
                        // grayscale value
                        lineIntencities[(int) ( x / 8 )] += (int) ( ( 2125 * src[2] + 7154 * src[1] + 721 * src[0] ) / 10000 );
                    }
                    src += offset;
                }
                y += n;

                // get average values
                int c1 = n * 8;
                int c2 = n * rem;

                for ( int i = 0; i < lenM1; i++, dst++ )
                {
                    *dst = (byte) ( lineIntencities[i] / c1 );
                }
                *dst = (byte) ( lineIntencities[lenM1] / c2 );
                dst++;
            }
        }
    }
}
