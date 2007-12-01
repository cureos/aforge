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
    /// Dithering using Sierra error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Sierra coefficients. Error is diffused
    /// on 10 neighbor pixels with next coefficients:</para>
    /// <code>
    ///         | * | 5 | 3 |
    /// | 2 | 4 | 5 | 4 | 2 |
    ///     | 2 | 3 | 2 |
    /// 
    /// / 32
    /// </code>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SierraDithering filter = new SierraDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="sierra.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public sealed class SierraDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SierraDithering"/> class.
        /// </summary>
        /// 
        public SierraDithering( ) : base( new int[3][] {
            new int[2] { 5, 3 },
            new int[5] { 2, 4, 5, 4, 2 },
            new int[3] { 2, 3, 2 }
        } )
        {
        }
    }
}
