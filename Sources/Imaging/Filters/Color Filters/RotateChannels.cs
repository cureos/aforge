// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Rotate RGB channels
	/// </summary>
	/// 
	/// <remarks>The filter rotates RGB channels: red channel is replaced with green,
	/// green channel is replaced with blue, blue channel is replaced with red.</remarks>
	/// 
	public sealed class RotateChannels : FilterColorToColor
	{
		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">Image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			int width	= imageData.Width;
			int height	= imageData.Height;
			int offset	= imageData.Stride - width * 3;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );
			byte t;

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					// rotate colors of each pixel
					t = ptr[RGB.R];
					ptr[RGB.R] = ptr[RGB.G];
					ptr[RGB.G] = ptr[RGB.B];
					ptr[RGB.B] = t;
				}
				ptr += offset;
			}
		}
	}
}
