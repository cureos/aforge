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
using System.Runtime.InteropServices;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace System.Drawing
{
    public sealed class Bitmap : IDisposable
    {
        #region FIELDS

        private bool _disposed = false;

        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;
        private readonly PixelFormat _pixelFormat;

        private readonly IntPtr _scan0;
        private readonly bool _freeScan0;

        #endregion

        #region CONSTRUCTORS

        public Bitmap(int width, int height, PixelFormat pixelFormat)
        {
            _width = width;
            _height = height;
            _stride = GetStride(width, pixelFormat);
            _pixelFormat = pixelFormat;

            _scan0 = Marshal.AllocHGlobal(_stride * height);
            _freeScan0 = true;
        }

        public Bitmap(int width, int height, int stride, PixelFormat pixelFormat, IntPtr scan0)
        {
            _width = width;
            _height = height;
            _stride = stride;
            _pixelFormat = pixelFormat;

            _scan0 = scan0;
            _freeScan0 = false;
        }

        ~Bitmap()
        {
            Dispose(false);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UnlockBits(BitmapData sourceData)
        {
            // TODO Need to do anything here?
        }

        public BitmapData LockBits(Rectangle rectangle, ImageLockMode readOnly, PixelFormat pixelFormat)
        {
            if (pixelFormat.Equals(PixelFormat.Indexed) || pixelFormat.Equals(PixelFormat.Undefined))
                throw new ArgumentException("LockBits method only applicable to pixel formats with prefix Format",
                    "pixelFormat");
            if (!pixelFormat.Equals(_pixelFormat))
                throw new ArgumentException(String.Format("Bitmap.PixelFormat = {0}", _pixelFormat), "pixelFormat");

            return new BitmapData(_width, _height, _stride, _pixelFormat, _scan0);
        }

        public static Bitmap FromStream(Stream stream)
        {
            var writeableBitmap = BitmapFactory.New(1, 1);
            return writeableBitmap.FromStream(stream).Result;
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

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free managed disposable objects
            }

            // Free unmanaged objects
            if (_freeScan0) Marshal.FreeHGlobal(_scan0);

            _disposed = true;
        }

        private static int GetStride(int width, PixelFormat format)
        {
            var bitsPerPixel = ((int)format & 0xff00) >> 8;
            var bytesPerPixel = (bitsPerPixel + 7) / 8;
            return 4 * ((width * bytesPerPixel + 3) / 4);
        }

        #endregion

        #region OPERATORS

        public static implicit operator WriteableBitmap(Bitmap bitmap)
        {
            var bytes = new byte[bitmap._stride * bitmap._height];
            Marshal.Copy(bitmap._scan0, bytes, 0, bytes.Length);

            var writeableBitmap = BitmapFactory.New(bitmap._width, bitmap._height);
            return writeableBitmap.FromByteArray(bytes);
        }

        public static implicit operator Bitmap(WriteableBitmap writeableBitmap)
        {
            var width = writeableBitmap.PixelWidth;
            var height = writeableBitmap.PixelHeight;
            const PixelFormat format = PixelFormat.Format32bppArgb;
            var bytes = writeableBitmap.ToByteArray();

            var bitmap = new Bitmap(width, height, format);
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        #endregion
    }
}
