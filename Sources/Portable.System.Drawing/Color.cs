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
using ActualColor = Windows.UI.Color;
#endif

namespace System.Drawing
{
    public struct Color
    {
        #region FIELDS

        public static readonly Color Red = new Color(0xff, 0xff, 0x00, 0x00); 
        public static readonly Color Green = new Color(0xff, 0x00, 0x80, 0x00);
        public static readonly Color Blue = new Color(0xff, 0x00, 0x00, 0xff); 
        public static readonly Color Yellow;
        public static readonly Color Violet; 
        public static readonly Color Brown; 
        public static readonly Color Olive; 
        public static readonly Color Cyan;
        public static readonly Color Magenta; 
        public static readonly Color Gold; 
        public static readonly Color Indigo; 
        public static readonly Color Ivory;
        public static readonly Color HotPink; 
        public static readonly Color DarkRed; 
        public static readonly Color DarkGreen; 
        public static readonly Color DarkBlue;
        public static readonly Color DarkSeaGreen; 
        public static readonly Color Gray; 
        public static readonly Color DarkKhaki; 
        public static readonly Color DarkGray;
        public static readonly Color LimeGreen; 
        public static readonly Color Tomato; 
        public static readonly Color SteelBlue; 
        public static readonly Color SkyBlue;
        public static readonly Color Silver; 
        public static readonly Color Salmon; 
        public static readonly Color SaddleBrown; 
        public static readonly Color RosyBrown;
        public static readonly Color PowderBlue; 
        public static readonly Color Plum; 
        public static readonly Color PapayaWhip; 
        public static readonly Color Orange;
        public static readonly Color Black = new Color(0xff, 0x00, 0x00, 0x00);
        public static readonly Color White = new Color(0xff, 0xff, 0xff, 0xff);
        public static readonly Color DarkCyan;
        public static readonly Color DarkMagenta;
        public static readonly Color LightGray;
        
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

        public static implicit operator ActualColor(Color color)
        {
            return global::Windows.UI.Color.FromArgb(color._a, color._r, color._g, color._b);
        }

        public static implicit operator Color(ActualColor color)
        {
            return FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion
    }
}