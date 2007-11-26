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
    /// Dithering using Floyd-Steinberg error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Floyd-Steinberg coefficients. Error is diffused
    /// on 4 neighbor pixels with next coefficients:</para>
    /// <code>
    ///      |  * |  7 |
    /// |  3 |  5 |  1 |
    /// 
    /// / 16
    /// </code>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// FloydSteinbergDithering filter = new FloydSteinbergDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="floyd_steinberg.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public sealed class FloydSteinbergDithering : ErrorDiffusionDithering
    {
        private static int[] coef = new int[] { 3, 5, 1 };

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="error">Current error value.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized by base class.</remarks>
        /// 
        protected override unsafe void Diffuse( int error, byte* ptr )
        {
            int ed;	// error diffusion

            // calculate error diffusion
            if ( x != stopXM1 )
            {
                // right pixel
                ed = ptr[1] + ( error * 7 ) / 16;
                ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                ptr[1] = (byte) ed;
            }

            if ( y != stopYM1 )
            {
                // bottom pixels
                ptr += stride;
                for ( int i = -1, j = 0; i <= 1; i++, j++ )
                {
                    if ( ( x + i >= startX ) && ( x + i < stopX ) )
                    {
                        ed = ptr[i] + ( error * coef[j] ) / 16;
                        ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                        ptr[i] = (byte) ed;
                    }
                }
            }
        }
    }
}
