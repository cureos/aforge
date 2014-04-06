/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Collections.Generic;
using ImagePixelEnumerator.ColorCaches.Common;

namespace ImagePixelEnumerator.ColorCaches
{
    internal abstract class BaseColorCache : IColorCache
    {
        #region | Fields |

        private readonly ConcurrentDictionary<Int32, Int32> cache;

        #endregion

        #region | Properties |

        /// <summary>
        /// Gets or sets the color model.
        /// </summary>
        /// <value>The color model.</value>
        protected ColorModel ColorModel { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is color model supported; otherwise, <c>false</c>.
        /// </value>
        public abstract Boolean IsColorModelSupported { get; }

        #endregion

        #region | Constructors |

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseColorCache"/> class.
        /// </summary>
        protected BaseColorCache()
        {
            cache = new ConcurrentDictionary<Int32, Int32>();
        }

        #endregion

        #region | Methods |

        /// <summary>
        /// Changes the color model.
        /// </summary>
        /// <param name="colorModel">The color model.</param>
        public void ChangeColorModel(ColorModel colorModel)
        {
            ColorModel = colorModel;
        }

        #endregion

        #region << Abstract methods |

        /// <summary>
        /// Called when a palette is about to be cached, or precached.
        /// </summary>
        /// <param name="palette">The palette.</param>
        protected abstract void OnCachePalette(IList<Color> palette);

        /// <summary>
        /// Called when palette index is about to be retrieve for a given color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="paletteIndex">Index of the palette.</param>
        protected abstract void OnGetColorPaletteIndex(Color color, out Int32 paletteIndex);

        #endregion

        #region << IColorCache >>

        /// <summary>
        /// See <see cref="IColorCache.Prepare"/> for more details.
        /// </summary>
        public virtual void Prepare()
        {
            cache.Clear();
        }

        /// <summary>
        /// See <see cref="IColorCache.CachePalette"/> for more details.
        /// </summary>
        public void CachePalette(IList<Color> palette)
        {
            OnCachePalette(palette);
        }

        /// <summary>
        /// See <see cref="IColorCache.GetColorPaletteIndex"/> for more details.
        /// </summary>
        public void GetColorPaletteIndex(Color color, out Int32 paletteIndex)
        {
            Int32 key = color.R << 16 | color.G << 8 | color.B;

            paletteIndex = cache.AddOrUpdate(key,
                colorKey =>
                {
                    Int32 paletteIndexInside;
                    OnGetColorPaletteIndex(color, out paletteIndexInside);
                    return paletteIndexInside;
                },
                (colorKey, inputIndex) => inputIndex);
        }

        #endregion
    }
}
