// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    public class Bitmap : Image
    {
        #region FIELDS

        private readonly PixelFormat _pixelFormat;
        private readonly IntPtr _imageData;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;

        #endregion

        #region CONSTRUCTORS

        public Bitmap(int width, int height, PixelFormat pixelFormat)
        {
            _width = width;
            _height = height;
            _pixelFormat = pixelFormat;
        }

        public Bitmap(int width, int height, int stride, PixelFormat pixelFormat, IntPtr imageData)
        {
            _width = width;
            _height = height;
            _stride = stride;
            _pixelFormat = pixelFormat;
            _imageData = imageData;
        }

        #endregion

        #region PROPERTIES

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public ColorPalette Palette { get; set; }

        public int HorizontalResolution { get; private set; }

        public int VerticalResolution { get; private set; }
        
        #endregion

        #region METHODS

        public void UnlockBits(BitmapData sourceData)
        {
            throw new NotImplementedException();
        }

        public BitmapData LockBits(Rectangle rectangle, object readOnly, PixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }

        public static Bitmap FromStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void SetResolution(int horizontalResolution, int verticalResolution)
        {
            HorizontalResolution = horizontalResolution;
            VerticalResolution = verticalResolution;
        }
        
        #endregion
    }
}
