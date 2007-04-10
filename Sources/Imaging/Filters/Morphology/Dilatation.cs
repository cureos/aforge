// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Dilatation operator from Mathematical Morphology.
	/// </summary>
	/// 
    /// <remarks><para>The filter assigns maximum value of surrounding pixels to each pixel of
    /// the result filter. Surrounding pixels, which should be processed, are specified by
    /// structuring element: 1 - to process the neighbor, 0 - to skip it.</para>
    /// <para>The filter especially useful for binary image processing, where it allows to grow
    /// separate objects or join close objects.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Dilatation filter = new Dilatation( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="dilatation.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class Dilatation : FilterAnyToAnyNewSameSize
	{
		// structuring element
		private short[,]	se = new short[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
		private int			size = 3;

		/// <summary>
		/// Initializes a new instance of the <see cref="Dilatation"/> class.
		/// </summary>
        /// 
		public Dilatation( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Dilatation"/> class.
		/// </summary>
		/// 
		/// <param name="se">Structuring element.</param>
		/// 
		public Dilatation( short[,] se )
		{
			int s = se.GetLength( 0 );

			// check structuring element size
			if ( ( s != se.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
				throw new ArgumentException( );

			this.se = se;
			this.size = s;
		}

		/// <summary>
		/// Process the filter on the specified image.
		/// </summary>
		/// 
		/// <param name="sourceData">Source image data.</param>
		/// <param name="destinationData">Destination image data.</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
		{
			// get image size
			int width	= destinationData.Width;
			int height	= destinationData.Height;
			int stride	= destinationData.Stride;
            int offset  = stride - ( ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );
            // loop and array indexes
			int	t, ir, jr, i, j;
			// structuring element's radius
			int	r = size >> 1;

			// do the job
			byte * src = (byte *) sourceData.Scan0.ToPointer( );
			byte * dst = (byte *) destinationData.Scan0.ToPointer( );

            if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                byte max, v;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++, dst++ )
                    {
                        max = 0;

                        // for each structuring element's row
                        for ( i = 0; i < size; i++ )
                        {
                            ir = i - r;
                            t = y + ir;

                            // skip row
                            if ( t < 0 )
                                continue;
                            // break
                            if ( t >= height )
                                break;

                            // for each structuring slement's column
                            for ( j = 0; j < size; j++ )
                            {
                                jr = j - r;
                                t = x + jr;

                                // skip column
                                if ( t < 0 )
                                    continue;
                                if ( t < width )
                                {
                                    if ( se[i, j] == 1 )
                                    {
                                        // get new MAX value
                                        v = src[ir * stride + jr];
                                        if ( v > max )
                                            max = v;
                                    }
                                }
                            }
                        }
                        // result pixel
                        *dst = max;
                    }
                    src += offset;
                    dst += offset;
                }
            }
            else
            {
                // color image
                byte maxR, maxG, maxB, v;
                byte* p;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src += 3, dst += 3 )
                    {
                        maxR = maxG = maxB = 0;

                        // for each structuring element's row
                        for ( i = 0; i < size; i++ )
                        {
                            ir = i - r;
                            t = y + ir;

                            // skip row
                            if ( t < 0 )
                                continue;
                            // break
                            if ( t >= height )
                                break;

                            // for each structuring element's column
                            for ( j = 0; j < size; j++ )
                            {
                                jr = j - r;
                                t = x + jr;

                                // skip column
                                if ( t < 0 )
                                    continue;
                                if ( t < width )
                                {
                                    if ( se[i, j] == 1 )
                                    {
                                        // get new MAX values
                                        p = &src[ir * stride + jr * 3];

                                        // red
                                        v = p[RGB.R];
                                        if ( v > maxR )
                                            maxR = v;

                                        // green
                                        v = p[RGB.G];
                                        if ( v > maxG )
                                            maxG = v;

                                        // blue
                                        v = p[RGB.B];
                                        if ( v > maxB )
                                            maxB = v;
                                    }
                                }
                            }
                        }
                        // result pixel
                        dst[RGB.R] = maxR;
                        dst[RGB.G] = maxG;
                        dst[RGB.B] = maxB;
                    }
                    src += offset;
                    dst += offset;
                }
            }
		}
	}
}
