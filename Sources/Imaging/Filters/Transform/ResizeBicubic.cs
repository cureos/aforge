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
    /// Resize image using bicubic interpolation
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ResizeBicubic : FilterResize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeBicubic"/> class
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image</param>
        /// <param name="newHeight">Height of new image</param>
        /// 
		public ResizeBicubic( int newWidth, int newHeight ) :
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

            // coordinates of source points and cooefficiens
            double  ox, oy, dx, dy, k1, k2;
            int     ox1, oy1, ox2, oy2;
            // destination pixel values
            double r, g, b;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointer
            byte* p;

            // check pixel format
            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale
                for ( int y = 0; y < newHeight; y++ )
                {
                    // Y coordinates
                    oy  = (double) y * yFactor - 0.5;
                    oy1 = (int) oy;
                    dy  = oy - (double) oy1;

                    for ( int x = 0; x < newWidth; x++, dst++ )
                    {
                        // X coordinates
                        ox  = (double) x * xFactor - 0.5f;
                        ox1 = (int) ox;
                        dx  = ox - (double) ox1;

                        // initial pixel value
                        g = 0;

                        for ( int n = -1; n < 3; n++ )
                        {
                            // get Y cooefficient
                            k1 = Interpolation.BiCubicKernel( dy - (double) n );

                            oy2 = oy1 + n;
                            if ( oy2 < 0 )
                                oy2 = 0;
                            if ( oy2 > ymax )
                                oy2 = ymax;

                            for ( int m = -1; m < 3; m++ )
                            {
                                // get X cooefficient
                                k2 = k1 * Interpolation.BiCubicKernel( (double) m - dx );

                                ox2 = ox1 + m;
                                if ( ox2 < 0 )
                                    ox2 = 0;
                                if ( ox2 > xmax )
                                    ox2 = xmax;

                                g += k2 * src[oy2 * srcStride + ox2];
                            }
                        }
                        *dst = (byte) g;
                    }
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB
                for ( int y = 0; y < newHeight; y++ )
                {
                    // Y coordinates
                    oy  = (double) y * yFactor - 0.5f;
                    oy1 = (int) oy;
                    dy  = oy - (double) oy1;

                    for ( int x = 0; x < newWidth; x++, dst += 3 )
                    {
                        // X coordinates
                        ox  = (double) x * xFactor - 0.5f;
                        ox1 = (int) ox;
                        dx  = ox - (double) ox1;

                        // initial pixel value
                        r = g = b = 0;

                        for ( int n = -1; n < 3; n++ )
                        {
                            // get Y cooefficient
                            k1 = Interpolation.BiCubicKernel( dy - (double) n );

                            oy2 = oy1 + n;
                            if ( oy2 < 0 )
                                oy2 = 0;
                            if ( oy2 > ymax )
                                oy2 = ymax;

                            for ( int m = -1; m < 3; m++ )
                            {
                                // get X cooefficient
                                k2 = k1 * Interpolation.BiCubicKernel( (double) m - dx );

                                ox2 = ox1 + m;
                                if ( ox2 < 0 )
                                    ox2 = 0;
                                if ( ox2 > xmax )
                                    ox2 = xmax;

                                // get pixel of original image
                                p = src + oy2 * srcStride + ox2 * 3;

                                r += k2 * p[RGB.R];
                                g += k2 * p[RGB.G];
                                b += k2 * p[RGB.B];
                            }
                        }

                        dst[RGB.R] = (byte) r;
                        dst[RGB.G] = (byte) g;
                        dst[RGB.B] = (byte) b;
                    }
                    dst += dstOffset;
                }
            }
        }
    }
}
