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
	/// Sepia filter - brown photo
	/// </summary>
	///
	/// <remarks>The filter makes an image look like an old brown photo. The main
	/// idea of the algorithm:
	/// <list type="bullet">
	/// <item>transform to YIQ color space;</item>
	/// <item>modify it;</item>
	/// <item>transform back to RGB.</item>
	/// </list>
	/// <br />
	/// <b>1) RGB -> YIQ</b>:<br /><code>
	///	Y = 0.299 * R + 0.587 * G + 0.114 * B
	///	I = 0.596 * R - 0.274 * G - 0.322 * B
	///	Q = 0.212 * R - 0.523 * G + 0.311 * B
	///	</code><br />
	/// <b>2) update</b>:<br /><code>
	///	I = 51
	///	Q = 0
	///	</code><br />
	///	<b>3) YIQ -> RGB</b>:<br /><code>
	///	R = 1.0 * Y + 0.956 * I + 0.621 * Q
	///	G = 1.0 * Y - 0.272 * I - 0.647 * Q
	///	B = 1.0 * Y - 1.105 * I + 1.702 * Q
	///	</code><br />
	///	Below is the example of this filter application:
	/// <list type="bullet">
	/// <item><b>Source image</b><br /><img src="sample1.jpg" width="480" height="361" /></item>
	/// <item><b>Result image</b><br /><img src="sepia.jpg" width="480" height="361" /></item>
	/// </list>
	/// </remarks> 
	///
	public sealed class Sepia : IInPlaceFilter
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
			BitmapData srcData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// apply the filter
			Bitmap dstImage = Apply( srcData );

			// unlock source image
			image.UnlockBits( srcData );

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
			if ( imageData.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// get image dimension
			int width = imageData.Width;
			int height = imageData.Height;

			// create new image
			Bitmap dstImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// copy image
			Win32.memcpy( dstData.Scan0, imageData.Scan0, imageData.Stride * height );

			// process the filter
			ProcessFilter( dstData );

			// unlock both images
			dstImage.UnlockBits( dstData );

			return dstImage;
		}

		/// <summary>
		/// Apply filter to an image
		/// </summary>
		/// 
		/// <param name="image">Image to apply filter to</param>
		/// 
		/// <remarks>The method applies the filter directly to the provided
		/// image.</remarks>
		/// 
		public void ApplyInPlace( Bitmap image )
		{
			// lock source bitmap data
			BitmapData data = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// apply the filter
			ApplyInPlace( data );

			// unlock image
			image.UnlockBits( data );
		}

		/// <summary>
		/// Apply filter to an image
		/// </summary>
		/// 
		/// <param name="imageData">Image to apply filter to</param>
		/// 
		/// <remarks>The method applies the filter directly to the provided
		/// image data.</remarks>
		/// 
		public void ApplyInPlace( BitmapData imageData )
		{
			if ( imageData.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// process the filter
			ProcessFilter( imageData );
		}


		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">image data</param>
		/// 
		private unsafe void ProcessFilter( BitmapData imageData )
		{
			int width	= imageData.Width;
			int height	= imageData.Height;
			int offset	= imageData.Stride - width * 3;

			// do the job
			byte *ptr = (byte *) imageData.Scan0.ToPointer( );
			byte t;

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					t = (byte)( 0.299 * ptr[RGB.R] + 0.587 * ptr[RGB.G] + 0.114 * ptr[RGB.B] );

					// red
					ptr[RGB.R] = (byte)( ( t > 206 ) ? 255 : t + 49 );
					// green
					ptr[RGB.G] = (byte)( ( t < 14 ) ? 0 : t - 14 );
					// blue
					ptr[RGB.B] = (byte)( ( t < 56 ) ? 0 : t - 56 );
				}
				ptr += offset;
			}
		}
	}
}
