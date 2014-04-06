/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using ImagePixelEnumerator.PathProviders;

namespace ImagePixelEnumerator.Quantizers
{
    /// <summary>
    /// This interface provides a color quantization capabilities.
    /// </summary>
    internal interface IColorQuantizer : IPathProvider
    {
        /// <summary>
        /// Gets a value indicating whether to allow parallel processing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to allow parallel processing; otherwise, <c>false</c>.
        /// </value>
        Boolean AllowParallel { get; }

        /// <summary>
        /// Prepares the quantizer for image processing.
        /// </summary>
        void Prepare(Image image);

        /// <summary>
        /// Adds the color to quantizer.
        /// </summary>
        void AddColor(Color color, Int32 x, Int32 y);

        /// <summary>
        /// Gets the palette with specified count of the colors.
        /// </summary>
        List<Color> GetPalette(Int32 colorCount);

        /// <summary>
        /// Gets the index of the palette for specific color.
        /// </summary>
        Int32 GetPaletteIndex(Color color, Int32 x, Int32 y);

        /// <summary>
        /// Gets the color count.
        /// </summary>
        Int32 GetColorCount();

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Finish();
    }
}