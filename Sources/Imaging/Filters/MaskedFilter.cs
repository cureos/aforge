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
	/// Masked filter - filter an image using binary mask image.
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
	public class MaskedFilter : IFilter
	{
		private Bitmap	mask;
		private IFilter	filter1 = null;
		private IFilter	filter2 = null;

        /// <summary>
        /// Binary mask image
        /// </summary>
        /// 
		public Bitmap Mask
		{
			get { return mask; }
			set { mask = value; }
		}

        /// <summary>
        /// Filter to be applied to image regions, which correspond to
        /// black pixels of the specified mask.
        /// </summary>
        /// 
		public IFilter Filter1
		{
			get { return filter1; }
			set { filter1 = value; }
		}

        /// <summary>
        /// Filter to be applied to image regios, which correspons to
        /// white pixels of the specified mask.
        /// </summary>
        /// 
        /// <remarks>If the filter is not set, then original source images
        /// will be used for white pixels of the specified mask.</remarks>
        /// 
		public IFilter Filter2
		{
			get { return filter2; }
			set { filter2 = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedFilter"/> class
        /// </summary>
        /// 
        /// <param name="mask">Binary mask image</param>
        /// <param name="filter1">Filter for black regions of the mask</param>
        /// 
        public MaskedFilter( Bitmap mask, IFilter filter1 )
		{
			this.mask = mask;
			this.filter1 = filter1;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedFilter"/> class
        /// </summary>
        /// 
        /// <param name="mask">Binary mask image</param>
        /// <param name="filter1">Filter for black regions of the mask</param>
        /// <param name="filter2">Filter for white regions of the mask</param>
        /// 
		public MaskedFilter( Bitmap mask, IFilter filter1, IFilter filter2 )
		{
			this.mask = mask;
			this.filter1 = filter1;
			this.filter2 = filter2;
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
            // get mask dimension
            int width = mask.Width;
            int height = mask.Height;

            // check source image size
            if ( ( width != imageData.Width ) || ( height != imageData.Height ) )
            {
                // sorry, but source image must have the same dimension as mask image
                throw new ArgumentException( "Source image has wrong dimension (does not equal to mask dimension" );
            }

            // apply first filter
            Bitmap dstImg1 = filter1.Apply( imageData );

            // check dimension of the result image
            if ( ( width != dstImg1.Width ) || ( height != dstImg1.Height ) )
            {
                dstImg1.Dispose( );
                // we are not handling such situations yet
                throw new ApplicationException( "Filters should not change image dimension" );
            }

            Bitmap dstImg2 = null;
            // apply second filter, if it was specified
            if ( filter2 != null )
            {
                dstImg2 = filter2.Apply( imageData );
                // check dimension of the result image
                if ( ( width != dstImg2.Width ) || ( height != dstImg2.Height ) )
                {
                    dstImg1.Dispose( );
                    dstImg2.Dispose( );
                    // we are not handling such situations yet
                    throw new ApplicationException( "Filters should not change image dimension" );
                }
            }

            // lock second image or get source instead of it
            BitmapData dstData2 = ( dstImg2 == null ) ? imageData :
                dstImg2.LockBits(
                    new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly,
                    ( dstImg2.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				        PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb );

            // check pixel formats
            if ( dstImg1.PixelFormat != dstData2.PixelFormat )
            {
                IFilter f = new GrayscaleToRGB( );

                // convert 1st image to RGB format
                if ( dstImg1.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    Bitmap t = f.Apply( dstImg1 );
                    dstImg1.Dispose( );
                    dstImg1 = t;
                }

                // convert 2nd image to RGB format
                if ( dstData2.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    Bitmap t = f.Apply( dstData2 );

                    // dispose temporary image
                    if ( dstImg2 != null )
                    {
                        dstImg2.UnlockBits( dstData2 );
                        dstImg2.Dispose( );
                    }

                    dstImg2 = t;
                    // lock second image again
                    dstData2 = dstImg2.LockBits(
                            new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly,
                            PixelFormat.Format24bppRgb );
                }
            }

            // lock 1st image - result image
            BitmapData dstData1 = dstImg1.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, dstImg1.PixelFormat );

            // lock mask bitmap data
            BitmapData maskData = mask.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, mask.PixelFormat );

            int pixelSize   = ( dstImg1.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int offset      = dstData1.Stride - width * pixelSize;
            int maskInc     = ( mask.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int maskOffset  = maskData.Stride - width * maskInc;

            // do the job
            unsafe
            {
                byte* dst = (byte*) dstData1.Scan0.ToPointer( );
                byte* src = (byte*) dstData2.Scan0.ToPointer( );
                byte* m = (byte*) maskData.Scan0.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, m += maskInc )
                    {
                        if ( *m != 0 )
                        {
                            for ( int i = 0; i < pixelSize; i++, src++, dst++ )
                            {
                                *dst = *src;
                            }
                        }
                        else
                        {
                            src += pixelSize;
                            dst += pixelSize;
                        }
                    }
                    src += offset;
                    dst += offset;
                    m += maskOffset;
                }
            }

            // unlock 1st image and mask
            dstImg1.UnlockBits( dstData1 );
            mask.UnlockBits( maskData );

            // dispose 2nd image
            if ( dstImg2 != null )
            {
                dstImg2.UnlockBits( dstData2 );
                dstImg2.Dispose( );
            }

            // return result image
            return dstImg1;
        }
	}
}
