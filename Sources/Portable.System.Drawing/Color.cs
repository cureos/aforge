namespace System.Drawing
{
    public struct Color
    {
        public bool Equals(Color other)
        {
            return _r == other._r && _g == other._g && _b == other._b && _a == other._a;
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
                int hashCode = _r.GetHashCode();
                hashCode = (hashCode * 397) ^ _g.GetHashCode();
                hashCode = (hashCode * 397) ^ _b.GetHashCode();
                hashCode = (hashCode * 397) ^ _a.GetHashCode();
                return hashCode;
            }
        }

        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;
        private readonly byte _a;

        public static Color Red; public static Color Green; public static Color Blue; public static Color Yellow; 
        public static Color Violet; public static Color Brown; public static Color Olive; public static Color Cyan; 
        public static Color Magenta; public static Color Gold; public static Color Indigo; public static Color Ivory; 
        public static Color HotPink; public static Color DarkRed; public static Color DarkGreen; public static Color DarkBlue; 
        public static Color DarkSeaGreen; public static Color Gray; public static Color DarkKhaki; public static Color DarkGray; 
        public static Color LimeGreen; public static Color Tomato; public static Color SteelBlue; public static Color SkyBlue; 
        public static Color Silver; public static Color Salmon; public static Color SaddleBrown; public static Color RosyBrown;
        public static Color PowderBlue; public static Color Plum; public static Color PapayaWhip; public static Color Orange; 
        public static Color Black;
        public static Color White;
        public static Color DarkCyan;
        public static Color DarkMagenta;
        public static Color LightGray;

        public Color(byte r, byte g, byte b, byte a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public static Color FromArgb(int r, int g, int b)
        {
            throw new NotImplementedException();
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

        public byte A
        {
            get { return _a; }
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Color lhs, Color rhs) { throw new NotImplementedException();}

        public static bool operator !=(Color lhs, Color rhs)
        {
            return !(lhs == rhs);
        }
    }
}