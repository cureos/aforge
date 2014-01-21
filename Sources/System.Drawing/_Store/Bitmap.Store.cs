// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace System.Drawing
{
	public sealed partial class Bitmap
	{
		#region METHODS

		public static Bitmap FromStream(Stream stream)
		{
            throw new NotImplementedException();
/*            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsInputStream());
            bitmapImage.CreateOptions = BitmapCreateOptions.None;

            var writeableBitmap = new WriteableBitmap(bitmapImage);
            bitmapImage.UriSource = null;

            return writeableBitmap;*/
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
		    using (var stream = writeableBitmap.PixelBuffer.AsStream())
		    {
		        stream.Write(bytes, 0, bytes.Length);
		    }

			return writeableBitmap;
		}

		public static implicit operator Bitmap(WriteableBitmap writeableBitmap)
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
