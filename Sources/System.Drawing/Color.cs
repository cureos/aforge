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

        private static readonly Color _red = new Color(0xff, 0xff, 0x00, 0x00); 
        private static readonly Color _green = new Color(0xff, 0x00, 0x80, 0x00);
        private static readonly Color _blue = new Color(0xff, 0x00, 0x00, 0xff); 
        private static readonly Color _yellow = new Color(0xff, 0xff, 0xff, 0x00);
        private static readonly Color _violet = new Color(0xff, 0xee, 0x82, 0xee); 
        private static readonly Color _brown = new Color(0xff, 0xa5, 0x2a, 0x2a); 
        private static readonly Color _olive = new Color(0xff, 0x80, 0x80, 0x00); 
        private static readonly Color _cyan = new Color(0xff, 0x00, 0xff, 0xff);
        private static readonly Color _magenta = new Color(0xff, 0xff, 0x00, 0xff);
        private static readonly Color _gold = new Color(0xff, 0xff, 0xd7, 0x00); 
        private static readonly Color _indigo = new Color(0xff, 0x4b, 0x00, 0x82); 
        private static readonly Color _ivory = new Color(0xff, 0xff, 0xff, 0xf0);
        private static readonly Color _pink = new Color(0xff, 0xff, 0xc0, 0xcb);
        private static readonly Color _hotPink = new Color(0xff, 0xff, 0x69, 0xb4); 
        private static readonly Color _darkRed = new Color(0xff, 0x8b, 0x00, 0x00); 
        private static readonly Color _darkGreen = new Color(0xff, 0x00, 0x64, 0x00); 
        private static readonly Color _darkBlue = new Color(0xff, 0x00, 0x00, 0x8b);
        private static readonly Color _darkSeaGreen = new Color(0xff, 0x8f, 0xbc, 0x8f); 
        private static readonly Color _gray = new Color(0xff, 0x80, 0x80, 0x80); 
        private static readonly Color _darkKhaki = new Color(0xff, 0xbd, 0xb7, 0x6b); 
        private static readonly Color _darkGray = new Color(0xff, 0xa9, 0xa9, 0xa9);
        private static readonly Color _limeGreen = new Color(0xff, 0x32, 0xcd, 0x32); 
        private static readonly Color _tomato = new Color(0xff, 0xff, 0x63, 0x47); 
        private static readonly Color _steelBlue = new Color(0xff, 0x46, 0x82, 0xb4); 
        private static readonly Color _skyBlue = new Color(0xff, 0x87, 0xce, 0xeb);
        private static readonly Color _silver = new Color(0xff, 0xc0, 0xc0, 0xc0); 
        private static readonly Color _salmon = new Color(0xff, 0xfa, 0x80, 0x72); 
        private static readonly Color _saddleBrown = new Color(0xff, 0x8b, 0x45, 0x13); 
        private static readonly Color _rosyBrown = new Color(0xff, 0xbc, 0x8f, 0x8f);
        private static readonly Color _powderBlue = new Color(0xff, 0xb0, 0xe0, 0xe6); 
        private static readonly Color _plum = new Color(0xff, 0xdd, 0xa0, 0xdd); 
        private static readonly Color _papayaWhip = new Color(0xff, 0xff, 0xef, 0xd5); 
        private static readonly Color _orange = new Color(0xff, 0xff, 0xa5, 0x00);
        private static readonly Color _black = new Color(0xff, 0x00, 0x00, 0x00);
        private static readonly Color _white = new Color(0xff, 0xff, 0xff, 0xff);
        private static readonly Color _darkCyan = new Color(0xff, 0x00, 0x8b, 0x8b);
        private static readonly Color _darkMagenta = new Color(0xff, 0x8b, 0x00, 0x8b);
        private static readonly Color _lightGray = new Color(0xff, 0xd3, 0xd3, 0xd3);
        private static readonly Color _transparent = new Color(0x00, 0xff, 0xff, 0xff);
        private static readonly Color _yellowGreen = new Color(0xff, 0x9a, 0xcd, 0x32);
        
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

        internal static Color Red { get { return _red; } }
        internal static Color Green { get { return _green; } }
        internal static Color Blue { get { return _blue; } }
        internal static Color Yellow { get { return _yellow; } }
        internal static Color Violet { get { return _violet; } }
        internal static Color Brown { get { return _brown; } }
        internal static Color Olive { get { return _olive; } }
        internal static Color Cyan { get { return _cyan; } }
        internal static Color Magenta { get { return _magenta; } }
        internal static Color Gold { get { return _gold; } }
        internal static Color Indigo { get { return _indigo; } }
        internal static Color Ivory { get { return _ivory; } }
        internal static Color Pink { get { return _pink; } }
        internal static Color HotPink { get { return _hotPink; } }
        internal static Color DarkRed { get { return _darkRed; } }
        internal static Color DarkGreen { get { return _darkGreen; } }
        internal static Color DarkBlue { get { return _darkBlue; } }
        internal static Color DarkSeaGreen { get { return _darkSeaGreen; } }
        internal static Color Gray { get { return _gray; } }
        internal static Color DarkKhaki { get { return _darkKhaki; } }
        internal static Color DarkGray { get { return _darkGray; } }
        internal static Color LimeGreen { get { return _limeGreen; } }
        internal static Color Tomato { get { return _tomato; } }
        internal static Color SteelBlue { get { return _steelBlue; } }
        internal static Color SkyBlue { get { return _skyBlue; } }
        internal static Color Silver { get { return _silver; } }
        internal static Color Salmon { get { return _salmon; } }
        internal static Color SaddleBrown { get { return _saddleBrown; } }
        internal static Color RosyBrown { get { return _rosyBrown; } }
        internal static Color PowderBlue { get { return _powderBlue; } }
        internal static Color Plum { get { return _plum; } }
        internal static Color PapayaWhip { get { return _papayaWhip; } }
        internal static Color Orange { get { return _orange; } }
        internal static Color Black { get { return _black; } }
        internal static Color White { get { return _white; } }
        internal static Color DarkCyan { get { return _darkCyan; } }
        internal static Color DarkMagenta { get { return _darkMagenta; } }
        internal static Color LightGray { get { return _lightGray; } }
        internal static Color Transparent { get { return _transparent; } }
        internal static Color YellowGreen { get { return _yellowGreen; } }

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