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
	using System.Collections;

	/// <summary>
	/// FiltersSequence class
	/// </summary>
	public class FiltersSequence : CollectionBase, IFilter
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersSequence"/> class
        /// </summary>
        /// 
		public FiltersSequence( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersSequence"/> class
        /// </summary>
        /// 
        /// <param name="filters">Sequence of filters to apply</param>
        /// 
        public FiltersSequence( params IFilter[] filters )
		{
			InnerList.AddRange( filters );
		}

        /// <summary>
        /// Get filter at the specified index 
        /// </summary>
        /// 
        /// <param name="index">Index of filter to get</param>
        /// 
        /// <returns>Returns filter at specified index.</returns>
        /// 
		public IFilter this[int index]
		{
			get { return ((IFilter) InnerList[index]); }
		}

        /// <summary>
        /// Add new filter to the sequence
        /// </summary>
        /// 
        /// <param name="filter">Filter to add to the sequence</param>
        /// 
		public void Add( IFilter filter )
		{
			InnerList.Add( filter );
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
			Bitmap	dstImg = null;
			Bitmap	tmpImg = null;
			int		n = InnerList.Count;

			// check for empty sequence
			if ( n == 0 )
				throw new ApplicationException( "No filters in the sequence" );

			// apply first filter
            dstImg = ( (IFilter) InnerList[0] ).Apply( imageData );

			// apply other filters
			for ( int i = 1; i < n; i++ )
			{
				tmpImg = dstImg;
				dstImg = ((IFilter) InnerList[i]).Apply( tmpImg );
				tmpImg.Dispose();
			}

			return dstImg;
		}
	}
}
