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
	/// Homogenity edge detector
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class DifferenceEdgeDetector : FilterColorToGray
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
			byte * src = null;
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// get width and height
			int width = sourceData.Width;
			int height = sourceData.Height;
			int widthM1 = width - 1;
			int heightM1 = height - 1;
			int stride = destinationData.Stride;
			int offset = stride - width;

			int d, max;

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

			// do the processing job

			// skip one stride
			src += stride;
			dst += stride;

			// for each line
			for ( int y = 1; y < heightM1; y++ )
			{
				src ++;
				dst ++;

				// for each pixel
				for ( int x = 1; x < widthM1; x++, src++, dst++ )
				{
					max = 0;

					// left diagonal
					d = (int) src[-stride - 1] - src[stride + 1];
					if ( d < 0 )
						d = -d;
					if ( d > max )
						max = d;
					// right diagonal
					d = (int) src[-stride + 1] - src[stride - 1];
					if ( d < 0 )
						d = -d;
					if ( d > max )
						max = d;
					// vertical
					d = (int) src[-stride] - src[stride];
					if ( d < 0 )
						d = -d;
					if ( d > max )
						max = d;
					// horizontal
					d = (int) src[-1] - src[1];
					if ( d < 0 )
						d = -d;
					if ( d > max )
						max = d;

					*dst = (byte) max;
				}
				src += offset + 1;
				dst += offset + 1;
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
