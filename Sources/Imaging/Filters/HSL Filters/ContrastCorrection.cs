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
	using AForge;

	/// <summary>
	/// Contrast adjusting in HSL color space
	/// </summary>
	/// 
	/// <remarks>The filter operates in <b>HSL</b> color space and adjusts
	/// pixels contrast value using luminance value of HSL color space.</remarks>
	/// 
	public class ContrastCorrection : IFilter, IInPlaceFilter
	{
		private HSLLinear	baseFilter = new HSLLinear( );
		private double		factor;

		/// <summary>
		/// Contrast adjusting factor. Defailt value is 1.25.
		/// </summary>
		public double Factor
		{
			get { return factor; }
			set
			{
				factor = Math.Max( 0.000001, value );

				// create luminance filter
				baseFilter.InLuminance	= new DoubleRange( 0.0, 1.0 );
				baseFilter.OutLuminance	= new DoubleRange( 0.0, 1.0 );

				if ( factor > 1 )
				{
					baseFilter.InLuminance = new DoubleRange( 0.5 - ( 0.5 / factor ), 0.5 + ( 0.5 / factor ) );
				}
				else
				{
					baseFilter.OutLuminance = new DoubleRange( 0.5 - ( 0.5 * factor ), 0.5 + ( 0.5 * factor ) );
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastCorrection"/> class
		/// </summary>
		/// 
		public ContrastCorrection( )
		{
			Factor = 1.25;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastCorrection"/> class
		/// </summary>
		/// 
		/// <param name="factor">Contrast adjusting factor</param>
		/// 
		public ContrastCorrection( double factor )
		{
			Factor = factor;
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
			return baseFilter.Apply( image );
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
		/// <remarks>The filter accepts bitmap data as input and returns the result
		/// of image processing filter as new image. The source image data are kept
		/// unchanged.</remarks>
		/// 
		public Bitmap Apply( BitmapData imageData )
		{
			return baseFilter.Apply( imageData );
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
			baseFilter.ApplyInPlace( image );
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
			baseFilter.ApplyInPlace( imageData );
		}
	}
}
