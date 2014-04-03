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

namespace System.Drawing
{
    public struct Point
    {
        #region FIELDS

        private readonly int _x;
        private readonly int _y;

        #endregion

        #region CONSTRUCTORS

        internal Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        #endregion

        #region PROPERTIES

        internal int X
        {
            get { return _x; }
        }

        internal int Y
        {
            get { return _y; }
        }

        #endregion

		#region METHODS

        internal static Point Round(PointF value)
	    {
		    int x, y;
		    checked
		    {
			    x = (int)Math.Round(value.X);
			    y = (int)Math.Round(value.Y);
		    }

		    return new Point(x, y);
	    }

	    #endregion
    }
}