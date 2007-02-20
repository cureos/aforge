// AForge Image Processing Library
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
    /// Resize image using nearest neighbor algorithm
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ResizeNearestNeighbor : FilterResize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image</param>
        /// <param name="newHeight">Height of new image</param>
        /// 
		public ResizeNearestNeighbor( int newWidth, int newHeight ) :
            base( newWidth, newHeight )
		{
		}

        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // get source image size
            int width   = sourceData.Width;
            int height  = sourceData.Height;

            int pixelSize = ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride - pixelSize * newWidth;
            double xFactor = (double) width / newWidth;
            double yFactor = (double) height / newHeight;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            byte* p;

            // cooridinaes of nearest point
            int ox, oy;

            // for each line
            for ( int y = 0; y < newHeight; y++ )
            {
                // Y coordinate of the nearest point
                oy = (int) ( y * yFactor );

                // for each pixel
                for ( int x = 0; x < newWidth; x++ )
                {
                    // X coordinate of the nearest point
                    ox = (int) ( x * xFactor );

                    p = src + oy * srcStride + ox * pixelSize;

                    for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                    {
                        *dst = *p;
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
