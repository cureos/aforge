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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace System.Drawing
{
	public sealed partial class Bitmap
	{
		#region METHODS

		internal static Bitmap FromStream(Stream stream)
		{
		    Bitmap bitmap = null;

		    Task.Run(async () =>
		    {
		        using (var raStream = new InMemoryRandomAccessStream())
		        {
		            await stream.CopyToAsync(raStream.AsStream());
		            var decoder = await BitmapDecoder.CreateAsync(raStream);
		            var pixelData = await decoder.GetPixelDataAsync();

		            var width = (int)decoder.OrientedPixelWidth;
		            var height = (int)decoder.OrientedPixelHeight;
		            const PixelFormat format = PixelFormat.Format32bppArgb;
		            var bytes = pixelData.DetachPixelData();

                    bitmap = new Bitmap(width, height, format);
                    var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
                    Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
                    bitmap.UnlockBits(data);
                }
		    }).Wait();

		    return bitmap;
		}

		#endregion

		#region OPERATORS

	    public static explicit operator BitmapSource(Bitmap bitmap)
	    {
	        if (bitmap.PixelFormat != PixelFormat.Format32bppPArgb)
	            bitmap = bitmap.Clone(PixelFormat.Format32bppPArgb);

	        var bytes = new byte[bitmap._stride * bitmap._height];
	        Marshal.Copy(bitmap._scan0, bytes, 0, bytes.Length);

	        var bitmapImage = new WriteableBitmap(bitmap._width, bitmap._height);
	        bitmapImage.PixelBuffer.AsStream().Write(bytes, 0, bytes.Length);

	        return bitmapImage;
	    }

	    public static explicit operator Bitmap(WriteableBitmap writeableBitmap)
		{
			var width = writeableBitmap.PixelWidth;
			var height = writeableBitmap.PixelHeight;
			var bytes = writeableBitmap.PixelBuffer.ToArray();

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
