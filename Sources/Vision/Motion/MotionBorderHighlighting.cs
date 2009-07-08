namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    public class MotionBorderHighlighting : IMotionProcessing
    {
        private Color highlightColor = Color.Red;

        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            int width  = videoFrame.Width;
            int height = videoFrame.Height;

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            byte fillR = highlightColor.R;
            byte fillG = highlightColor.G;
            byte fillB = highlightColor.B;

            byte* src    = (byte*) videoFrame.ImageData.ToPointer( );
            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            int srcOffset    = videoFrame.Stride  - ( width - 2 ) * 3;
            int motionOffset = motionFrame.Stride - ( width - 2 );

            src    += videoFrame.Stride + 3;
            motion += motionFrame.Stride + 1;

            int widthM1  = width - 1;
            int heightM1 = height - 1;

            // use simple edge detector
            for ( int y = 1; y < heightM1; y++ )
            {
                for ( int x = 1; x < widthM1; x++, motion++, src += 3 )
                {
                    if ( 4 * *motion - motion[-width] - motion[width] - motion[1] - motion[-1] != 0 )
                    {
                        src[RGB.R] = fillR;
                        src[RGB.G] = fillG;
                        src[RGB.B] = fillB;                    }
                }

                motion += motionOffset;
                src += srcOffset;
            }
        }

        public void Reset( )
        {
        }
    }
}
