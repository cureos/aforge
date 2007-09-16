// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Invert image.
    /// </summary>
    /// 
    /// <remarks>The filter inverts colored and grayscale images.</remarks> 
    ///
    public sealed class Invert : FilterAnyToAnyPartial
    {
        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            int startY  = rect.Top;
            int stopY   = startY + rect.Height;

            int startX  = rect.Left * pixelSize;
            int stopX   = startX + rect.Width * pixelSize;

            int offset  = imageData.Stride - ( stopX - startX );

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + rect.Left * pixelSize );

            // invert
            for ( int y = startY; y < stopY; y++ )
            {
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    // ivert each pixel
                    *ptr = (byte) ( 255 - *ptr );
                }
                ptr += offset;
            }
        }
    }
}
