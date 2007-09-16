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
	/// Convert grayscale image to RGB
	/// </summary>
	/// 
	/// <remarks>The filter creates color image from specified grayscale image
	/// initializing all RGB channels to the same value - pixel's intensity.</remarks>
	/// 
	public sealed class GrayscaleToRGB : FilterGrayToColor
	{
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

			int srcOffset = sourceData.Stride - width;
			int dstOffset = destinationData.Stride - width * 3;

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, src++, dst += 3 )
				{
					dst[RGB.R] = dst[RGB.G] = dst[RGB.B] = *src;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}
	}
}
