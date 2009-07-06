namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    public class MotionAreaHighlighting : IMotionProcessing
    {
        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            int width  = videoFrame.Width;
            int height = videoFrame.Height;

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            byte* src = (byte*) videoFrame.ImageData.ToPointer( );
            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            int srcOffset = videoFrame.Stride - width * 3;
            int motionOffset = motionFrame.Stride - width;

            // shift to the red channel
            src += 2;

            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++, motion++, src += 3 )
                {
                    *src |= *motion;
                }
                src += srcOffset;
                motion += motionOffset;
            }
        }

        public void Reset( )
        {
        }
    }
}
