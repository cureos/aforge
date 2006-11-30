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
	/// Base class for image grayscaling
	/// </summary>
	/// 
	/// <remarks>This class is the base class for image grayscaling. Other
	/// classes should inherit from this class and specify <b>RGB</b>
	/// coefficients used for image conversion to grayscale.</remarks>
	/// 
	public abstract class Grayscale : IFilter
	{
		// RGB coefficients for grayscale transformation
		private double	cr;
		private double	cg;
		private double	cb;

		/// <summary>
		/// Initializes a new instance of the <see cref="Grayscale"/> class
		/// </summary>
		/// 
		/// <param name="cr">Red coefficient</param>
		/// <param name="cg">Green coefficient</param>
		/// <param name="cb">Blue coefficient</param>
		/// 
		public Grayscale( double cr, double cg, double cb )
		{
			this.cr = cr;
			this.cg = cg;
			this.cb = cb;
		}

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
				new Rectangle(0, 0, image.Width, image.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

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

			// get width and height
			int width = imageData.Width;
			int height = imageData.Height;

			// create new grayscale image
			Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

			// lock destination bitmap data
			BitmapData dstData = dstImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

			int srcOffset = imageData.Stride - width * 3;
			int dstOffset = dstData.Stride - width;

			// do the job
			unsafe
			{
				byte * src = (byte *) imageData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, src += 3, dst ++ )
					{
						*dst = (byte)( cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B] );
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
