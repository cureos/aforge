// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Blur filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Blur : Correlation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Blur"/> class
		/// </summary>
		public Blur( ) : base( new int[,] {
								{ 1, 2, 3, 2, 1 },
								{ 2, 4, 5, 4, 2 },
								{ 3, 5, 6, 5, 3 },
								{ 2, 4, 5, 4, 2 },
								{ 1, 2, 3, 2, 1 } } )
		{
		}
	}
}
