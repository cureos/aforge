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
	/// Textured filter - filter an image using texture
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
	public class TexturedFilter : IFilter
	{
        // texture generator
        private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
        // generated texture
        private float[,] texture = null;
        // two filters
		private IFilter	filter1 = null;
		private IFilter	filter2 = null;

        // filtering factor
        private double filterLevel = 1.0;
        // preservation factor
        private double preserveLevel = 0.0;

        /// <summary>
        /// Filter level value
        /// </summary>
        /// 
        /// <remarks>Filtering factor determines image fraction to filter.</remarks>
        /// 
        public double FilterLevel
        {
            get { return filterLevel; }
            set { filterLevel = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Preserve level value
        /// </summary>
        /// 
        /// <remarks>Preserving factor determines image fraction to keep from filtering.</remarks>
        /// 
        public double PreserveLevel
		{
			get { return preserveLevel; }
			set { preserveLevel = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
		}

        /// <summary>
        /// Generated texture
        /// </summary>
        /// 
        /// <remarks>Two dimensional array of texture intecities.</remarks>
        /// 
        public float[,] Texture
		{
			get { return texture; }
			set { texture = value; }
		}

        /// <summary>
        /// Texture generator
        /// </summary>
        /// 
        /// <remarks>Generator used to generate texture.</remarks>
        /// 
        public AForge.Imaging.Textures.ITextureGenerator TextureGenerator
		{
			get { return textureGenerator; }
			set { textureGenerator = value; }
		}

        /// <summary>
        /// First filter
        /// </summary>
        /// 
        public IFilter Filter1
		{
			get { return filter1; }
			set { filter1 = value; }
		}

        /// <summary>
        /// Second filter
        /// </summary>
        /// 
        public IFilter Filter2
		{
			get { return filter2; }
			set { filter2 = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class
        /// </summary>
        /// 
        /// <param name="texture">Generated texture</param>
        /// <param name="filter1">First filter</param>
        /// 
		public TexturedFilter( float[,] texture, IFilter filter1 )
		{
			this.texture = texture;
			this.filter1 = filter1;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class
        /// </summary>
        /// 
        /// <param name="texture">Generated texture</param>
        /// <param name="filter1">First filter</param>
        /// <param name="filter2">Second filter</param>
        /// 
		public TexturedFilter( float[,] texture, IFilter filter1, IFilter filter2 )
		{
			this.texture = texture;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class
        /// </summary>
        /// 
        /// <param name="generator">Texture generator</param>
        /// <param name="filter1">First filter</param>
        /// 
		public TexturedFilter( AForge.Imaging.Textures.ITextureGenerator generator, IFilter filter1 )
		{
			this.textureGenerator = generator;
			this.filter1 = filter1;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedFilter"/> class
        /// </summary>
        /// 
        /// <param name="generator">Texture generator</param>
        /// <param name="filter1">First filter</param>
        /// <param name="filter2">Second filter</param>
        /// 
		public TexturedFilter( AForge.Imaging.Textures.ITextureGenerator generator, IFilter filter1, IFilter filter2 )
		{
			this.textureGenerator = generator;
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
            // get source image dimension
            int width = imageData.Width;
            int height = imageData.Height;

            // if generator was specified, then generate a texture
            // otherwise use provided texture
            if ( textureGenerator != null )
            {
                texture = textureGenerator.Generate( width, height );
            }
            else
            {
                // check existing texture
                if ( ( texture.GetLength( 0 ) != height ) || ( texture.GetLength( 1 ) != width ) )
                {
                    // sorry, but source image must have the same dimension as texture
                    throw new ArgumentException( "Texture size does not match  image size" );
                }
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

            int pixelSize = ( dstData1.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int offset = dstData1.Stride - width * pixelSize;

			// do the job
            unsafe
            {
                byte* dst = (byte*) dstData1.Scan0.ToPointer( );
                byte* src = (byte*) dstData2.Scan0.ToPointer( );

                if ( preserveLevel != 0.0 )
                {
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++ )
                        {
                            double t = texture[y, x];

                            for ( int i = 0; i < pixelSize; i++, src++, dst++ )
                            {
                                *dst = (byte) Math.Min( 255.0f, ( preserveLevel * *src ) + ( filterLevel * *dst ) * t );
                            }
                        }
                        src += offset;
                        dst += offset;
                    }
                }
                else
                {
                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++ )
                        {
                            double t1 = texture[y, x];
                            double t2 = 1 - t1;

                            for ( int i = 0; i < pixelSize; i++, src++, dst++ )
                            {
                                *dst = (byte) Math.Min( 255.0f, t1 * *dst + t2 * *src );
                            }
                        }
                        src += offset;
                        dst += offset;
                    }
                }
            }

            // unlock 1st image and mask
            dstImg1.UnlockBits( dstData1 );

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
