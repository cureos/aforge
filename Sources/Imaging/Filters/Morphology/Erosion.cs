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
	/// Erosion operator from Mathematical Morphology
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Erosion : FilterGrayToGrayNewSameSize
	{
		// structuring element
		private short[,]	se = new short[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
		private int			size = 3;

		/// <summary>
		/// Initializes a new instance of the <see cref="Erosion"/> class
		/// </summary>
		public Erosion( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Erosion"/> class
		/// </summary>
		/// 
		/// <param name="se">Structuring element</param>
		/// 
		public Erosion( short[,] se )
		{
			int s = se.GetLength( 0 );

			// check structuring element size
			if ( ( s != se.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
				throw new ArgumentException( );

			this.se = se;
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
			// get image size
			int width	= destinationData.Width;
			int height	= destinationData.Height;
			int stride	= destinationData.Stride;
			int offset	= stride - width;
			// loop and array indexes
			int	t, ir, jr, i, j;
			// structuring element's radius
			int	r = size >> 1;
			// pixel value
			byte min, v;

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, src++, dst++ )
				{
					min = 255;

					// for each structuring element's row
					for ( i = 0; i < size; i++ )
					{
						ir = i - r;
						t = y + ir;

						// skip row
						if ( t < 0 )
							continue;
						// break
						if ( t >= height )
							break;

						// for each structuring element's column
						for ( j = 0; j < size; j++ )
						{
							jr = j - r;
							t = x + jr;

							// skip column
							if ( t < 0 )
								continue;
							if ( t < width )
							{
								if ( se[i, j] == 1 )
								{
									// get new MIN value
									v = src[ir * stride + jr];
									if ( v < min )
										min = v;
								}
							}
						}
					}
					// result pixel
					*dst = min;
				}
				src += offset;
				dst += offset;
			}
		}
	}
}