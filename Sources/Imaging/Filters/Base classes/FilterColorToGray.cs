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
	/// Base class for filters, which transform color image to grayscale
	/// </summary>
	/// 
	/// <remarks>The abstract class is the base class for all filters, which
	/// transform color imgae to grayscale without changing image dimension.
	/// Filters based on this class cannot be applied directly to the source
	/// image. Instead of this they provide output image as a result of image
	/// processing routine.
	/// </remarks>
	/// 
	public abstract class FilterColorToGray : IFilter
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

			// process the filter
			ProcessFilter( imageData, dstData );

			// unlock destination images
			dstImage.UnlockBits( dstData );

			return dstImage;
		}

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="sourceData">Source image data</param>
		/// <param name="destinationData">Destination image data</param>
		/// 
		protected abstract unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData );
	}
}
