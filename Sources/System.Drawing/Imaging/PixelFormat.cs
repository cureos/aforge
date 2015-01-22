/*
 *  Copyright (c) 2013-2015, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.Drawing.
 *
 *  Shim.Drawing is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.Drawing is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.Drawing.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace System.Drawing.Imaging
{
    [Flags]
    public enum PixelFormat
    {
// ReSharper disable InconsistentNaming
        Format1bppIndexed = 196865,
        Format4bppIndexed = 197634,
        Format8bppIndexed = 198659,
        Format16bppArgb1555 = 397319,
        Format16bppGrayScale = 1052676,
        Format16bppRgb555 = 135173,
        Format16bppRgb565 = 135174,
        Format24bppRgb = 137224,
        Format32bppRgb = 139273,
        Format32bppArgb = 2498570,
        Format32bppPArgb = 925707,
        Format48bppRgb = 1060876,
        Format64bppArgb = 3424269,
        Format64bppPArgb = 1851406,
// ReSharper restore InconsistentNaming
        Indexed = 0x00010000,
        Gdi = 0x00020000,
        Alpha = 0x00040000,
        PAlpha = 0x00080000,
        Canonical = 0x00200000,
        Undefined = 0,
// ReSharper disable InconsistentNaming
        Format32bppRgbaProprietary = 17 | (32 << 8) | Alpha | Gdi | Canonical,
        Format32bppPRgbaProprietary = 18 | (32 << 8) | Alpha | PAlpha | Gdi
// ReSharper restore InconsistentNaming
    }
}