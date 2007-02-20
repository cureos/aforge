// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for image resizing filters
    /// </summary>
    /// 
    /// <remarks>The abstract class is the base class for all filters,
    /// which implement resizing algorithms.</remarks>
    /// 
    public abstract class FilterResize : FilterAnyToAnyNew
    {
        /// <summary>
        /// New image width
        /// </summary>
        protected int newWidth;

        /// <summary>
        /// New image height
        /// </summary>
        protected int newHeight;

        /// <summary>
        /// Width of new image
        /// </summary>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set { newWidth = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Height of new image
        /// </summary>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set { newHeight = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterResize"/> class
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image</param>
        /// <param name="newHeight">Height of new image</param>
        /// 
        protected FilterResize( int newWidth, int newHeight )
		{
			this.newWidth   = newWidth;
			this.newHeight  = newHeight;
		}

        /// <summary>
        /// Calculates new image size
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// 
        /// <returns>New image size</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( BitmapData sourceData )
        {
            return new Size( newWidth, newHeight );
        }
    }
}
