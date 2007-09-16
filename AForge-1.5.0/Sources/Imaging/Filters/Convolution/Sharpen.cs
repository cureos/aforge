// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Sharpen filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Sharpen : Correlation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Sharpen"/> class
		/// </summary>
		public Sharpen( ) : base( new int[,] {
										{  0, -1,  0 },
										{ -1,  5, -1 },
										{  0, -1,  0 } } )
		{
		}
	}
}
