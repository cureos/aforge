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
	/// Base class for error diffusion dithering
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public abstract class ErrorDiffusionDithering : FilterAnyToGray
	{
		/// <summary>
		/// Current processing X coordinate
		/// </summary>
		protected int x;

		/// <summary>
		/// Current processing Y coordinate
		/// </summary>
		protected int y;

		/// <summary>
		/// Processing image's width
		/// </summary>
		protected int width;

		/// <summary>
		/// Processing image's height
		/// </summary>
		protected int height;

		/// <summary>
		/// Processing image's width minus 1
		/// </summary>
		protected int widthM1;

		/// <summary>
		/// Processing image's height minus 1 
		/// </summary>
		protected int heightM1;
		
		/// <summary>
		/// Processing image's stride (line size)
		/// </summary>
		protected int stride;

		/// <summary>
		/// Do error diffusion
		/// </summary>
		/// 
		/// <param name="error">Current error value</param>
		/// <param name="ptr">Pointer to current processing pixel</param>
		/// 
		/// <remarks>All parameters of the image and current processing pixel's coordinates
		/// are initialized in protected members.</remarks>
		/// 
		protected abstract unsafe void Diffuse( int error, byte * ptr );

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="sourceData">Source image data</param>
		/// <param name="destinationData">Destination image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
		{
			// get image dimension
			width		= sourceData.Width;
			height		= sourceData.Height;
			stride		= destinationData.Stride;
			widthM1		= width - 1;
			heightM1	= height - 1;

			// allocate memory for source copy
			IntPtr sourceCopy = Win32.LocalAlloc( Win32.MemoryFlags.Fixed, destinationData.Stride * height );

			// check source image format
			if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				// copy source image
				Win32.memcpy( sourceCopy, sourceData.Scan0, destinationData.Stride * height );
			}
			else
			{
				// convert color image to grayscale
				IFilter filter = new GrayscaleRMY( );
				Bitmap grayImage = filter.Apply( sourceData );
				// copy the image
				BitmapData grayData = grayImage.LockBits(
					new Rectangle( 0, 0, width, height ),
					ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

				Win32.memcpy( sourceCopy, grayData.Scan0, destinationData.Stride * height );

				grayImage.UnlockBits( grayData );
				// free the gray image
				grayImage.Dispose( );
			}

			int offset = stride - width;
			int	v, error;

			// do the job
			byte * src = (byte *) sourceCopy.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// for each line
			for ( y = 0; y < height; y++ )
			{
				// for each pixels
				for ( x = 0; x < width; x++, src++, dst++ )
				{
					v = *src;

					// fill the next destination pixel
					if ( v < 128 )
					{
						*dst = 0;
						error = v;
					}
					else
					{
						*dst = 255;
						error = v - 255;
					}

					// do error diffusion
					Diffuse( error, src );
				}
				src += offset;
				dst += offset;
			}

			// free memory for image copy
			Win32.LocalFree( sourceCopy );
		}
	}
}
