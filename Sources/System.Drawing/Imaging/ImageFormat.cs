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
    public sealed class ImageFormat
    {
        #region FIELDS

        private readonly string _name;

        private static readonly ImageFormat _bmp;
        private static readonly ImageFormat _gif;
        private static readonly ImageFormat _icon;
        private static readonly ImageFormat _jpeg;
        private static readonly ImageFormat _png;
        private static readonly ImageFormat _tiff;
        private static readonly ImageFormat _wmf;
        private static readonly ImageFormat _emf;
        #endregion

        #region CONSTRUCTORS

        private ImageFormat(string name)
        {
            this._name = name;
        }

        static ImageFormat()
        {
            _bmp = new ImageFormat("BMP");
            _gif = new ImageFormat("GIF");
            _icon = new ImageFormat("ICON");
            _jpeg = new ImageFormat("JPEG");
            _png = new ImageFormat("PNG");
            _tiff = new ImageFormat("TIFF");
            _wmf = new ImageFormat("WMF");
            _emf = new ImageFormat("EMF");
        }

        #endregion

        #region PROPERTIES

        public static ImageFormat Bmp
        {
            get
            {
                return _bmp;
            }
        }

        public static ImageFormat Gif
        {
            get
            {
                return _gif;
            }
        }

        public static ImageFormat Icon
        {
            get
            {
                return _icon;
            }
        }

        public static ImageFormat Jpeg
        {
            get
            {
                return _jpeg;
            }
        }

        public static ImageFormat Png
        {
            get
            {
                return _png;
            }
        }

        public static ImageFormat Tiff
        {
            get
            {
                return _tiff;
            }
        }

        public static ImageFormat Wmf
        {
            get
            {
                return _wmf;
            }
        }

        public static ImageFormat Emf
        {
            get
            {
                return _emf;
            }
        }

        #endregion

        #region METHODS

        public override bool Equals(object obj)
        {
            return obj is ImageFormat && this._name == ((ImageFormat)obj)._name;
        }

        public override int GetHashCode()
        {
            return this._name.GetHashCode();
        }

        #endregion
    }
}