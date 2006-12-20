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
	/// Median filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Median : FilterAnyToAnyNewSameSize
	{
		private int	size = 3;

		/// <summary>
		/// Processing square size for the median filter. Minimum valus is 3.
		/// Maximum value is 25. The value should be odd. Default value is 3.
		/// </summary>
		public int Size
		{
			get { return size; }
			set { size = Math.Max( 3, Math.Min( 25, value | 1 ) ); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Median"/> class
		/// </summary>
		public Median( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Median"/> class
		/// </summary>
		/// 
		/// <param name="size">Processing square size</param>
		/// 
		public Median( int size )
		{
			Size = size;
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
			int i, j, t;
			// processing square's radius
			int radius = size >> 1;
			// number of elements
			int c;

			// array to hold pixel values (R, G, B)
			byte[]	r = new byte[size * size];
			byte[]	g = new byte[size * size];
			byte[]	b = new byte[size * size];

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
						c = 0;
			
						// for each kernel row
						for ( i = -radius; i <= radius; i++ )
						{
							t = y + i;

							// skip row
							if ( t < 0 )
								continue;
							// break
							if ( t >= height )
								break;

							// for each kernel column
							for ( j = -radius; j <= radius; j++ )
							{
								t = x + j;

								// skip column
								if ( t < 0 )
									continue;

								if ( t < width )
								{
									g[c++] = src[i * stride + j];
								}
							}
						}
						// sort elements
						Array.Sort( g, 0, c );
						// get the median
						*dst = g[c >> 1];
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
					for ( int x = 0; x < width; x++, src += 3, dst += 3 )
					{
						c = 0;
			
						// for each kernel row
						for ( i = -radius; i <= radius; i++ )
						{
							t = y + i;

							// skip row
							if ( t < 0 )
								continue;
							// break
							if ( t >= height )
								break;

							// for each kernel column
							for ( j = -radius; j <= radius; j++ )
							{
								t = x + j;

								// skip column
								if ( t < 0 )
									continue;

								if ( t < width )
								{
									p = &src[i * stride + j * 3];

									r[c] = p[RGB.R];
									g[c] = p[RGB.G];
									b[c] = p[RGB.B];
									c++;
								}
							}
						}

						// sort elements
						Array.Sort( r, 0, c );
						Array.Sort( g, 0, c );
						Array.Sort( b, 0, c );
						// get the median
						t = c >> 1;
						dst[RGB.R] = r[t];
						dst[RGB.G] = g[t];
						dst[RGB.B] = b[t];
					}
					src += offset;
					dst += offset;
				}
			}		
		}
	}
}
