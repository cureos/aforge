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
			var writeableBitmap = BitmapFactory.New(1, 1);
#if NETFX_CORE
			return writeableBitmap.FromStream(stream).Result;
#else
			return writeableBitmap.FromStream(stream);
#endif
		}

		#endregion

		#region OPERATORS

		public static implicit operator WriteableBitmap(Bitmap bitmap)
		{
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				bitmap = bitmap.Clone(PixelFormat.Format32bppArgb);

			var bytes = new byte[bitmap._stride * bitmap._height];
			Marshal.Copy(bitmap._scan0, bytes, 0, bytes.Length);

			var writeableBitmap = BitmapFactory.New(bitmap._width, bitmap._height);
			return writeableBitmap.FromByteArray(bytes);
		}

		public static implicit operator Bitmap(WriteableBitmap writeableBitmap)
		{
#if !NETFX_CORE && !WINDOWS_PHONE
			if (writeableBitmap.Format != PixelFormats.Pbgra32)
				writeableBitmap = BitmapFactory.ConvertToPbgra32Format(writeableBitmap);
#endif
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
