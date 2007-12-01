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
    /// Dithering using Stevenson and Arce error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Stevenson-Arce coefficients. Error is diffused
    /// on 12 neighbor pixels with next coefficients:</para>
    /// <code>
    ///                |  * |    | 32 |
    /// | 12 |    | 26 |    | 30 |    | 16 |
    ///      | 12 |    | 26 |    | 12 |
    /// |  5 |    | 12 |    | 12 |    |  5 |
    /// 
    /// / 200
    /// </code>
    /// 
    /// <para><note>This class is marked as obsolete and should not be used for real
    /// image processing tasks. The Stevenson-Arce error diffusion is supposed to be used
    /// with hexagonal pixel lattices and that is why it gives poor result on regular
    /// images. The class is kept only for educational purpose.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// StevensonArceDithering filter = new StevensonArceDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="stevenson_arce.jpg" width="480" height="361" />
    /// </remarks>
    ///
    [Obsolete( "The class is kept only for educational purpose." )]
    public sealed class StevensonArceDithering : ErrorDiffusionDithering
    {
        private static int[] coef1 = new int[] { 12, 26, 30, 16 };
        private static int[] coef2 = new int[] { 12, 26, 12 };
        private static int[] coef3 = new int[] { 5, 12, 12, 5 };

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
            if ( x < stopX - 2 )
            {
                // right + 1 pixel
                ed = ptr[2] + ( error * 32 ) / 200;
                ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                ptr[2] = (byte) ed;
            }

            if ( y < stopY - 1 )
            {
                // bottom pixels
                ptr += stride;
                for ( int i = -3, j = 0; i <= 3; i += 2, j++ )
                {
                    if ( ( x + i >= startX ) && ( x + i < stopX ) )
                    {
                        ed = ptr[i] + ( error * coef1[j] ) / 200;
                        ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                        ptr[i] = (byte) ed;
                    }
                }
            }

            if ( y < stopY - 2 )
            {
                // bottom + 1 pixels
                ptr += stride;
                for ( int i = -2, j = 0; i <= 2; i += 2, j++ )
                {
                    if ( ( x + i >= startX ) && ( x + i < stopX ) )
                    {
                        ed = ptr[i] + ( error * coef2[j] ) / 200;
                        ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                        ptr[i] = (byte) ed;
                    }
                }
            }

            if ( y < stopY - 3 )
            {
                // bottom + 2 pixels
                ptr += stride;
                for ( int i = -3, j = 0; i <= 3; i += 2, j++ )
                {
                    if ( ( x + i >= startX ) && ( x + i < stopX ) )
                    {
                        ed = ptr[i] + ( error * coef3[j] ) / 200;
                        ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                        ptr[i] = (byte) ed;
                    }
                }
            }
        }
    }
}
