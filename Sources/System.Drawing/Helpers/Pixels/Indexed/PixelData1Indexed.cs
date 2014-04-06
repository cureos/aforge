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
    internal struct PixelData1Indexed : IIndexedPixel
    {
        // raw component values
        private Byte index;

        // get - index method
        public Byte GetIndex(Int32 offset)
        {
            return (index & 1 << (7 - offset)) != 0 ? Pixel.One : Pixel.Zero;
        }

        // set - index method
        public void SetIndex(Int32 offset, Byte value)
        {
            value = value == 0 ? Pixel.One : Pixel.Zero;

            if (value == 0)
            {
                index |= (Byte) (1 << (7 - offset));
            }
            else
            {
                index &= (Byte) (~(1 << (7 - offset)));
            }
        }
    }
}
