// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Mean filter.
	/// </summary>
	/// 
    /// <remarks><para>The filter performs each pixel value's averaging with its 8 neighbors, which is 
    /// <see cref="Convolution">convolution filter</see> using the mean kernel:</para>
    /// 
    /// <code lang="none">
    /// 1  1  1
    /// 1  1  1
    /// 1  1  1
    /// </code>
    /// 
    /// <para>For the list of supported pixel formats, see the documentation to <see cref="Convolution"/>
    /// filter.</para>
    /// 
    /// <para>With the above kernel the convolution filter is just calculates each pixel's value
    /// in result image as average of 9 corresponding pixels in the source image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Mean filter = new Mean( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample13.png" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/mean.png" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="Convolution"/>
    /// 
    public sealed class Mean : Convolution
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mean"/> class.
		/// </summary>
		public Mean( ) : base( new int[,] {
										{ 1, 1, 1 },
										{ 1, 1, 1 },
										{ 1, 1, 1 } } )
		{
		}
	}
}
