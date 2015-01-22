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
    public struct Size
    {
        #region FIELDS

        private readonly int _width;
        private readonly int _height;
        
        #endregion

        #region CONSTRUCTORS

        public Size(int width, int height)
        {
            _width = width;
            _height = height;
        }
        
        #endregion

        #region PROPERTIES

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }
        
        #endregion
    }
}