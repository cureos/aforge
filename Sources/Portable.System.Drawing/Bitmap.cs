using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    public class Bitmap : Image
    {
        private PixelFormat _pixelFormat;
        private int _width;
        private int _height;
        private ColorPalette _palette;
        private int _horizontalResolution;
        private int _verticalResolution;

        public Bitmap(int width, int height, PixelFormat pixelFormat)
        {
            _width = width;
            _height = height;
            _pixelFormat = pixelFormat;
        }

        public Bitmap(int width, int height, int stride, PixelFormat pixelFormat, IntPtr imageData)
        {
            throw new NotImplementedException();
        }

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set { _pixelFormat = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public ColorPalette Palette
        {
            get { return _palette; }
            set { _palette = value; }
        }

        public int HorizontalResolution
        {
            get { return _horizontalResolution; }
            set { _horizontalResolution = value; }
        }

        public int VerticalResolution
        {
            get { return _verticalResolution; }
            set { _verticalResolution = value; }
        }

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
            throw new NotImplementedException();
        }
    }
}
