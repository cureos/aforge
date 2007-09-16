// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Mean filter
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Mean : Correlation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mean"/> class
		/// </summary>
		public Mean( ) : base( new int[,] {
										{ 1, 1, 1 },
										{ 1, 1, 1 },
										{ 1, 1, 1 } } )
		{
		}
	}
}
