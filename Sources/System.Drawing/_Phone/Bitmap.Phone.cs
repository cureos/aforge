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

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
#endif

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
#if NETFX_CORE
			return writeableBitmap.FromStream(stream).Result;
#else
			return writeableBitmap;
#endif
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
			const PixelFormat format = PixelFormat.Format32bppPArgb;

		    var pixels = writeableBitmap.Pixels;
			var bytes = new byte[Buffer.ByteLength(pixels)];
		    Buffer.BlockCopy(pixels, 0, bytes, 0, bytes.Length);

			var bitmap = new Bitmap(width, height, format);
			var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
			Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
			bitmap.UnlockBits(data);

			return bitmap;
		}

		#endregion
	}
}
