// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//
// Original idea from CxImage
// http://www.codeproject.com/bitmap/cximage.asp
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Dithering using Burkes error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Burkes coefficients. Error is diffused
    /// on 7 neighbor pixels with next coefficients:</para>
    /// <code>
    ///         | * | 8 | 4 |
    /// | 2 | 4 | 8 | 4 | 2 |
    /// 
    /// / 32
    /// </code>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BurkesDithering filter = new BurkesDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="burkes.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public sealed class BurkesDithering : ErrorDiffusionDithering
    {
        private static int[] coef = new int[] { 2, 4, 8, 4, 2 };

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
            if ( x < stopXM1 )
            {
                // right pixel
                ed = ptr[1] + ( error * 8 ) / 32;
                ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                ptr[1] = (byte) ed;
            }

            if ( x < stopXM1 - 1 )
            {
                // right + 1 pixel
                ed = ptr[2] + ( error * 4 ) / 32;
                ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                ptr[2] = (byte) ed;
            }

            if ( y != stopYM1 )
            {
                // bottom pixels
                ptr += stride;
                for ( int i = -2, j = 0; i <= 2; i++, j++ )
                {
                    if ( ( x + i >= startX ) && ( x + i < stopX ) )
                    {
                        ed = ptr[i] + ( error * coef[j] ) / 32;
                        ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                        ptr[i] = (byte) ed;
                    }
                }
            }
        }
    }
}
