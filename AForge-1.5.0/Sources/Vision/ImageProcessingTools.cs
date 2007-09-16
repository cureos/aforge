// AForge Vision Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//
namespace AForge.Vision
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Some image processing tools used internally.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    internal class ImageProcessingTools
    {
        // Avoid class instantiation
        private ImageProcessingTools( ) { }


        /// <summary>
        /// Grayscale source image.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source color image.</param>
        /// <param name="destBuffer">Destination buffer for grayscale image.</param>
        /// 
        public static unsafe void GrayscaleImage( BitmapData sourceImage, IntPtr destBuffer )
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            // do the job
            byte* src = (byte*) sourceImage.Scan0.ToPointer( );
            byte* dst = (byte*) destBuffer.ToPointer( );
            int srcOffset = sourceImage.Stride - width * 3;

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, src += 3, dst++ )
                {
                    // use integer arithmetic to instead of doubles to speed up calculation
                    *dst = (byte) ( ( 2125 * src[2] + 7154 * src[1] + 721 * src[0] ) / 10000 );
                }
                src += srcOffset;
            }
        }
    }
}
