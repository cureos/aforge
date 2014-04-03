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

namespace System.Drawing.Imaging
{
    public class BitmapData
    {
        #region FIELDS

        private readonly IntPtr _scan0;
        private readonly int _stride;
        private readonly int _width;
        private readonly int _height;
        private readonly PixelFormat _pixelFormat;
        
        #endregion

        #region CONSTRUCTORS

        public BitmapData(int width, int height, int stride, PixelFormat pixelFormat, IntPtr scan0)
        {
            _scan0 = scan0;
            _stride = stride;
            _width = width;
            _height = height;
            _pixelFormat = pixelFormat;
        }

        #endregion

        #region PROPERTIES

        public IntPtr Scan0
        {
            get { return _scan0; }
        }

        public int Stride
        {
            get { return _stride; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }
        
        #endregion
    }
}