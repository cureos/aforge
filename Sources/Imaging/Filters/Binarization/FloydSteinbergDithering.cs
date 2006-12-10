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
	/// Dithering using Floyd-Steinberg error diffusion
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class FloydSteinbergDithering : ErrorDiffusionDithering
	{
		private static int[] coef = new int[] { 3, 5, 1 };

		/// <summary>
		/// Do error diffusion
		/// </summary>
		/// 
		/// <param name="error">Current error value</param>
		/// <param name="ptr">Pointer to current processing pixel</param>
		/// 
		/// <remarks>All parameters of the image and current processing pixel's coordinates
		/// are initialized by base class.</remarks>
		/// 
		protected override unsafe void Diffuse( int error, byte * ptr )
		{
			int ed;	// error diffusion

			// calculate error diffusion
			if ( x != widthM1 )
			{
				// right pixel
				ed = ptr[1] + ( error * 7 ) / 16;
				ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
				ptr[1] = (byte) ed;
			}

			if ( y != heightM1 )
			{
				// bottom pixels
				ptr += stride;
				for ( int i = -1, j = 0; i <= 1; i++, j++ )
				{
					if ( ( x + i >= 0 ) && ( x + i < width ) )
					{
						ed = ptr[i] + ( error * coef[j] ) / 16;
						ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
						ptr[i] = (byte) ed;
					}
				}
			}
		}
	}
}
