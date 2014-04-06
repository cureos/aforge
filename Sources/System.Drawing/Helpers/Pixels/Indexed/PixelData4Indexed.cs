/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Runtime.InteropServices;

namespace ImagePixelEnumerator.Helpers.Pixels.Indexed
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct PixelData4Indexed : IIndexedPixel
    {
        // raw component values
        private Byte index;

        // get - index method
        public Byte GetIndex(Int32 offset)
        {
            return (Byte) GetBitRange(8 - offset - 4, 7 - offset);
        }

        // set - index method
        public void SetIndex(Int32 offset, Byte value)
        {
            SetBitRange(8 - offset - 4, 7 - offset, value);
        }

        private Int32 GetBitRange(Int32 startOffset, Int32 endOffset)
        {
            Int32 result = 0;
            Byte bitIndex = 0;

            for (Int32 offset = startOffset; offset <= endOffset; offset++)
            {
                Int32 bitValue = 1 << bitIndex;
                result += GetBit(offset) ? bitValue : 0;
                bitIndex++;
            }

            return result;
        }

        private Boolean GetBit(Int32 offset)
        {
            return (index & (1 << offset)) != 0;
        }

        private void SetBitRange(Int32 startOffset, Int32 endOffset, Int32 value)
        {
            Byte bitIndex = 0;

            for (Int32 offset = startOffset; offset <= endOffset; offset++)
            {
                Int32 bitValue = 1 << bitIndex;
                SetBit(offset, (value & bitValue) != 0);
                bitIndex++;
            }
        }

        private void SetBit(Int32 offset, Boolean value)
        {
            if (value)
            {
                index |= (Byte) (1 << offset);
            }
            else
            {
                index &= (Byte) (~(1 << offset));
            }
        }
    }
}
