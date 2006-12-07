// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Convert grayscale image to RGB
	/// </summary>
	/// 
	/// <remarks>The filter creates color image from specified grayscale image
	/// initializing all RGB channels to the same value - pixel's intensity.</remarks>
	/// 
	public sealed class GrayscaleToRGB : IFilter
	{
		/// <summary>
		/// Apply filter to an image
		/// </summary>
		/// 
		/// <param name="image">Source image to apply filter to</param>
		/// 
		/// <returns>Returns filter's result obtained by applying the filter to
		/// the source image</returns>
		/// 
		/// <remarks>The method keeps the source image unchanged and returns the
		/// the result of image processing filter as new image.</remarks> 
		///
		public Bitmap Apply( Bitmap image )
		{
			// lock source bitmap data
			BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			// apply the filter
			Bitmap dstImage = Apply( imageData );

			// unlock source image
			image.UnlockBits( imageData );

			return dstImage;
		}

		/// <summary>
		/// Apply filter to an image
		/// </summary>
		/// 
		/// <param name="imageData">Source image to apply filter to</param>
		/// 
		/// <returns>Returns filter's result obtained by applying the filter to
		/// the source image</returns>
		/// 
		/// <remarks>The filter accepts birmap data as input and returns the result
		/// of image processing filter as new image. The source image data are kept
		/// unchanged.</remarks>
		/// 
		public Bitmap Apply( BitmapData imageData )
		{
			if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get source image size
			int width = imageData.Width;
			int height = imageData.Height;

			// create new RGB image
			Bitmap dstImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			int srcOffset = imageData.Stride - width;
			int dstOffset = dstData.Stride - width * 3;

			// do the job
			unsafe
			{
				byte * src = (byte *) imageData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				// for each row
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, src++, dst += 3 )
					{
						dst[RGB.R] = dst[RGB.G] = dst[RGB.B] = *src;
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			// unlock destination images
			dstImage.UnlockBits( dstData );

			return dstImage;
		}
	}
}
