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
    public class AdaptiveSmooth : FilterAnyToAnyUsingCopy
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
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData )
        {
            // get source image size
            int width       = destinationData.Width;
            int height      = destinationData.Height;
            int widthM2     = width - 2;
            int heightM2    = height - 2;

            int stride      = destinationData.Stride;
            int pixelSize   = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int pixelSize2  = pixelSize * 2;
            int offset      = stride - width * pixelSize;

            // gradient and weights
            double gx, gy, weight, weightTotal, total;
            // precalculated factor value
            double f = -8 * factor * factor;

            // do the job
            byte* src = (byte*) sourceData.ToPointer( ) + stride * 2;
            byte* dst = (byte*) destinationData.Scan0.ToPointer( ) + stride * 2;

            for ( int y = 2; y < heightM2; y++ )
            {
                src += 2 * pixelSize;
                dst += 2 * pixelSize;

                for ( int x = 2; x < widthM2; x++ )
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
