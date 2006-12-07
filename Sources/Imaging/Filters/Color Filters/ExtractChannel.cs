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
	/// Extract RGB channel from image
	/// </summary>
	/// 
	/// <remarks>Extracts specified channel of color image and returns
	/// it the form of grayscale image.</remarks>
	/// 
	public class ExtractChannel : FilterColorToGray
	{
		private short channel = RGB.R;

		/// <summary>
		/// RGB channel to extract (default value is red)
		/// </summary>
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != RGB.R ) &&
					( value != RGB.G ) &&
					( value != RGB.B )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}

		// Constructor


		/// <summary>
		/// Initializes a new instance of the <see cref="ExtractChannel"/> class
		/// </summary>
		public ExtractChannel( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtractChannel"/> class
		/// </summary>
		/// 
		/// <param name="channel">RGB channel to extract</param>
		/// 
		public ExtractChannel( short channel )
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

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

			// allign source pointer to the required channel
			src += channel;

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++, src += 3, dst ++ )
				{
					*dst = *src;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}
	}
}
