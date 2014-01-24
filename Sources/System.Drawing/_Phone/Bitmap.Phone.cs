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

using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace System.Drawing
{
	public sealed partial class Bitmap
	{
		#region METHODS

		public static Bitmap FromStream(Stream stream)
		{
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            bitmapImage.CreateOptions = BitmapCreateOptions.None;

            var writeableBitmap = new WriteableBitmap(bitmapImage);
            bitmapImage.UriSource = null;

            return writeableBitmap;
		}

		#endregion

		#region OPERATORS

		public static implicit operator WriteableBitmap(Bitmap bitmap)
		{
			if (bitmap.PixelFormat != PixelFormat.Format32bppPArgb)
				bitmap = bitmap.Clone(PixelFormat.Format32bppPArgb);

			var bytes = new byte[bitmap._stride * bitmap._height];
			Marshal.Copy(bitmap._scan0, bytes, 0, bytes.Length);

			var writeableBitmap = new WriteableBitmap(bitmap._width, bitmap._height);
		    Buffer.BlockCopy(bytes, 0, writeableBitmap.Pixels, 0, bytes.Length);

			return writeableBitmap;
		}

		public static implicit operator Bitmap(WriteableBitmap writeableBitmap)
		{
			var width = writeableBitmap.PixelWidth;
			var height = writeableBitmap.PixelHeight;

		    var pixels = writeableBitmap.Pixels;
			var bytes = new byte[Buffer.ByteLength(pixels)];
		    Buffer.BlockCopy(pixels, 0, bytes, 0, bytes.Length);

            // Remove alpha pre-multiplication.
		    unsafe
		    {
		        var length = bytes.Length;
		        fixed (byte* b0 = bytes)
		        {
		            var b = b0 + length;
		            for (var i = 0; i < length; i += 4)
		            {
		                byte alpha;
		                if ((alpha = *--b) == 0)
		                {
		                    b -= 3;
                            continue;
		                }
		                *--b = (byte)(255 * *b / alpha);
                        *--b = (byte)(255 * *b / alpha);
                        *--b = (byte)(255 * *b / alpha);
		            }
		        }
		    }

			const PixelFormat format = PixelFormat.Format32bppArgb;
			var bitmap = new Bitmap(width, height, format);
			var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
			Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
			bitmap.UnlockBits(data);

			return bitmap;
		}

		#endregion
	}
}
