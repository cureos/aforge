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

        // threshold values
        private int differenceThreshold    =  15;
        private int differenceThresholdNeg = -15;

        // grayscale filter
        private GrayscaleBT709 grayFilter = new GrayscaleBT709( );
        // binary erosion filter
        private BinaryErosion3x3 erosionFilter = new BinaryErosion3x3( );

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
        /// <remarks>Amount of changes in the last processed frame.</remarks>
        /// 
        public double MotionLevel
        {
            get { return (double) pixelsChanged / ( width * height ); }
        }

        public UnmanagedImage MotionFrame
        {
            get { return motionFrame; }
        }

        public unsafe void ProcessFrame( UnmanagedImage videoFrame )
        {
            // check previous frame
            if ( backgroundFrame == null )
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
            }

            // calculate amount of motion pixels
            pixelsChanged = 0;
            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            for ( int i = 0; i < frameSize; i++, motion++ )
            {
                pixelsChanged += ( *motion & 1 );
            }
        }

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
    }
}
