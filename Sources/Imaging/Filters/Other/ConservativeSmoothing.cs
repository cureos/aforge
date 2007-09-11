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
    /// Conservative smoothing.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ConservativeSmoothing : FilterAnyToAnyUsingCopyPartial
    {
        private int size = 3;

        /// <summary>
        /// Kernel size.
        /// </summary>
        /// 
        /// <remarks>Determines the size of pixel's square used for smoothing. Default value is 3.
        /// Minimum value is 3. Maximum value is 25.</remarks>
        /// 
        public int KernelSize
        {
            get { return size; }
            set { size = Math.Max( 3, Math.Min( 25, value | 1 ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        /// 
        public ConservativeSmoothing( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        /// 
        /// <param name="size">Kernel size.</param>
        /// 
        public ConservativeSmoothing( int size )
        {
            KernelSize = size;
        }


        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Pointer to source image data (first scan line).</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData, Rectangle rect )
        {
            int pixelSize = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int stride = destinationData.Stride;
            int offset = stride - rect.Width * pixelSize;

            // loop and array indexes
            int i, j, t;
            // kernel's radius
            int radius = size >> 1;
            // pixel value (min and max)
            byte minR, maxR, minG, maxG, minB, maxB, v;

            byte* src = (byte*) sourceData.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            byte* p;

            // allign pointers to the first pixel to process
            src += ( startY * stride + startX * pixelSize );
            dst += ( startY * stride + startX * pixelSize );

            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // Grayscale image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++, dst++ )
                    {
                        minG = 255;
                        maxG = 0;

                        // for each kernel row
                        for ( i = -radius; i <= radius; i++ )
                        {
                            t = y + i;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = -radius; j <= radius; j++ )
                            {
                                t = x + j;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( ( i != j ) && ( t < stopX ) )
                                {
                                    // find MIN and MAX values
                                    v = src[i * stride + j];

                                    if ( v < minG )
                                        minG = v;
                                    if ( v > maxG )
                                        maxG = v;
                                }
                            }
                        }
                        // set destination pixel
                        v = *src;
                        *dst = ( v > maxG ) ? maxG : ( ( v < minG ) ? minG : v );
                    }
                    src += offset;
                    dst += offset;
                }
            }
            else
            {
                // RGB image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src += 3, dst += 3 )
                    {
                        minR = minG = minB = 255;
                        maxR = maxG = maxB = 0;

                        // for each kernel row
                        for ( i = -radius; i <= radius; i++ )
                        {
                            t = y + i;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = -radius; j <= radius; j++ )
                            {
                                t = x + j;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( ( i != j ) && ( t < stopX ) )
                                {
                                    p = &src[i * stride + j * 3];

                                    // find MIN and MAX values

                                    // red
                                    v = p[RGB.R];

                                    if ( v < minR )
                                        minR = v;
                                    if ( v > maxR )
                                        maxR = v;

                                    // green
                                    v = p[RGB.G];

                                    if ( v < minG )
                                        minG = v;
                                    if ( v > maxG )
                                        maxG = v;

                                    // blue
                                    v = p[RGB.B];

                                    if ( v < minB )
                                        minB = v;
                                    if ( v > maxB )
                                        maxB = v;
                                }
                            }
                        }
                        // set destination pixel

                        // red
                        v = src[RGB.R];
                        dst[RGB.R] = ( v > maxR ) ? maxR : ( ( v < minR ) ? minR : v );
                        // green
                        v = src[RGB.G];
                        dst[RGB.G] = ( v > maxG ) ? maxG : ( ( v < minG ) ? minG : v );
                        // blue
                        v = src[RGB.B];
                        dst[RGB.B] = ( v > maxB ) ? maxB : ( ( v < minB ) ? minB : v );
                    }
                    src += offset;
                    dst += offset;
                }
            }
        }
    }
}
