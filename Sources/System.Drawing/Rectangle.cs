// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//
// Some code in this implementation has been adapted from the Mono implementation of the
// System.Drawing.Rectangle class:
// https://github.com/mono/mono/blob/master/mcs/class/System.Drawing/System.Drawing/Rectangle.cs

namespace System.Drawing
{
	public struct Rectangle : IEquatable<Rectangle>
	{
		#region FIELDS

		public static readonly Rectangle Empty;

		private int _x;
		private int _y;
		private int _width;
		private int _height;
		
		#endregion

		#region CONSTRUCTORS

		static Rectangle()
		{
			Empty = new Rectangle();
		}

		public Rectangle(int x, int y, int width, int height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}
		
		#endregion

		#region PROPERTIES

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public int Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public int Left
		{
			get { return _x; }
		}

		public int Top
		{
			get { return _y; }
		}

		public int Right
		{
			get { return _x + _width; }
		}

		public int Bottom
		{
			get { return _y + _height; }
		}

        public bool IsEmpty
        {
            get
            {
                return (_x == 0 && _y == 0 && _width == 0 && _height == 0);
            }
        }

		#endregion

		#region METHODS

		public bool Equals(Rectangle other)
		{
			return _x == other._x && _y == other._y && _width == other._width && _height == other._height;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Rectangle && Equals((Rectangle)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = _x;
				hashCode = (hashCode * 397) ^ _y;
				hashCode = (hashCode * 397) ^ _width;
				hashCode = (hashCode * 397) ^ _height;
				return hashCode;
			}
		}

		public void Intersect(Rectangle rect)
		{
			this = Intersect(this, rect);
		}

	    public bool IntersectsWith(Rectangle rect)
	    {
	        return !(Left >= rect.Right || Right <= rect.Left || Top >= rect.Bottom || Bottom <= rect.Top);
	    }

	    public bool Contains(int x, int y)
		{
			return ((x >= Left) && (x < Right) && (y >= Top) && (y < Bottom));
		}

        public bool Contains(Rectangle rect)
        {
            return (rect == Intersect(this, rect));
        }

		public static Rectangle Intersect(Rectangle a, Rectangle b)
		{
			if (!a.IntersectsWithInclusive(b)) return Empty;

			return FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right),
				Math.Min(a.Bottom, b.Bottom));
		}

		public void Inflate(int width, int height)
		{
			_x -= width;
			_y -= height;
			_width += width * 2;
			_height += height * 2;
		}

		private bool IntersectsWithInclusive(Rectangle r)
		{
			return !((Left > r.Right) || (Right < r.Left) ||
				(Top > r.Bottom) || (Bottom < r.Top));
		}

		internal static Rectangle FromLTRB(int left, int top, int right, int bottom)
		{
			return new Rectangle(left, top, right - left, bottom - top);
		}
		
		#endregion

		#region OPERATORS

		public static bool operator ==(Rectangle lhs, Rectangle rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Rectangle lhs, Rectangle rhs)
		{
			return !(lhs == rhs);
		}

		#endregion
	}
}