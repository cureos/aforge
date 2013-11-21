// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

#if NETFX_CORE
using TrueColor = Windows.UI.Color;
#endif

namespace System.Drawing
{
    public struct Color
    {
        #region FIELDS

        public static readonly Color Red = new Color(0xff, 0xff, 0x00, 0x00); 
        public static readonly Color Green = new Color(0xff, 0x00, 0x80, 0x00);
        public static readonly Color Blue = new Color(0xff, 0x00, 0x00, 0xff); 
        public static readonly Color Yellow = new Color(0xff, 0xff, 0xff, 0x00);
        public static readonly Color Violet = new Color(0xff, 0xee, 0x82, 0xee); 
        public static readonly Color Brown = new Color(0xff, 0xa5, 0x2a, 0x2a); 
        public static readonly Color Olive = new Color(0xff, 0x80, 0x80, 0x00); 
        public static readonly Color Cyan = new Color(0xff, 0x00, 0xff, 0xff);
        public static readonly Color Magenta = new Color(0xff, 0xff, 0x00, 0xff);
        public static readonly Color Gold = new Color(0xff, 0xff, 0xd7, 0x00); 
        public static readonly Color Indigo = new Color(0xff, 0x4b, 0x00, 0x82); 
        public static readonly Color Ivory = new Color(0xff, 0xff, 0xff, 0xf0);
        public static readonly Color HotPink = new Color(0xff, 0xff, 0x69, 0xb4); 
        public static readonly Color DarkRed = new Color(0xff, 0x8b, 0x00, 0x00); 
        public static readonly Color DarkGreen = new Color(0xff, 0x00, 0x64, 0x00); 
        public static readonly Color DarkBlue = new Color(0xff, 0x00, 0x00, 0x8b);
        public static readonly Color DarkSeaGreen = new Color(0xff, 0x8f, 0xbc, 0x8f); 
        public static readonly Color Gray = new Color(0xff, 0x80, 0x80, 0x80); 
        public static readonly Color DarkKhaki = new Color(0xff, 0xbd, 0xb7, 0x6b); 
        public static readonly Color DarkGray = new Color(0xff, 0xa9, 0xa9, 0xa9);
        public static readonly Color LimeGreen = new Color(0xff, 0x32, 0xcd, 0x32); 
        public static readonly Color Tomato = new Color(0xff, 0xff, 0x63, 0x47); 
        public static readonly Color SteelBlue = new Color(0xff, 0x46, 0x82, 0xb4); 
        public static readonly Color SkyBlue = new Color(0xff, 0x87, 0xce, 0xeb);
        public static readonly Color Silver = new Color(0xff, 0xc0, 0xc0, 0xc0); 
        public static readonly Color Salmon = new Color(0xff, 0xfa, 0x80, 0x72); 
        public static readonly Color SaddleBrown = new Color(0xff, 0x8b, 0x45, 0x13); 
        public static readonly Color RosyBrown = new Color(0xff, 0xbc, 0x8f, 0x8f);
        public static readonly Color PowderBlue = new Color(0xff, 0xb0, 0xe0, 0xe6); 
        public static readonly Color Plum = new Color(0xff, 0xdd, 0xa0, 0xdd); 
        public static readonly Color PapayaWhip = new Color(0xff, 0xff, 0xef, 0xd5); 
        public static readonly Color Orange = new Color(0xff, 0xff, 0xa5, 0x00);
        public static readonly Color Black = new Color(0xff, 0x00, 0x00, 0x00);
        public static readonly Color White = new Color(0xff, 0xff, 0xff, 0xff);
        public static readonly Color DarkCyan = new Color(0xff, 0x00, 0x8b, 0x8b);
        public static readonly Color DarkMagenta = new Color(0xff, 0x8b, 0x00, 0x8b);
        public static readonly Color LightGray = new Color(0xff, 0xd3, 0xd3, 0xd3);
        
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

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(0xff, (byte)r, (byte)g, (byte)b);
        }

        public bool Equals(Color other)
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

        public static implicit operator TrueColor(Color color)
        {
            return TrueColor.FromArgb(color._a, color._r, color._g, color._b);
        }

        public static implicit operator Color(TrueColor trueColor)
        {
            return FromArgb(trueColor.A, trueColor.R, trueColor.G, trueColor.B);
        }

        #endregion
    }
}