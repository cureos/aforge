
namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    public class MotionDetector
    {
        private IMotionDetector detector;
        private IMotionProcessing processor;

        public IMotionDetector MotionDetectionAlgorthm
        {
            get { return detector; }
        }
        public IMotionProcessing MotionProcessingAlgorithm
        {
            get { return processor; }
        }


        public MotionDetector( IMotionDetector detector ) : this( detector, null ) { }

        public MotionDetector( IMotionDetector detector, IMotionProcessing processor )
        {
            this.detector  = detector;
            this.processor = processor;
        }

        public double ProcessFrame( Bitmap videoFrame )
        {
            BitmapData videoData = videoFrame.LockBits(
                new Rectangle( 0, 0, videoFrame.Width, videoFrame.Height ),
                ImageLockMode.ReadWrite, videoFrame.PixelFormat );

            UnmanagedImage uImage = new UnmanagedImage( videoData );

            detector.ProcessFrame( uImage );
            if ( ( processor != null ) && ( detector.MotionFrame != null ) )
            {
                processor.ProcessFrame( uImage, detector.MotionFrame );
            }

            videoFrame.UnlockBits( videoData );

            return detector.MotionLevel;
        }

        public void Reset( )
        {
            detector.Reset( );
            if ( processor != null )
            {
                processor.Reset( );
            }
        }
    }
}
