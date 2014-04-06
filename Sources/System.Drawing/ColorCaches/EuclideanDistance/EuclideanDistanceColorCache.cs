/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using ImagePixelEnumerator.ColorCaches.Common;
using ImagePixelEnumerator.Helpers;

namespace ImagePixelEnumerator.ColorCaches.EuclideanDistance
{
    internal class EuclideanDistanceColorCache : BaseColorCache
    {
        #region | Fields |

        private IList<Color> palette;

        #endregion

        #region | Properties |

        /// <summary>
        /// Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is color model supported; otherwise, <c>false</c>.
        /// </value>
        public override Boolean IsColorModelSupported
        {
            get { return true; }
        }

        #endregion

        #region | Constructors |

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistanceColorCache"/> class.
        /// </summary>
        public EuclideanDistanceColorCache()
        {
            ColorModel = ColorModel.RedGreenBlue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistanceColorCache"/> class.
        /// </summary>
        /// <param name="colorModel">The color model.</param>
        public EuclideanDistanceColorCache(ColorModel colorModel)
        {
            ColorModel = colorModel;
        }

        #endregion

        #region << BaseColorCache >>

        /// <summary>
        /// See <see cref="BaseColorCache.OnCachePalette"/> for more details.
        /// </summary>
        protected override void OnCachePalette(IList<Color> palette)
        {
            this.palette = palette;
        }

        /// <summary>
        /// See <see cref="BaseColorCache.OnGetColorPaletteIndex"/> for more details.
        /// </summary>
        protected override void OnGetColorPaletteIndex(Color color, out Int32 paletteIndex)
        {
            paletteIndex = ColorModelHelper.GetEuclideanDistance(color, ColorModel, palette);
        }

        #endregion
    }
}