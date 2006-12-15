// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Correlation filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Correlation : FilterAnyToAnyNewSameSize
	{
		/// <summary>
		/// Processing kernel
		/// </summary>
		protected int[,]	kernel;
		
		/// <summary>
		/// Kernel size
		/// </summary>
		protected int		size;

		/// <summary>
		/// Initializes a new instance of the <see cref="Correlation"/> class
		/// </summary>
		/// 
		protected Correlation( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Correlation"/> class
		/// </summary>
		/// 
		/// <param name="kernel">Processing kernel</param>
		/// 
		public Correlation( int[,] kernel )
		{
			int s = kernel.GetLength( 0 );

			// check kernel size
			if ( ( s != kernel.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
				throw new ArgumentException( );

			this.kernel = kernel;
			this.size = s;
		}

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="sourceData">Source image data</param>
		/// <param name="destinationData">Destination image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
		{
			// get source image size
			int width = sourceData.Width;
			int height = sourceData.Height;

			int stride = sourceData.Stride;
			int offset = stride - ( ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

			// loop and array indexes
			int i, j, t, k, ir, jr;
			// kernel's radius
			int radius = size >> 1;
			// color sums
			long r, g, b, div;

			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );
			byte * p;

			// do the processing job
			if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				// grayscale image

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, src++, dst++ )
					{
						g = div = 0;
			
						// for each kernel row
						for ( i = 0; i < size; i++ )
						{
							ir = i - radius;
							t = y + ir;

							// skip row
							if ( t < 0 )
								continue;
							// break
							if ( t >= height )
								break;

							// for each kernel column
							for ( j = 0; j < size; j++ )
							{
								jr = j - radius;
								t = x + jr;

								// skip column
								if ( t < 0 )
									continue;

								if ( t < width )
								{
									k = kernel[i, j];

									div += k;
									g += k * src[ir * stride + jr];
								}
							}
						}

						// check divider
						if ( div != 0 )
						{
							g /= div;
						}
						*dst = ( g > 255 ) ? (byte) 255 : ( ( g < 0 ) ? (byte) 0 : (byte) g );
					}
					src += offset;
					dst += offset;
				}
			}
			else
			{
				// RGB image

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x <	width; x++, src += 3, dst += 3 )
					{
						r = g = b = div = 0;
			
						// for each kernel row
						for ( i = 0; i < size; i++ )
						{
							ir = i - radius;
							t = y + ir;

							// skip row
							if ( t < 0 )
								continue;
							// break
							if ( t >= height )
								break;

							// for each kernel column
							for ( j = 0; j < size; j++ )
							{
								jr = j - radius;
								t = x + jr;

								// skip column
								if ( t < 0 )
									continue;

								if ( t < width )
								{
									k = kernel[i, j];
									p = &src[ir * stride + jr * 3];

									div += k;

									r += k * p[RGB.R];
									g += k * p[RGB.G];
									b += k * p[RGB.B];
								}
							}
						}

						// check divider
						if ( div != 0 )
						{
							r /= div;
							g /= div;
							b /= div;
						}
						dst[RGB.R] = ( r > 255 ) ? (byte) 255 : ( ( r < 0 ) ? (byte) 0 : (byte) r );
						dst[RGB.G] = ( g > 255 ) ? (byte) 255 : ( ( g < 0 ) ? (byte) 0 : (byte) g );
						dst[RGB.B] = ( b > 255 ) ? (byte) 255 : ( ( b < 0 ) ? (byte) 0 : (byte) b );
					}
					src += offset;
					dst += offset;
				}
			}		
		}
	}
}
