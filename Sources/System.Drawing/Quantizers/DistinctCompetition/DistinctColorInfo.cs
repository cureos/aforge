/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Drawing;

namespace ImagePixelEnumerator.Quantizers.DistinctCompetition
{
    /// <summary>
    /// Stores all the informations about single color only once, to be used later.
    /// </summary>
    internal class DistinctColorInfo
    {
        private const Int32 Factor = 5000000;

        /// <summary>
        /// The original color.
        /// </summary>
        public Int32 Color { get; private set; }

        /// <summary>
        /// The pixel presence count in the image.
        /// </summary>
        public Int32 Count { get; private set; }

        /// <summary>
        /// A hue component of the color.
        /// </summary>
        public Int32 Hue { get; private set; }

        /// <summary>
        /// A saturation component of the color.
        /// </summary>
        public Int32 Saturation { get; private set; }

        /// <summary>
        /// A brightness component of the color.
        /// </summary>
        public Int32 Brightness { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctColorInfo"/> struct.
        /// </summary>
        public DistinctColorInfo(Color color)
        {
            Color = color.ToArgb();
            Count = 1;

            Hue = Convert.ToInt32(color.GetHue() * Factor);
            Saturation = Convert.ToInt32(color.GetSaturation() * Factor);
            Brightness = Convert.ToInt32(color.GetBrightness() * Factor);
        }

        /// <summary>
        /// Increases the count of pixels of this color.
        /// </summary>
        public DistinctColorInfo IncreaseCount()
        {
            Count++;
            return this;
        }
    }
}
