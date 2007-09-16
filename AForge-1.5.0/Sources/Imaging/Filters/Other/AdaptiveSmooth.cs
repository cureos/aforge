// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//
// Found description in
// "An Edge Detection Technique Using the Facet
// Model and Parameterized Relaxation Labeling"
// by Ioannis Matalas, Student Member, IEEE, Ralph Benjamin, and Richard Kitney
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Adaptive Smoothing - noise removal with edges preserving.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class AdaptiveSmooth : FilterAnyToAnyUsingCopyPartial
    {
        private double factor = 3.0;

        /// <summary>
        /// Factor value.
        /// </summary>
        /// 
        /// <remarks></remarks>
        /// 
        public double Factor
        {
            get { return factor; }
            set { factor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveSmooth"/> class.
        /// </summary>
        /// 
        public AdaptiveSmooth( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveSmooth"/> class.
        /// </summary>
        /// 
        /// <param name="factor">Factor value.</param>
        /// 
        public AdaptiveSmooth( double factor )
        {
            this.factor = factor;
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
            int pixelSize   = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int pixelSize2  = pixelSize * 2;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int startXP2    = startX + 2;
            int startYP2    = startY + 2;
            int stopXM2     = stopX - 2;
            int stopYM2     = stopY - 2;

            int stride = destinationData.Stride;
            int offset = stride - rect.Width * pixelSize;

            // gradient and weights
            double gx, gy, weight, weightTotal, total;
            // precalculated factor value
            double f = -8 * factor * factor;

            // do the job
            byte* src = (byte*) sourceData.ToPointer( ) + stride * 2;
            byte* dst = (byte*) destinationData.Scan0.ToPointer( ) + stride * 2;

            // allign pointers to the first pixel to process
            src += ( startY * stride + startX * pixelSize );
            dst += ( startY * stride + startX * pixelSize );

            for ( int y = startYP2; y < stopYM2; y++ )
            {
                src += 2 * pixelSize;
                dst += 2 * pixelSize;

                for ( int x = startXP2; x < stopXM2; x++ )
                {
                    for ( int i = 0; i < pixelSize; i++, src++, dst++ )
                    {
                        weightTotal = 0;
                        total = 0;

                        // original formulas for weight calculation:
                        // w(x, y) = exp( -1 * (Gx^2 + Gy^2) / (2 * factor^2) )
                        // Gx(x, y) = (I(x + 1, y) - I(x - 1, y)) / 2
                        // Gy(x, y) = (I(x, y + 1) - I(x, y - 1)) / 2
                        //
                        // here is a little bit optimized version

                        // x - 1, y - 1
                        gx = src[-stride] - src[-pixelSize2 - stride];
                        gy = src[-pixelSize] - src[-pixelSize - 2 * stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize - stride];
                        weightTotal += weight;

                        // x, y - 1
                        gx = src[pixelSize - stride] - src[-pixelSize - stride];
                        gy = *src - src[-2 * stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-stride];
                        weightTotal += weight;

                        // x + 1, y - 1
                        gx = src[pixelSize2 - stride] - src[-stride];
                        gy = src[pixelSize] - src[pixelSize - 2 * stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize - stride];
                        weightTotal += weight;

                        // x - 1, y
                        gx = *src - src[-pixelSize2];
                        gy = src[-pixelSize + stride] - src[-pixelSize - stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize];
                        weightTotal += weight;

                        // x, y
                        gx = src[pixelSize] - src[-pixelSize];
                        gy = src[stride] - src[-stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * ( *src );
                        weightTotal += weight;

                        // x + 1, y
                        gx = src[pixelSize2] - *src;
                        gy = src[pixelSize + stride] - src[pixelSize - stride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize];
                        weightTotal += weight;

                        // x - 1, y + 1
                        gx = src[stride] - src[-pixelSize2 + stride];
                        gy = src[-pixelSize + 2 * stride] - src[-pixelSize];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize + stride];
                        weightTotal += weight;

                        // x, y + 1
                        gx = src[pixelSize + stride] - src[-pixelSize + stride];
                        gy = src[2 * stride] - *src;
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[stride];
                        weightTotal += weight;

                        // x + 1, y + 1
                        gx = src[pixelSize2 + stride] - src[stride];
                        gy = src[pixelSize + 2 * stride] - src[pixelSize];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize + stride];
                        weightTotal += weight;

                        // save destination value
                        *dst = ( weightTotal == 0.0 ) ? *src : (byte) System.Math.Min( total / weightTotal, 255.0 );
                    }
                }
                src += offset + 2 * pixelSize;
                dst += offset + 2 * pixelSize;
            }
        }
    }
}
