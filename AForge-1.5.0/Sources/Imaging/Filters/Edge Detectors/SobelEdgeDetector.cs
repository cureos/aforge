// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//
// Article by Bill Green was used as the reference
// http://www.pages.drexel.edu/~weg22/can_tut.html
//
namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Sobel edge detector
	/// </summary>
	public class SobelEdgeDetector : FilterColorToGray
	{
		// Sobel kernels
		private static int[,] xKernel = new int[,]
		{
			{ -1,  0,  1 },
			{ -2,  0,  2 },
			{ -1,  0,  1 }
		};
		private static int[,] yKernel = new int[,]
		{
			{  1,  2,  1 },
			{  0,  0,  0 },
			{ -1, -2, -1 }
		};

		private bool scaleIntensity = true;

		/// <summary>
		/// Scale intensity
		/// </summary>
		/// 
		/// <remarks>The property  determines if pixels intensity should be scaled in the
		/// range of the lowest and the highest possible intensity value.</remarks>
		/// 
		public bool ScaleIntensity
		{
			get { return scaleIntensity; }
			set { scaleIntensity = value; }
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

			// loop and array indexes
			int i, j, ir;
			// variables for gradient calculation
			double	v, gx, gy, g, max = 0;

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
				src++;
				dst++;

				// for each pixel
				for ( int x = 1; x < widthM1; x++, src++, dst++ )
				{
					gx = gy = 0;
					// for each kernel row
					for ( i = 0; i < 3; i++ )
					{
						ir = i - 1;
						// for each kernel column
						for ( j = 0; j < 3; j++ )
						{
							// source value
							v = src[ir * stride + j - 1];

							gx += v * xKernel[i, j];
							gy += v * yKernel[i, j];
						}
					}
					// get gradient value
//					g = Math.Min( Math.Sqrt( gx * gx + gy * gy ), 255 );
					g = Math.Min( Math.Abs( gx ) + Math.Abs( gy ), 255 );	// approximation
					if ( g > max )
						max = g;
					*dst = (byte) g;
				}
				src += ( offset + 1 );
				dst += ( offset + 1 );
			}

			// do we need scaling
			if ( ( scaleIntensity ) && ( max != 255 ) )
			{
				// make the second pass for intensity scaling
				double factor = 255.0 / (double) max;
				dst = (byte *) destinationData.Scan0.ToPointer( ) + stride;

				// for each line
				for ( int y = 1; y < heightM1; y++ )
				{
					dst++;
					// for each pixel
					for (int x = 1; x <	widthM1; x++, dst++ )
					{
						*dst = (byte) ( factor * *dst );
					}
					dst += ( offset + 1 );
				}
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
