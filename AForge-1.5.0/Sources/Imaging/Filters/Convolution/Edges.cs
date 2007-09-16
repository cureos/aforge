// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Simple edge detector
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Edges : Correlation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Edges"/> class
		/// </summary>
		public Edges( ) : base( new int[,] {
										{  0, -1,  0 },
										{ -1,  4, -1 },
										{  0, -1,  0 } } )
		{
		}
	}
}
