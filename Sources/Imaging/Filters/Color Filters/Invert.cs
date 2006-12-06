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
	/// Invert image
	/// </summary>
	/// 
	/// <remarks>The filter inverts colored and grayscale images.</remarks> 
	///
	public sealed class Invert : FilterAnyToAny
	{
		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			int width	= imageData.Width;
			int height	= imageData.Height;

			int lineSize = width * ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3 );
			int offset = imageData.Stride - lineSize;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );

			// invert
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < lineSize; x++, ptr ++ )
				{
					// ivert each pixel
					*ptr = (byte)( 255 - *ptr );
				}
				ptr += offset;
			}
		}
	}
}
