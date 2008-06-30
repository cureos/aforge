
// joan.charmant@gmail.com

namespace AForge.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Block matchers interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies set of methods, which should be implemented by different
    /// block matching  algorithms.</para></remarks>
    /// 
    public interface IBlockMatcher
    {
        /// <summary>
        /// Process images matching blocks between images.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image with reference points.</param>
        /// <param name="coordinates">Array of reference points to be matched.</param>
        /// <param name="searchImage">Image in which the reference points will be looked for.</param>
        /// <param name="relative">True if results should be given as relative displacement, false for absolute coordinates.</param>
        /// 
        /// <returns>Returns array of relative displacements or absolute coordinates</returns>
        /// 
        /// <exception cref="ArgumentException">Source images sizes must match.</exception>
        /// <exception cref="ArgumentException">Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and overlay images must have same pixel format.</exception>
        /// 
        Point[] ProcessImage(Bitmap sourceImage, Point[] coordinates, Bitmap searchImage, bool relative);

        /// <summary>
        /// Process images matching blocks between images.
        /// </summary>
        /// 
        /// <param name="sourceImageDataBitmap">Source image with reference points.</param>
        /// <param name="coordinates">Array of reference points to be matched.</param>
        /// <param name="searchImageDataBitmap">Image in which the reference points will be looked for.</param>
        /// <param name="relative">True if result should be given as relative displacement, false for absolute coordinates.</param>
        /// 
        /// <returns>Returns array of relative displacements or absolute coordinates</returns>
        /// 
        /// <exception cref="ArgumentException">Source images sizes must match.</exception>
        /// <exception cref="ArgumentException">Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and overlay images must have same pixel format.</exception>
        /// 
        Point[] ProcessImage(BitmapData sourceImageDataBitmap, Point[] coordinates, BitmapData searchImageDataBitmap, bool relative);


    }
}
