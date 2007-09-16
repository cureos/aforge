// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;

	/// <summary>
	/// Ordered dithering uaing Bayer matrix.
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class BayerDithering : OrderedDithering
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BayerDithering"/> class.
		/// </summary>
		/// 
		public BayerDithering( ) : base( new byte[,] {
								{   0, 192,  48, 240 },
								{ 128,  64, 176, 112 },
								{  32, 224,  16, 208 },
								{ 160,  96, 144,  80 } } )
		{
		}
	}
}
