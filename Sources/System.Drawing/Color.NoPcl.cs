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

#if NETFX_CORE
using TrueColor = Windows.UI.Color;
#else
using TrueColor = System.Windows.Media.Color;
#endif

namespace System.Drawing
{
	public partial struct Color
	{

		public static implicit operator TrueColor(Color color)
		{
			return TrueColor.FromArgb(color._a, color._r, color._g, color._b);
		}

		public static implicit operator Color(TrueColor trueColor)
		{
			return FromArgb(trueColor.A, trueColor.R, trueColor.G, trueColor.B);
		}
	}
}