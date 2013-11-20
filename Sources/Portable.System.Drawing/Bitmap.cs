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
using Windows.Graphics.Imaging;
#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace System.Drawing
{
    public class Bitmap : IDisposable
    {
        #region FIELDS

        private readonly WriteableBitmap _self;

        private readonly PixelFormat _pixelFormat;

        #endregion

        #region CONSTRUCTORS

        public Bitmap(int width, int height, PixelFormat pixelFormat)
        {
            _self = BitmapFactory.New(width, height);
            _pixelFormat = pixelFormat;
        }

        public Bitmap(int width, int height, int stride, PixelFormat pixelFormat, IntPtr imageData)
        {
            _self = BitmapFactory.New(width, height);
            _pixelFormat = pixelFormat;
        }

        #endregion

        #region PROPERTIES

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        public int Width
        {
            get { return _self.PixelWidth; }
        }

        public int Height
        {
            get { return _self.PixelHeight; }
        }

        public ColorPalette Palette { get; set; }

        public int HorizontalResolution { get; private set; }

        public int VerticalResolution { get; private set; }
        
        #endregion

        #region METHODS

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void UnlockBits(BitmapData sourceData)
        {
            throw new NotImplementedException();
        }

        public BitmapData LockBits(Rectangle rectangle, ImageLockMode readOnly, PixelFormat pixelFormat)
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

        public static int GetPixelFormatSize(PixelFormat pixelFormat)
        {
            return Image.GetPixelFormatSize(pixelFormat);
        }
        
        #endregion
    }
}
