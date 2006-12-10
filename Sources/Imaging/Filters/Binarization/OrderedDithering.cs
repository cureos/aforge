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
	/// Binarization with thresholds matrix
	/// </summary>
	public class OrderedDithering : FilterAnyToGray
	{
		private int rows = 4;
		private int cols = 4;

		private byte[,] matrix = new byte[4, 4]
		{
			{  15, 143,  47, 175 },
			{ 207,  79, 239, 111 },
			{  63, 191,  31, 159 },
			{ 255, 127, 223,  95 }
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedDithering"/> class
		/// </summary>
		/// 
		public OrderedDithering( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedDithering"/> class
		/// </summary>
		/// 
		/// <param name="matrix">Thresholds matrix</param>
		/// 
		public OrderedDithering( byte[,] matrix )
		{
			rows = matrix.GetLength( 0 );
			cols = matrix.GetLength( 1 );

			this.matrix = matrix;
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
			// get width and height
			int width = sourceData.Width;
			int height = sourceData.Height;

			int srcOffset = sourceData.Stride - ( ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );
			int dstOffset = destinationData.Stride - width;

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				// graysclae binarization
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++, src ++, dst ++)
					{
						*dst = (byte)( ( *src <= matrix[( y % rows ), ( x % cols )] ) ? 0 : 255 );
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			else
			{
				// RGB binarization
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++, src += 3, dst ++)
					{
						// grayscale value using BT709
						*dst = (byte)( ( (byte)( 0.2125 * src[RGB.R] + 0.7154 * src[RGB.G] + 0.0721 * src[RGB.B] ) <=
							matrix[( y % rows ), ( x % cols )] ) ? 0 : 255 );
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
		}
	}
}
