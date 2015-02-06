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

namespace System.Drawing
{
	internal sealed class Pen : IDisposable
	{
		#region FIELDS

		private readonly Color color;

	    private readonly float width;

	    #endregion
		
		#region CONSTRUCTORS

		public Pen(Color color)
		{
			this.color = color;
		}

	    public Pen(Color color, float width)
	    {
	        this.color = color;
	        this.width = width;
	    }

	    #endregion

		#region PROPERTIES

		public Color Color
		{
			get { return this.color; }
		}

	    public float Width
	    {
	        get
	        {
	            return this.width;
	        }
	    }

	    #endregion		

		#region METHODS

		public void Dispose()
		{
		}
		
		#endregion
	}
}