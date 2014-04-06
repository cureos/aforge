/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;

namespace ImagePixelEnumerator.Helpers.Pixels
{
    internal interface IIndexedPixel
    {
        // index methods
        Byte GetIndex(Int32 offset);
        void SetIndex(Int32 offset, Byte value);
    }
}
