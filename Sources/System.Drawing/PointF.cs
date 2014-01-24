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
	public struct PointF
	{
		#region FIELDS

		private float _x;
		private float _y;

		#endregion

		#region CONSTRUCTORS

		public PointF(float x, float y)
		{
			_x = x;
			_y = y;
		}

		#endregion

		#region PROPERTIES

		public float X
		{
			get { return _x; }
			set { _x = value; }
		}

		public float Y
		{
			get { return _y; }
			set { _y = value; }
		}

		#endregion
	}
}