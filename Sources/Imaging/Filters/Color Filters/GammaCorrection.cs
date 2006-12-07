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
	/// Gamma correction filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class GammaCorrection : FilterAnyToAny
	{
		private double	gamma;
		private byte[]	table = new byte[256];

		/// <summary>
		/// Gamma value (default value is 2.2)
		/// </summary>
		public double Gamma
		{
			get { return gamma; }
			set
			{
				// get gamma value
				gamma = Math.Max( 0.1, Math.Min( 5.0, value ) );

				// calculate tranformation table
				double g = 1 / gamma;
				for ( int i = 0; i < 256; i++ )
				{
					table[i] = (byte) Math.Min( 255, (int) ( Math.Pow( i / 255.0, g ) * 255 + 0.5 ) );
				}
			}
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="GammaCorrection"/> class
		/// </summary>
		public GammaCorrection( )
		{
			Gamma = 2.2;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GammaCorrection"/> class
		/// </summary>
		/// 
		/// <param name="gamma">Gamma value</param>
		/// 
		public GammaCorrection( double gamma )
		{
			Gamma = gamma;
		}


		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">Image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			int width	= imageData.Width;
			int height	= imageData.Height;

			int lineSize = width * ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3 );
			int offset = imageData.Stride - lineSize;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );

			// gamma correction
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < lineSize; x++, ptr ++ )
				{
					// process each pixel
					*ptr = table[*ptr];
				}
				ptr += offset;
			}
		}
	}
}
