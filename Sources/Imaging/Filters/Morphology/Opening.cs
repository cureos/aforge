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
	/// Opening operator from Mathematical Morphology
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Opening : IFilter
	{
		private IFilter errosion = new Erosion( );
		private IFilter dilatation = new Dilatation( );

		/// <summary>
		/// Initializes a new instance of the <see cref="Opening"/> class
		/// </summary>
		public Opening( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Opening"/> class
		/// </summary>
		/// 
		/// <param name="se">Structuring element</param>
		/// 
		public Opening( short[,] se )
		{
			errosion = new Erosion( se );
			dilatation = new Dilatation( se );
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
			Bitmap tempImage = errosion.Apply( image );
			Bitmap destImage = dilatation.Apply( tempImage );

			tempImage.Dispose();

			return destImage;
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
			Bitmap tempImage = errosion.Apply( imageData );
			Bitmap destImage = dilatation.Apply( tempImage );

			tempImage.Dispose();

			return destImage;
		}
	}
}
