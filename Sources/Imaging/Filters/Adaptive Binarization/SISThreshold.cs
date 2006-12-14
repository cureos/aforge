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
	/// Threshold using Simple Image Statistics (SIS)
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class SISThreshold : FilterAnyToGray
	{
		private byte threshold;

		/// <summary>
		/// Threshold value
		/// </summary>
		/// 
		/// <remarks>The property is read only and represents the value, which
		/// was automaticaly calculated using image statistics.</remarks>
		/// 
		public byte Threshold
		{
			get { return threshold; }
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
			byte * src = null;
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// get width and height
			int width = sourceData.Width;
			int height = sourceData.Height;
			int widthM1 = width - 1;
			int heightM1 = height - 1;
			int stride = destinationData.Stride;
			int offset = stride - width;
			double ex, ey, weight, weightTotal = 0, total = 0;

			// convert image to grayscale if it is color
			Bitmap		grayImage = null;
			BitmapData	grayData = null;

			if ( sourceData.PixelFormat != PixelFormat.Format8bppIndexed )
			{
				// create grayscale filter
				IFilter filter = new GrayscaleRMY( );
				// do the processing
				grayImage = filter.Apply( sourceData );
				// lock the image
				grayData = grayImage.LockBits(
					new Rectangle( 0, 0, width, height ),
					ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );
				// set source pointer
				src = (byte *) grayData.Scan0.ToPointer( );
			}
			else
			{
				src = (byte *) sourceData.Scan0.ToPointer( );
			}

			// --- 1st pass - collecting statistics

			// skip the first line for the first pass
			src += stride;

			// for each line
			for ( int y = 1; y < heightM1; y++ )
			{
				src++;
				// for each pixels
				for ( int x = 1; x < widthM1; x++, src++ )
				{
					// the equations are:
					// ex = I(x + 1, y) - I(x - 1, y)
					// ey = I(x, y + 1) - I(x, y - 1)
					// weight = max(ex, ey)
					// weightTotal += weight
					// total += weight * I(x, y)
					
					ex = src[1] - src[-1];
					ey = src[stride] - src[-stride];
					weight = ( ex > ey ) ? ex : ey;
					weightTotal += weight;
					total += weight * ( *src );
				}
				src += offset + 1;
			}

			// calculate threshold
			threshold = ( weightTotal == 0 ) ? (byte) 0 : (byte) ( total / weightTotal );

			// --- 2nd pass - thresholding
			src = ( grayData != null ) ? (byte *) grayData.Scan0.ToPointer( ) : (byte *) sourceData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for all pixels
				for ( int x = 0; x < width; x++, src++, dst++ )
				{
					*dst = (byte) ( ( *src <= threshold ) ? 0 : 255 );
				}
				src += offset;
				dst += offset;
			}

			// release gray image, if there was conversion
			if ( grayData != null )
			{
				grayImage.UnlockBits( grayData );
				grayImage.Dispose( );
			}
		}
	}
}
