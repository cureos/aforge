// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;
using ImagePixelEnumerator.Extensions;
using ImagePixelEnumerator.Helpers;
using ImagePixelEnumerator.Quantizers;
using ImagePixelEnumerator.Quantizers.DistinctCompetition;

namespace System.Drawing
{
    public sealed class Graphics : IDisposable
    {
        #region FIELDS

        private static readonly IColorQuantizer Quantizer;

        private bool _disposed = false;

        private Image _bitmap;

        #endregion

        #region CONSTRUCTORS

        static Graphics()
        {
            Quantizer = new DistinctSelectionQuantizer();
        }

        private Graphics(Image bitmap)
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
            var targetFormat = _bitmap.PixelFormat;
            List<Color> palette = null;

            // indexed formats require 2 passes - one more pass to determines colors for palette beforehand
            if (targetFormat.IsIndexed())
            {
                Quantizer.Prepare(source);

                // Pass: scan
                ImageBuffer.ProcessPerPixel(source, null, 4, (passIndex, pixel) =>
                {
                    var color = pixel.GetColor();
                    Quantizer.AddColor(color, pixel.X, pixel.Y);
                    return true;
                });

                // determines palette
                palette = Quantizer.GetPalette(targetFormat.GetColorCount());
            }

            // Pass: apply
            ImageBuffer.TransformImagePerPixel(source, palette, ref _bitmap, null, 4, (passIndex, sourcePixel, targetPixel) =>
            {
                var color = sourcePixel.GetColor();
                targetPixel.SetColor(color, Quantizer);
                return true;
            });
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