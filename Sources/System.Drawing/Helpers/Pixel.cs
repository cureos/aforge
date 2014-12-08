/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using ImagePixelEnumerator.Helpers.Pixels;
using ImagePixelEnumerator.Helpers.Pixels.Indexed;
using ImagePixelEnumerator.Helpers.Pixels.NonIndexed;
using ImagePixelEnumerator.Quantizers;

namespace ImagePixelEnumerator.Helpers
{
    /// <summary>
    /// This is a pixel format independent pixel.
    /// </summary>
    internal class Pixel : IDisposable
    {
        #region | Constants |

        internal const Byte Zero = 0;
        internal const Byte One = 1;
        internal const Byte Two = 2;
        internal const Byte Four = 4;
        internal const Byte Eight = 8;

        internal const Byte NibbleMask = 0xF;
        internal const Byte ByteMask = 0xFF;

        internal const Int32 AlphaShift = 24;
        internal const Int32 RedShift = 16;
        internal const Int32 GreenShift = 8;
        internal const Int32 BlueShift = 0;

        internal const Int32 AlphaMask = ByteMask << AlphaShift;
        internal const Int32 RedGreenBlueMask = 0xFFFFFF;
        internal const Single HighColorFactor = 8192.0f/256.0f;

        #endregion

        #region | Fields |

        private Type pixelType;
        private Int32 bitOffset;
        private Object pixelData;
        private IntPtr pixelDataPointer;

        #endregion

        #region | Properties |

        /// <summary>
        /// Gets the X.
        /// </summary>
        public Int32 X { get; private set; }

        /// <summary>
        /// Gets the Y.
        /// </summary>
        public Int32 Y { get; private set; }

        /// <summary>
        /// Gets the parent buffer.
        /// </summary>
        public ImageBuffer Parent { get; private set; }

        #endregion

        #region | Calculated properties |

        /// <summary>
        /// Gets a value indicating whether this instance is indexed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is indexed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIndexed
        {
            get { return Parent.IsIndexed; }
        }

        #endregion

        #region | Constructors |

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> struct.
        /// </summary>
        public Pixel(ImageBuffer parent)
        {
            Parent = parent;

            Initialize();
        }

        private void Initialize()
        {
            // creates pixel data
            pixelType = IsIndexed ? GetIndexedType(Parent.PixelFormat) : GetNonIndexedType(Parent.PixelFormat);
            NewExpression newType = Expression.New(pixelType);
            UnaryExpression convertNewType = Expression.Convert(newType, typeof(Object));
            Expression<Func<Object>> indexedExpression = Expression.Lambda<Func<Object>>(convertNewType);
            pixelData = indexedExpression.Compile().Invoke();
            pixelDataPointer = MarshalToPointer(pixelData);
        }

        #endregion

        #region | Read methods |

        public Int32 GetIndex()
        {
            Int32 result;

            // determines whether the format is indexed
            if (IsIndexed)
            {
                result = ((IIndexedPixel) pixelData).GetIndex(bitOffset);
            }
            else // not possible to get index from a non-indexed format
            {
                String message = string.Format("Cannot retrieve index for a non-indexed format. Please use Color (or Value) property instead.");
                throw new NotSupportedException(message);
            }

            return result;
        }

        public Color GetColor()
        {
            Color result;

            // retrieves color by the index
            if (IsIndexed)
            {
                Int32 index = ((IIndexedPixel) pixelData).GetIndex(bitOffset);
                result = Parent.GetPaletteColor(index);
            }
            else // retrieves color directly
            {
                result = ((INonIndexedPixel) pixelData).GetColor();
            }

            // returns the color
            return result;
        }

        #endregion

        #region | Write methods |

        public void SetIndex(Int32 index, Byte[] buffer = null)
        {
            // determines whether the format is indexed
            if (IsIndexed)
            {
                ((IIndexedPixel) pixelData).SetIndex(bitOffset, (Byte) index);
            }
            else // cannot write color to an indexed format
            {
                String message = string.Format("Cannot set index for a non-indexed format. Please use Color (or Value) property instead.");
                throw new NotSupportedException(message);
            }
        }

        public void SetColor(Color color, IColorQuantizer quantizer)
        {
            // determines whether the format is indexed
            if (IsIndexed)
            {
                // last chance if quantizer is provided, use it
                if (quantizer != null)
                {
                    Byte index = (Byte) quantizer.GetPaletteIndex(color, X, Y);
                    ((IIndexedPixel) pixelData).SetIndex(bitOffset, index);
                }
                else // cannot write color to an index format
                {
                    String message = string.Format("Cannot retrieve color for an indexed format. Use GetPixelIndex() instead.");
                    throw new NotSupportedException(message);
                }
            }
            else // sets color to a non-indexed format
            {
                ((INonIndexedPixel) pixelData).SetColor(color);
            }
        }

        #endregion

        #region | Helper methods |

        /// <summary>
        /// Gets the type of the indexed pixel format.
        /// </summary>
        internal Type GetIndexedType(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed: return typeof(PixelData1Indexed);
                case PixelFormat.Format4bppIndexed: return typeof(PixelData4Indexed);
                case PixelFormat.Format8bppIndexed: return typeof(PixelData8Indexed);

                default:
                    String message = String.Format("This pixel format '{0}' is either non-indexed, or not supported.", pixelFormat);
                    throw new NotSupportedException(message);
            }
        }

        /// <summary>
        /// Gets the type of the non-indexed pixel format.
        /// </summary>
        internal Type GetNonIndexedType(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format16bppArgb1555: return typeof(PixelDataArgb1555);
                case PixelFormat.Format16bppGrayScale: return typeof(PixelDataGray16);
                case PixelFormat.Format16bppRgb555: return typeof(PixelDataRgb555);
                case PixelFormat.Format16bppRgb565: return typeof(PixelDataRgb565);
                case PixelFormat.Format24bppRgb: return typeof(PixelDataRgb888);
                case PixelFormat.Format32bppRgb: return typeof(PixelDataRgb8888);
                case PixelFormat.Format32bppArgb: return typeof(PixelDataArgb8888);
                case PixelFormat.Format32bppPArgb: return typeof (PixelDataPArgb8888);
                case PixelFormat.Format48bppRgb: return typeof(PixelDataRgb48);
                case PixelFormat.Format64bppArgb: return typeof(PixelDataArgb64);
                // case PixelFormat.Format64bppPArgb: return typeof (PixelDataPArgb64);

                case PixelFormat.Format32bppRgbaProprietary: return typeof(PixelDataRgba8888);
                case PixelFormat.Format32bppPRgbaProprietary: return typeof(PixelDataPRgba8888);

                default:
                    String message = String.Format("This pixel format '{0}' is either indexed, or not supported.", pixelFormat);
                    throw new NotSupportedException(message);
            }
        }

        private static IntPtr MarshalToPointer(Object data)
        {
            Int32 size = Marshal.SizeOf(data);
            IntPtr pointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, pointer, false);
            return pointer;
        }

        #endregion

        #region | Update methods |

        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void Update(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
            bitOffset = Parent.GetBitOffset(x);
        }

        /// <summary>
        /// Reads the raw data.
        /// </summary>
        /// <param name="imagePointer">The image pointer.</param>
        public void ReadRawData(IntPtr imagePointer)
        {
            pixelData = Marshal.PtrToStructure(imagePointer, pixelType);
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public void ReadData(Byte[] buffer, Int32 offset)
        {
            Marshal.Copy(buffer, offset, pixelDataPointer, Parent.BytesPerPixel);
            pixelData = Marshal.PtrToStructure(pixelDataPointer, pixelType);
        }

        /// <summary>
        /// Writes the raw data.
        /// </summary>
        /// <param name="imagePointer">The image pointer.</param>
        public void WriteRawData(IntPtr imagePointer)
        {
            Marshal.StructureToPtr(pixelData, imagePointer, false);
        }

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public void WriteData(Byte[] buffer, Int32 offset)
        {
            Marshal.Copy(pixelDataPointer, buffer, offset, Parent.BytesPerPixel);
        }

        #endregion

        #region << IDisposable >>

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(pixelDataPointer);
        }

        #endregion
    }
}


