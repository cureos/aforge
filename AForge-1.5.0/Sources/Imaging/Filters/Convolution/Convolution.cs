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
	/// Convolution filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Convolution : Correlation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Convolution"/> class
		/// </summary>
		/// 
		/// <param name="kernel">Processing kernel</param>
		/// 
		public Convolution( int[,] kernel )
		{
			int s = kernel.GetLength( 0 );

			// check kernel size
			if ( ( s != kernel.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
				throw new ArgumentException( );

			this.kernel = new int[s, s];
			this.size = s;

			// reverse the array
			for ( int i = 0; i < s; i++ )
			{
				for (int j = 0; j < s; j++ )
				{
					this.kernel[i, j] = kernel[s - i - 1, s - j - 1];
				}
			}
			// the rest is usual correlation
		}
	}
}
