/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

// Parts of code have been adapted from Mono, 
// https://github.com/mono/sysdrawing-coregraphics/blob/master/System.Drawing/Color.cs
//
// Authors:
//         Dennis Hayes (dennish@raytek.com)
//         Ben Houston  (ben@exocortex.org)
//         Gonzalo Paniagua (gonzalo@ximian.com)
//         Juraj Skripsky (juraj@hotfeet.ch)
//        Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Dennis Hayes
// (c) 2002 Ximian, Inc. (http://www.ximiam.com)
// (C) 2005 HotFeet GmbH (http://www.hotfeet.ch)
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Drawing
{
    public partial struct Color
    {
        #region FIELDS

        internal static Color Red { get { return new Color(0xff, 0xff, 0x00, 0x00); } } 
        internal static Color Green { get { return new Color(0xff, 0x00, 0x80, 0x00); } }
        internal static Color Blue { get { return new Color(0xff, 0x00, 0x00, 0xff); } } 
        internal static Color Yellow { get { return new Color(0xff, 0xff, 0xff, 0x00); } }
        internal static Color Violet { get { return new Color(0xff, 0xee, 0x82, 0xee); } } 
        internal static Color Brown { get { return new Color(0xff, 0xa5, 0x2a, 0x2a); } } 
        internal static Color Olive { get { return new Color(0xff, 0x80, 0x80, 0x00); } } 
        internal static Color Cyan { get { return new Color(0xff, 0x00, 0xff, 0xff); } }
        internal static Color Magenta { get { return new Color(0xff, 0xff, 0x00, 0xff); } }
        internal static Color Gold { get { return new Color(0xff, 0xff, 0xd7, 0x00); } } 
        internal static Color Indigo { get { return new Color(0xff, 0x4b, 0x00, 0x82); } } 
        internal static Color Ivory { get { return new Color(0xff, 0xff, 0xff, 0xf0); } }
        internal static Color HotPink { get { return new Color(0xff, 0xff, 0x69, 0xb4); } } 
        internal static Color DarkRed { get { return new Color(0xff, 0x8b, 0x00, 0x00); } } 
        internal static Color DarkGreen { get { return new Color(0xff, 0x00, 0x64, 0x00); } } 
        internal static Color DarkBlue { get { return new Color(0xff, 0x00, 0x00, 0x8b); } }
        internal static Color DarkSeaGreen { get { return new Color(0xff, 0x8f, 0xbc, 0x8f); } } 
        internal static Color Gray { get { return new Color(0xff, 0x80, 0x80, 0x80); } } 
        internal static Color DarkKhaki { get { return new Color(0xff, 0xbd, 0xb7, 0x6b); } } 
        internal static Color DarkGray { get { return new Color(0xff, 0xa9, 0xa9, 0xa9); } }
        internal static Color LimeGreen { get { return new Color(0xff, 0x32, 0xcd, 0x32); } } 
        internal static Color Tomato { get { return new Color(0xff, 0xff, 0x63, 0x47); } } 
        internal static Color SteelBlue { get { return new Color(0xff, 0x46, 0x82, 0xb4); } } 
        internal static Color SkyBlue { get { return new Color(0xff, 0x87, 0xce, 0xeb); } }
        internal static Color Silver { get { return new Color(0xff, 0xc0, 0xc0, 0xc0); } } 
        internal static Color Salmon { get { return new Color(0xff, 0xfa, 0x80, 0x72); } } 
        internal static Color SaddleBrown { get { return new Color(0xff, 0x8b, 0x45, 0x13); } } 
        internal static Color RosyBrown { get { return new Color(0xff, 0xbc, 0x8f, 0x8f); } }
        internal static Color PowderBlue { get { return new Color(0xff, 0xb0, 0xe0, 0xe6); } } 
        internal static Color Plum { get { return new Color(0xff, 0xdd, 0xa0, 0xdd); } } 
        internal static Color PapayaWhip { get { return new Color(0xff, 0xff, 0xef, 0xd5); } } 
        internal static Color Orange { get { return new Color(0xff, 0xff, 0xa5, 0x00); } }
        internal static Color Black { get { return new Color(0xff, 0x00, 0x00, 0x00); } }
        internal static Color White { get { return new Color(0xff, 0xff, 0xff, 0xff); } }
        internal static Color DarkCyan { get { return new Color(0xff, 0x00, 0x8b, 0x8b); } }
        internal static Color DarkMagenta { get { return new Color(0xff, 0x8b, 0x00, 0x8b); } }
        internal static Color LightGray { get { return new Color(0xff, 0xd3, 0xd3, 0xd3); } }
	    internal static Color Transparent { get { return new Color(0x00, 0xff, 0xff, 0xff); } }
        
        private readonly byte _a;
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;

	    #endregion

        #region CONSTRUCTORS

        private Color(byte a, byte r, byte g, byte b)
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }
        
        #endregion

        #region PROPERTIES

        public byte A
        {
            get { return _a; }
        }

        public byte R
        {
            get { return _r; }
        }

        public byte G
        {
            get { return _g; }
        }

        public byte B
        {
            get { return _b; }
        }
        
        #endregion

        #region METHODS

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color((byte)a, (byte)r, (byte)g, (byte)b);
        }

        internal static Color FromArgb(int r, int g, int b)
        {
            return new Color(0xff, (byte)r, (byte)g, (byte)b);
        }

        internal static Color FromArgb(int alpha, Color baseColor)
		{
			return FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
		}

        internal static Color FromArgb(int argb)
        {
            return FromArgb((argb >> 24) & 0x0ff, (argb >> 16) & 0x0ff, (argb >> 8) & 0x0ff, argb & 0x0ff);
        }

        internal int ToArgb()
        {
            return (int)((uint)_a << 24) + (_r << 16) + (_g << 8) + _b;
        }

        internal float GetBrightness()
        {
            var minval = Math.Min(R, Math.Min(G, B));
            var maxval = Math.Max(R, Math.Max(G, B));

            return (float)(maxval + minval) / 510;
        }

        internal float GetSaturation()
        {
            var minval = Math.Min(R, Math.Min(G, B));
            var maxval = Math.Max(R, Math.Max(G, B));

            if (maxval == minval)
                return 0.0f;

            var sum = maxval + minval;
            if (sum > 255)
                sum = 510 - sum;

            return (float)(maxval - minval) / sum;
        }

        internal float GetHue()
        {
            int r = R;
            int g = G;
            int b = B;
            var minval = (byte)Math.Min(r, Math.Min(g, b));
            var maxval = (byte)Math.Max(r, Math.Max(g, b));

            if (maxval == minval)
                return 0.0f;

            var diff = (float)(maxval - minval);
            var rnorm = (maxval - r) / diff;
            var gnorm = (maxval - g) / diff;
            var bnorm = (maxval - b) / diff;

            var hue = 0.0f;
            if (r == maxval)
                hue = 60.0f * (6.0f + bnorm - gnorm);
            if (g == maxval)
                hue = 60.0f * (2.0f + rnorm - bnorm);
            if (b == maxval)
                hue = 60.0f * (4.0f + gnorm - rnorm);
            if (hue > 360.0f)
                hue = hue - 360.0f;

            return hue;
        }

        internal bool Equals(Color other)
        {
            return  _a == other._a && _r == other._r && _g == other._g && _b == other._b;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _a.GetHashCode();
                hashCode = (hashCode * 397) ^ _r.GetHashCode();
                hashCode = (hashCode * 397) ^ _g.GetHashCode();
                hashCode = (hashCode * 397) ^ _b.GetHashCode();
                return hashCode;
            }
        }
        
        #endregion

        #region OPERATORS

        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Color lhs, Color rhs)
        {
            return !(lhs == rhs);
        }

        #endregion
    }
}