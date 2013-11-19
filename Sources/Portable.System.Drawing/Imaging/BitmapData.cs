// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

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

        internal BitmapData(IntPtr scan0, int stride, int width, int height, PixelFormat pixelFormat)
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