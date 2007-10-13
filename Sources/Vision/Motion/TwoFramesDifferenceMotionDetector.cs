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
    /// Motion detector based on two frames difference.
    /// </summary>
    /// 
    /// <remarks><para>The class implements the simplest motion detector, which is based on
    /// two continues frames difference. The difference frame is threshold and the amount of
    /// difference pixels is calculated. To suppress stand-alone noisy pixels erosion
    /// morphological operator may be applied.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector with noise suppresion
    /// IMotionDetector detector = new TwoFramesDifferenceMotionDetector( false, true );
    /// // feed first image to the detector
    /// detector.ProcessFrame( image );
    /// // ...
    /// // feed next image
    /// detector.ProcessFrame( nextImage );
    /// // check amount of motion
    /// if ( detector.MotionLevel &gt; 0.005 )
    /// {
    ///     // motion amount is greater then 0.5% - fire alarm
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class TwoFramesDifferenceMotionDetector : IMotionDetector
    {
        // frame's dimension
        private int width;
        private int height;
        private int frameSize;

        // previous frame of video stream
        private IntPtr previousFrame;
        // current frame of video sream
        private IntPtr currentFrame;
        // temporary buffer used for suppressing noise
        private IntPtr tempFrame;
        // pixel changed in the new frame of video stream
        private int pixelsChanged;

        // the flag determines if it is required to highlight motion regions
        private bool highlightMotionRegions = false;
        // suppress noise
        private bool suppressNoise = false;

        // threshold values
        private int differenceThreshold = 15;
        private int differenceThresholdNeg = -15;

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
                if ( ( suppressNoise ) && ( tempFrame == IntPtr.Zero ) && ( previousFrame != IntPtr.Zero ) )
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
        /// Initializes a new instance of the <see cref="TwoFramesDifferenceMotionDetector"/> class.
        /// </summary>
        /// 
        public TwoFramesDifferenceMotionDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoFramesDifferenceMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// 
        public TwoFramesDifferenceMotionDetector( bool highlightMotionRegions )
        {
            this.highlightMotionRegions = highlightMotionRegions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoFramesDifferenceMotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="highlightMotionRegions">Highlight motion regions or not.</param>
        /// <param name="suppressNoise">Suppress noise in video frames or not.</param>
        /// 
        public TwoFramesDifferenceMotionDetector( bool highlightMotionRegions, bool suppressNoise )
        {
            this.highlightMotionRegions = highlightMotionRegions;
            this.suppressNoise = suppressNoise;
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
            if ( previousFrame == IntPtr.Zero )
            {
                // save image dimension
                width = image.Width;
                height = image.Height;
                frameSize = width * height;

                // alocate memory for previous and current frames
                previousFrame = Marshal.AllocHGlobal( frameSize );
                currentFrame  = Marshal.AllocHGlobal( frameSize );
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
                ImageProcessingTools.GrayscaleImage( imageData, previousFrame );

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

            // pointers to previous and current frames
            byte* prevFrame = (byte*) previousFrame.ToPointer( );
            byte* currFrame = (byte*) currentFrame.ToPointer( );
            // difference value
            int diff;

            // 1 - get difference between frames
            // 2 - threshold the difference
            // 3 - copy current frame to previous frame
            for ( int i = 0; i < frameSize; i++, prevFrame++, currFrame++ )
            {
                // difference
                diff = (int) *currFrame - (int) *prevFrame;
                // copy current frame to previous
                *prevFrame = *currFrame;
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
                byte* src = (byte*) imageData.Scan0.ToPointer( );
                byte* motion = (byte*) currentFrame.ToPointer( );
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
            if ( previousFrame != IntPtr.Zero )
            {
                Marshal.FreeHGlobal( previousFrame );
                previousFrame = IntPtr.Zero;
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
        }
    }
}
