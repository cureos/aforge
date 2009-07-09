namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    public class BlobCountingObjectsProcessing : IMotionProcessing
    {
        private Color highlightColor = Color.Red;

        private BlobCounter blobCounter = new BlobCounter( );

        public BlobCountingObjectsProcessing( )
        {
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 10;
            blobCounter.MinWidth = 10;
        }

        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            int width  = videoFrame.Width;
            int height = videoFrame.Height;

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            blobCounter.ProcessImage( motionFrame );

//            if ( highlightMotionRegions )
            {
                // highlight each moving object
                Rectangle[] rects = blobCounter.GetObjectsRectangles( );

                foreach ( Rectangle rect in rects )
                {
                    Drawing.Rectangle( videoFrame, rect, highlightColor );
                }
            }
        }

        public void Reset( )
        {
        }
    }
}
