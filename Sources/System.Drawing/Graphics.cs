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

// DrawLine, DrawRectangle and DrawEllipseCentered methods are adapted from WriteableBitmapEx,
// Copyright © 2009-2012 Rene Schulte and WriteableBitmapEx Contributors
 

using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using ImagePixelEnumerator.Extensions;
using ImagePixelEnumerator.Helpers;
using ImagePixelEnumerator.Quantizers;
using ImagePixelEnumerator.Quantizers.DistinctCompetition;

namespace System.Drawing
{
    internal sealed class Graphics : IDisposable
    {
        #region FIELDS

        private static readonly int ParallelTaskCount;

        private bool _disposed = false;
        private readonly object _locker = new object();

        private Image _bitmap;
        private ImageBuffer _dummyImageBuffer;
        private readonly IColorQuantizer _quantizer;

        // TODO Matrix needs to be applied onto the image.
        private readonly Matrix matrix;

        #endregion

        #region CONSTRUCTORS

        static Graphics()
        {
            ParallelTaskCount = Environment.ProcessorCount;
        }

        private Graphics(Image bitmap)
        {
            _bitmap = bitmap;
            _quantizer = new DistinctSelectionQuantizer();
            this.matrix = new Matrix();
        }

        ~Graphics()
        {
            Dispose(false);
        }

        #endregion

        #region METHODS

        internal static Graphics FromImage(Image bitmap)
        {
            return new Graphics(bitmap);
        }

        internal void DrawImage(Image source, int x, int y, int width, int height)
        {
            var targetFormat = _bitmap.PixelFormat;
            List<Color> palette = null;

            // indexed formats require 2 passes - one more pass to determines colors for palette beforehand
            if (targetFormat.IsIndexed())
            {
                _quantizer.Prepare(source);

                // Pass: scan
                ImageBuffer.ProcessPerPixel(source, null, ParallelTaskCount, (passIndex, pixel) =>
                {
                    var color = pixel.GetColor();
                    _quantizer.AddColor(color, pixel.X, pixel.Y);
                    return false;
                });

                // determines palette
                palette = _quantizer.GetPalette(targetFormat.GetColorCount());
            }

            // TODO Handle scenario when source image should NOT be drawn one-on-one onto the target image

            // Pass: apply
            ImageBuffer.TransformImagePerPixel(source, palette, ref _bitmap, null, ParallelTaskCount,
                (passIndex, sourcePixel, targetPixel) =>
                {
                    var color = sourcePixel.GetColor();
                    targetPixel.SetColor(color, _quantizer);
                    return true;
                });
        }

        internal void DrawImageUnscaled(Image image, int x, int y)
        {
        }

        internal void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            var ellipseIndices = DrawEllipseCentered(_bitmap.Width, _bitmap.Height, x + width / 2, y + height / 2,
                width / 2, height / 2);
            ApplyColorToPixelIndices(ellipseIndices, pen.Color);
        }

        internal void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            DrawEllipse(pen, (int)x, (int)y, (int)width, (int)height);
        }

        internal void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            var lineIndices = DrawLine(_bitmap.Width, _bitmap.Height, x1, y1, x2, y2);
            ApplyColorToPixelIndices(lineIndices, pen.Color);
        }

        internal void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        internal void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            DrawLine(pen, (int)Math.Round(x1), (int)Math.Round(y1), (int)Math.Round(x2), (int)Math.Round(y2));
        }

        internal void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, (int)Math.Round(pt1.X), (int)Math.Round(pt1.Y), (int)Math.Round(pt2.X),
                (int)Math.Round(pt2.Y));
        }

        internal void DrawCurve(Pen pen, Point[] points)
        {
            var curveIndices = new List<int>();

            var x1 = points[0].X;
            var y1 = points[0].Y;

            for (var i = 1; i < points.Length; ++i)
            {
                var x2 = points[i].X;
                var y2 = points[i].Y;

                curveIndices.AddRange(DrawLine(_bitmap.Width, _bitmap.Height, x1, y1, x2, y2));
                x1 = x2;
                y1 = y2;
            }

            ApplyColorToPixelIndices(curveIndices, pen.Color);
        }

        internal void DrawCurve(Pen pen, PointF[] points)
        {
            var curveIndices = new List<int>();

            var x1 = (int)Math.Round(points[0].X);
            var y1 = (int)Math.Round(points[0].Y);

            for (var i = 1; i < points.Length; ++i)
            {
                var x2 = (int)Math.Round(points[i].X);
                var y2 = (int)Math.Round(points[i].Y);

                curveIndices.AddRange(DrawLine(_bitmap.Width, _bitmap.Height, x1, y1, x2, y2));
                x1 = x2;
                y1 = y2;
            }

            ApplyColorToPixelIndices(curveIndices, pen.Color);
        }

        internal void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            var rectangleIndices = DrawRectangle(_bitmap.Width, _bitmap.Height, (int)Math.Round(x), (int)Math.Round(y),
                (int)Math.Round(x + width) - 1, (int)Math.Round(y + height) - 1);
            ApplyColorToPixelIndices(rectangleIndices, pen.Color);
        }

        internal void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            throw new NotImplementedException();
        }

        internal void FillRectangle(Brush brush, RectangleF rect)
        {
            if (!(brush is SolidBrush))
            {
                throw new NotSupportedException("Only solid brushes are supported.");
            }
            var color = ((SolidBrush)brush).Color;

            var x0 = (int)Math.Round(rect.X);
            var y0 = (int)Math.Round(rect.Y);
            var x1 = x0 + (int)Math.Round(rect.Width) - 1;
            var y1 = y0 + (int)Math.Round(rect.Height) - 1;

            ImageBuffer.ProcessPerPixel(_bitmap, null, ParallelTaskCount, (passIndex, pixel) =>
            {
                var x = pixel.X;
                var y = pixel.Y;
                if (x0 <= x && x <= x1 && y0 <= y && y <= y1) pixel.SetColor(color, _quantizer);
                return true;
            });
        }

        internal void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize,
            CopyPixelOperation copyPixelOperation)
        {
            throw new NotImplementedException();
        }

        internal void ResetTransform()
        {
            this.matrix.Reset();
        }

        internal void RotateTransform(float angle)
        {
            this.matrix.Rotate(angle);
        }

        internal void TranslateTransform(float dx, float dy)
        {
            this.matrix.Translate(dx, dy);
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

        /// <summary>
        /// Defines a line in 2D array by connecting two points using an optimized DDA. 
        /// </summary>
        /// <param name="width">The width of one scanline in the pixels array.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        private static List<int> DrawLine(int width, int height, int x1, int y1, int x2, int y2)
        {
            var lineIndices = new List<int>();

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            const int PRECISION_SHIFT = 8;

            // Determine slope (absoulte value)
            int lenX, lenY;
            if (dy >= 0)
            {
                lenY = dy;
            }
            else
            {
                lenY = -dy;
            }

            if (dx >= 0)
            {
                lenX = dx;
            }
            else
            {
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                if (dx < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / dx;

                int y1s = y1 << PRECISION_SHIFT;
                int y2s = y2 << PRECISION_SHIFT;
                int hs = height << PRECISION_SHIFT;

                if (y1 < y2)
                {
                    if (y1 >= height || y2 < 0)
                    {
                        return lineIndices;
                    }
                    if (y1s < 0)
                    {
                        if (incy == 0)
                        {
                            return lineIndices;
                        }
                        int oldy1s = y1s;
                        // Find lowest y1s that is greater or equal than 0.
                        y1s = incy - 1 + ((y1s + 1) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s >= hs)
                    {
                        if (incy != 0)
                        {
                            // Find highest y2s that is less or equal than ws - 1.
                            // y2s = y1s + n * incy. Find n.
                            y2s = hs - 1 - (hs - 1 - y1s) % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }
                else
                {
                    if (y2 >= height || y1 < 0)
                    {
                        return lineIndices;
                    }
                    if (y1s >= hs)
                    {
                        if (incy == 0)
                        {
                            return lineIndices;
                        }
                        int oldy1s = y1s;
                        // Find highest y1s that is less or equal than ws - 1.
                        // y1s = oldy1s + n * incy. Find n.
                        y1s = hs - 1 + (incy - (hs - 1 - oldy1s) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s < 0)
                    {
                        if (incy != 0)
                        {
                            // Find lowest y2s that is greater or equal than 0.
                            // y2s = y1s + n * incy. Find n.
                            y2s = y1s % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }

                if (x1 < 0)
                {
                    y1s -= incy * x1;
                    x1 = 0;
                }
                if (x2 >= width)
                {
                    x2 = width - 1;
                }

                int ys = y1s;

                // Walk the line!
                int y = ys >> PRECISION_SHIFT;
                int previousY = y;
                int index = x1 + y * width;
                int k = incy < 0 ? 1 - width : 1 + width;
                for (int x = x1; x <= x2; ++x)
                {
                    lineIndices.Add(index);
                    ys += incy;
                    y = ys >> PRECISION_SHIFT;
                    if (y != previousY)
                    {
                        previousY = y;
                        index += k;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            else
            {
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return lineIndices;
                }
                if (dy < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int x1s = x1 << PRECISION_SHIFT;
                int x2s = x2 << PRECISION_SHIFT;
                int ws = width << PRECISION_SHIFT;

                int incx = (dx << PRECISION_SHIFT) / dy;

                if (x1 < x2)
                {
                    if (x1 >= width || x2 < 0)
                    {
                        return lineIndices;
                    }
                    if (x1s < 0)
                    {
                        if (incx == 0)
                        {
                            return lineIndices;
                        }
                        int oldx1s = x1s;
                        // Find lowest x1s that is greater or equal than 0.
                        x1s = incx - 1 + ((x1s + 1) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s >= ws)
                    {
                        if (incx != 0)
                        {
                            // Find highest x2s that is less or equal than ws - 1.
                            // x2s = x1s + n * incx. Find n.
                            x2s = ws - 1 - (ws - 1 - x1s) % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }
                else
                {
                    if (x2 >= width || x1 < 0)
                    {
                        return lineIndices;
                    }
                    if (x1s >= ws)
                    {
                        if (incx == 0)
                        {
                            return lineIndices;
                        }
                        int oldx1s = x1s;
                        // Find highest x1s that is less or equal than ws - 1.
                        // x1s = oldx1s + n * incx. Find n.
                        x1s = ws - 1 + (incx - (ws - 1 - oldx1s) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s < 0)
                    {
                        if (incx != 0)
                        {
                            // Find lowest x2s that is greater or equal than 0.
                            // x2s = x1s + n * incx. Find n.
                            x2s = x1s % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }

                if (y1 < 0)
                {
                    x1s -= incx * y1;
                    y1 = 0;
                }
                if (y2 >= height)
                {
                    y2 = height - 1;
                }

                int index = x1s + ((y1 * width) << PRECISION_SHIFT);

                // Walk the line!
                var inc = (width << PRECISION_SHIFT) + incx;
                for (int y = y1; y <= y2; ++y)
                {
                    lineIndices.Add(index >> PRECISION_SHIFT);
                    index += inc;
                }
            }

            return lineIndices;
        }

        /// <summary>
        /// Draws a rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="w">Width of the bitmap.</param>
        /// <param name="h">Height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        private static List<int> DrawRectangle(int w, int h, int x1, int y1, int x2, int y2)
        {
            var indices = new List<int>();

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
                || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return indices;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 >= w) { x1 = w - 1; }
            if (y1 >= h) { y1 = h - 1; }
            if (x2 >= w) { x2 = w - 1; }
            if (y2 >= h) { y2 = h - 1; }

            var startY = y1 * w;
            var endY = y2 * w;

            var offset2 = endY + x1;
            var endOffset = startY + x2;
            var startYPlusX1 = startY + x1;

            // top and bottom horizontal scanlines
            for (var x = startYPlusX1; x <= endOffset; x++)
            {
                indices.Add(x); // top horizontal line
                indices.Add(offset2); // bottom horizontal line
                offset2++;
            }

            // vertical scanlines
            endOffset = startYPlusX1 + w;
            offset2 -= w;

            for (var y = startY + x2 + w; y <= offset2; y += w)
            {
                indices.Add(y); // right vertical line
                indices.Add(endOffset); // left vertical line
                endOffset += w;
            }

            return indices;
        }

        /// <summary>
        /// A Fast Bresenham Type Algorithm For Drawing Ellipses http://homepage.smc.edu/kennedy_john/belipse.pdf 
        /// Uses a different parameter representation than DrawEllipse().
        /// </summary>
        /// <param name="width">The width of one scanline in the pixels array.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="xc">The x-coordinate of the ellipses center.</param>
        /// <param name="yc">The y-coordinate of the ellipses center.</param>
        /// <param name="xr">The radius of the ellipse in x-direction.</param>
        /// <param name="yr">The radius of the ellipse in y-direction.</param>
        public static List<int> DrawEllipseCentered(int width, int height, int xc, int yc, int xr, int yr)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var ellipsePixels = new List<int>();

            // Avoid endless loop
            if (xr < 1 || yr < 1)
            {
                return ellipsePixels;
            }

            // Init vars
            int uh, lh, uy, ly, lx, rx;
            int x = xr;
            int y = 0;
            int xrSqTwo = (xr * xr) << 1;
            int yrSqTwo = (yr * yr) << 1;
            int xChg = yr * yr * (1 - (xr << 1));
            int yChg = xr * xr;
            int err = 0;
            int xStopping = yrSqTwo * xr;
            int yStopping = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                uy = yc + y; // Upper half
                ly = yc - y; // Lower half
                if (uy < 0) uy = 0; // Clip
                if (uy >= height) uy = height - 1; // ...
                if (ly < 0) ly = 0;
                if (ly >= height) ly = height - 1;
                uh = uy * width; // Upper half
                lh = ly * width; // Lower half

                rx = xc + x;
                lx = xc - x;
                if (rx < 0) rx = 0; // Clip
                if (rx >= width) rx = width - 1; // ...
                if (lx < 0) lx = 0;
                if (lx >= width) lx = width - 1;
                ellipsePixels.Add(rx + uh); // Quadrant I (Actually an octant)
                ellipsePixels.Add(lx + uh); // Quadrant II
                ellipsePixels.Add(lx + lh); // Quadrant III
                ellipsePixels.Add(rx + lh); // Quadrant IV

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo;
                if ((xChg + (err << 1)) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;
            uy = yc + y; // Upper half
            ly = yc - y; // Lower half
            if (uy < 0) uy = 0; // Clip
            if (uy >= height) uy = height - 1; // ...
            if (ly < 0) ly = 0;
            if (ly >= height) ly = height - 1;
            uh = uy * width; // Upper half
            lh = ly * width; // Lower half
            xChg = yr * yr;
            yChg = xr * xr * (1 - (yr << 1));
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = xc + x;
                lx = xc - x;
                if (rx < 0) rx = 0; // Clip
                if (rx >= width) rx = width - 1; // ...
                if (lx < 0) lx = 0;
                if (lx >= width) lx = width - 1;
                ellipsePixels.Add(rx + uh); // Quadrant I (Actually an octant)
                ellipsePixels.Add(lx + uh); // Quadrant II
                ellipsePixels.Add(lx + lh); // Quadrant III
                ellipsePixels.Add(rx + lh); // Quadrant IV

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo;
                if ((yChg + (err << 1)) > 0)
                {
                    y--;
                    uy = yc + y; // Upper half
                    ly = yc - y; // Lower half
                    if (uy < 0) uy = 0; // Clip
                    if (uy >= height) uy = height - 1; // ...
                    if (ly < 0) ly = 0;
                    if (ly >= height) ly = height - 1;
                    uh = uy * width; // Upper half
                    lh = ly * width; // Lower half
                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }

            return ellipsePixels;
        }

        private void ApplyColorToPixelIndices(IEnumerable<int> indices, Color color)
        {
            var pixels = indices.Select(idx => new Point(idx % _bitmap.Width, idx / _bitmap.Width)).ToList();

            lock (_locker)
            {
                if (_dummyImageBuffer == null)
                    _dummyImageBuffer = new ImageBuffer(new Bitmap(_bitmap.Width, _bitmap.Height, _bitmap.PixelFormat),
                        ImageLockMode.ReadOnly);

                _dummyImageBuffer.TransformPerPixel(null, ref _bitmap,
                    pixels, ParallelTaskCount,
                    (passIndex, sourcePixel, targetPixel) =>
                    {
                        targetPixel.SetColor(color, _quantizer);
                        return true;
                    });
            }
        }

        #endregion

        #region INNER TYPES

        private class Matrix
        {
            #region FIELDS

            private readonly float[,] values;

            #endregion

            #region CONSTRUCTORS

            internal Matrix()
            {
                values = new float[3, 3];
                values[0, 0] = values[1, 1] = values[2, 2] = 1.0f;
            }

            #endregion

            #region METHODS

            internal void Reset()
            {
                values.Initialize();
                values[0, 0] = values[1, 1] = values[2, 2] = 1.0f;
            }

            internal void Rotate(float angle)
            {
                var radians = angle * Math.PI / 180.0;
                var cos = (float)Math.Cos(radians);
                var sin = (float)Math.Sin(radians);

                var matrix = new float[3, 3];
                matrix[0, 0] = matrix[1, 1] = cos;
                matrix[0, 1] = sin;
                matrix[1, 1] = -sin;
                matrix[2, 2] = 1.0f;

                Transform(matrix);
            }

            internal void Translate(float dx, float dy)
            {
                var matrix = new float[3, 3];
                matrix[0, 0] = matrix[1, 1] = matrix[2, 2] = 1.0f;
                matrix[2, 0] = dx;
                matrix[2, 1] = dy;

                Transform(matrix);
            }

            private void Transform(float[,] matrix)
            {
                var tmp = new float[3, 3];
                for (var i = 0; i < 3; ++i)
                {
                    for (var j = 0; j < 3; ++j)
                    {
                        for (var k = 0; k < 3; ++k)
                        {
                            tmp[i, j] += matrix[i, k] * values[k, j];
                        }
                    }
                }

                Array.Copy(tmp, values, 9);
            }

            #endregion
        }

        #endregion
    }
}