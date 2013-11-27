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
using System.Runtime.InteropServices;
using AForge;

namespace System.Drawing
{
    public sealed class Graphics : IDisposable
    {
        #region FIELDS

        private bool _disposed = false;

        private Bitmap _bitmap;

        #endregion

        #region CONSTRUCTORS

        private Graphics(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        ~Graphics()
        {
            Dispose(false);
        }

        #endregion

        #region METHODS

        public static Graphics FromImage(Bitmap bitmap)
        {
            return new Graphics(bitmap);
        }

        public void DrawImage(Bitmap source, int x, int y, int width, int height)
        {
            var sourceData = source.LockBits(new Rectangle(x, y, width, height), ImageLockMode.ReadOnly,
                source.PixelFormat);
            var bitmapData = _bitmap.LockBits(new Rectangle(x, y, width, height), ImageLockMode.ReadWrite,
                _bitmap.PixelFormat);

            // TODO Obtain pixels from source and draw them onto _bitmap in _bitmap pixel format
            SystemTools.CopyUnmanagedMemory(bitmapData.Scan0, sourceData.Scan0,
                Math.Min(bitmapData.Stride * bitmapData.Height, sourceData.Stride * sourceData.Height));

            _bitmap.UnlockBits(bitmapData);
            source.UnlockBits(sourceData);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free managed resources
                _bitmap = null;
            }

            // Free unmanaged resources

            _disposed = true;
        }

        #endregion
    }
}