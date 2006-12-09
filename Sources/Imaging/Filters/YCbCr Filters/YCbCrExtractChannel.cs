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
	/// Extract YCbCr channel from image
	/// </summary>
	/// 
	/// <remarks>Extracts specified YCbCr channel of color image and returns
	/// it the form of grayscale image.</remarks>
	/// 
	public class YCbCrExtractChannel : FilterColorToGray
	{
		private short channel = YCbCr.YIndex;

		/// <summary>
		/// YCbCr channel to extract (default value is Y)
		/// </summary>
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != YCbCr.YIndex ) &&
					( value != YCbCr.CbIndex ) &&
					( value != YCbCr.CrIndex )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YCbCrExtractChannel"/> class
		/// </summary>
		/// 
		public YCbCrExtractChannel( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="YCbCrExtractChannel"/> class
		/// </summary>
		/// 
		/// <param name="channel">YCbCr channel to extract</param>
		/// 
		public YCbCrExtractChannel( short channel )
		{
			this.Channel = channel;
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

			int srcOffset = sourceData.Stride - width * 3;
			int dstOffset = destinationData.Stride - width;
			RGB rgb = new RGB( );
			YCbCr ycbcr = new YCbCr( );

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );
			byte v = 0;

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, src += 3, dst ++ )
				{
					rgb.Red		= src[RGB.R];
					rgb.Green	= src[RGB.G];
					rgb.Blue	= src[RGB.B];

					// convert to YCbCr
					AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

					switch ( channel )
					{
						case YCbCr.YIndex:
							v = (byte) ( ycbcr.Y * 255 );
							break;

						case YCbCr.CbIndex:
							v = (byte) ( ( ycbcr.Cb + 0.5 ) * 255 );
							break;

						case YCbCr.CrIndex:
							v = (byte) ( ( ycbcr.Cr + 0.5 ) * 255 );
							break;
					}

					*dst = v;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}
	}
}
