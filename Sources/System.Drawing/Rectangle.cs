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

// Some code in this implementation has been adapted from the Mono implementation of the
// System.Drawing.Rectangle class:
// https://github.com/mono/mono/blob/master/mcs/class/System.Drawing/System.Drawing/Rectangle.cs

namespace System.Drawing
{
	public struct Rectangle : IEquatable<Rectangle>
	{
		#region FIELDS

		internal static readonly Rectangle Empty;

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
			internal set { _x = value; }
		}

        public int Y
		{
			get { return _y; }
			internal set { _y = value; }
		}

        public int Width
		{
			get { return _width; }
			internal set { _width = value; }
		}

        public int Height
		{
			get { return _height; }
			internal set { _height = value; }
		}

        internal int Left
		{
			get { return _x; }
		}

        internal int Top
		{
			get { return _y; }
		}

        internal int Right
		{
			get { return _x + _width; }
		}

        internal int Bottom
		{
			get { return _y + _height; }
		}

        internal bool IsEmpty
        {
            get
            {
                return (_x == 0 && _y == 0 && _width == 0 && _height == 0);
            }
        }

        internal Point Location
        {
            get
            {
                return new Point(_x, _y);
            }
            set
            {
                _x = value.X;
                _y = value.Y;
            }
        }

        internal Size Size
        {
            get
            {
                return new Size(_width, _height);
            }
            set
            {
                _width = value.Width;
                _height = value.Height;
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

        internal void Intersect(Rectangle rect)
		{
			this = Intersect(this, rect);
		}

        internal bool IntersectsWith(Rectangle rect)
	    {
	        return !(Left >= rect.Right || Right <= rect.Left || Top >= rect.Bottom || Bottom <= rect.Top);
	    }

        internal bool Contains(int x, int y)
		{
			return ((x >= Left) && (x < Right) && (y >= Top) && (y < Bottom));
		}

        internal bool Contains(Rectangle rect)
        {
            return (rect == Intersect(this, rect));
        }

        internal static Rectangle Intersect(Rectangle a, Rectangle b)
		{
			if (!a.IntersectsWithInclusive(b)) return Empty;

			return FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right),
				Math.Min(a.Bottom, b.Bottom));
		}

        internal void Inflate(int width, int height)
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