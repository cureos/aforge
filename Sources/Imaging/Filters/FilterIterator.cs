// AForge Image Processing Library
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
	/// Filter iterator
	/// </summary>
    /// 
    /// <remarks>Filter iterator performs specified amounr of filter's iterations.</remarks>
    /// 
	public class FilterIterator : IFilter
	{
		private IFilter	baseFilter;
		private int		iterations = 1;

        /// <summary>
        /// Base filter
        /// </summary>
        /// 
        /// <remarks>The base filter is the filter to be applied specified amount of iterations to
        /// the specified image.</remarks>
        /// 
		public IFilter BaseFilter
		{
			get { return baseFilter; }
			set { baseFilter = value; }
		}

        /// <summary>
        /// Iterations amount
        /// </summary>
        /// 
        /// <remarks>The amount of times to apply specified filter to the specified image.
        /// Default value is 1. Minimum value is 1. Maximum value 255.</remarks>
        /// 
		public int Iterations
		{
			get { return iterations; }
			set { iterations = Math.Max( 1, Math.Min( 255, value ) ); }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIterator"/> class
        /// </summary>
        /// 
        /// <param name="baseFilter">Filter to iterate</param>
        /// 
		public FilterIterator( IFilter baseFilter )
		{
			this.baseFilter = baseFilter;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIterator"/> class
        /// </summary>
        /// 
        /// <param name="baseFilter">Filter to iterate</param>
        /// <param name="iterations">Iterations amount</param>
        /// 
		public FilterIterator( IFilter baseFilter, int iterations )
		{
			this.baseFilter = baseFilter;
			this.iterations = iterations;
		}

        /// <summary>
        /// Apply filter to an image
        /// </summary>
        /// 
        /// <param name="image">Source image to apply filter to</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns the
        /// the result of image processing filter as new image.</remarks> 
        ///
        public Bitmap Apply( Bitmap image )
		{
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly,
                ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb );

            // apply the filter
            Bitmap dstImage = Apply( imageData );

            // unlock source image
            image.UnlockBits( imageData );

            return dstImage;
		}

        /// <summary>
        /// Apply filter to an image
        /// </summary>
        /// 
        /// <param name="imageData">Source image to apply filter to</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image</returns>
        /// 
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            // initial iteration
            Bitmap dstImg = baseFilter.Apply( imageData );
            Bitmap tmpImg;

            // continue to iterate
            for ( int i = 1; i < iterations; i++ )
            {
                tmpImg = dstImg;
                dstImg = baseFilter.Apply( tmpImg );
                tmpImg.Dispose( );
            }

            return dstImg;
        }
	}
}
