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
	/// Threshold binarization with error carry
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class ThresholdWithCarry : FilterAnyToGray
	{
		private byte threshold = 128;

		/// <summary>
		/// Threshold value (default is 128)
		/// </summary>
		public byte ThresholdValue
		{
			get { return threshold; }
			set { threshold = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class
		/// </summary>
		/// 
		public ThresholdWithCarry( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class
		/// </summary>
		/// 
		/// <param name="threshold">Threshold value</param>
		/// 
		public ThresholdWithCarry( byte threshold )
		{
			this.threshold = threshold;
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

			short carry = 0;

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				// graysclae binarization
				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, src++, dst++ )
					{
						carry += *src;

						if ( carry >= threshold )
						{
							*dst = (byte) 255;
							carry -= 255;
						}
						else
						{
							*dst = (byte) 0;
						}
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			else
			{
				// RGB binarization
				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, src += 3, dst ++ )
					{
						// grayscale value using BT709
						carry += (byte)( 0.2125 * src[RGB.R] + 0.7154 * src[RGB.G] + 0.0721 * src[RGB.B] );

						if ( carry > threshold )
						{
							*dst = (byte) 255;
							carry -= 255;
						}
						else
						{
							*dst = (byte) 0;
						}
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
		}
	}
}
