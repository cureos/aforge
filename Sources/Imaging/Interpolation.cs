// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;

	/// <summary>
	/// Interpolation routines
	/// </summary>
    /// 
	internal class Interpolation
	{
        /// <summary>
        /// Bicubic kernel
        /// </summary>
        /// 
        /// <param name="x">X value</param>
        /// 
        /// <returns>Bicubic cooefficient</returns>
        /// 
		public static double BiCubicKernel( double x)
		{
			if ( x > 2.0 )
				return 0.0 ;

			double a, b, c, d;
            double xm1 = x - 1.0;
            double xp1 = x + 1.0;
            double xp2 = x + 2.0;

			a = ( xp2 <= 0.0 ) ? 0.0 : xp2 * xp2 * xp2;
			b = ( xp1 <= 0.0 ) ? 0.0 : xp1 * xp1 * xp1;
			c = ( x   <= 0.0 ) ? 0.0 : x * x * x;
			d = ( xm1 <= 0.0 ) ? 0.0 : xm1 * xm1 * xm1;

			return ( 0.16666666666666666667 * ( a - ( 4.0 * b ) + ( 6.0 * c ) - ( 4.0 * d ) ) );
		}
	}
}
