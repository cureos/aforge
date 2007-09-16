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
	/// Merge two images using factors from texture
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class TexturedMerge : FilterAnyToAny
	{
        // texture generator
        private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
        // generated texture
        private float[,] texture = null;
        // overlay image
		private Bitmap overlayImage;

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
        /// Overlay image
        /// </summary>
        /// 
        public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class
        /// </summary>
        /// 
        /// <param name="texture">Generated texture</param>
        /// 
		public TexturedMerge( float[,] texture )
		{
			this.texture = texture;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class
        /// </summary>
        /// 
        /// <param name="generator">Texture generator</param>
        /// 
        public TexturedMerge( AForge.Imaging.Textures.ITextureGenerator generator )
		{
			this.textureGenerator = generator;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class
        /// </summary>
        /// 
        /// <param name="texture">Generated texture</param>
        /// <param name="overlayImage">Overlay image</param>
        /// 
		public TexturedMerge( float[,] texture, Bitmap overlayImage )
		{
			this.texture = texture;
			this.overlayImage = overlayImage;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedMerge"/> class
        /// </summary>
        /// 
        /// <param name="generator">Texture generator</param>
        /// <param name="overlayImage">Overlay image</param>
        /// 
		public TexturedMerge( AForge.Imaging.Textures.ITextureGenerator generator, Bitmap overlayImage )
		{
			this.textureGenerator = generator;
			this.overlayImage = overlayImage;
		}

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			// source image and overlay must have same pixel format
			if ( imageData.PixelFormat != overlayImage.PixelFormat )
				throw new ArgumentException( "Source and overlay images must have same pixel format" );

			// get image dimension
			int width	= imageData.Width;
			int height	= imageData.Height;

			// check overlay image size
			if ( ( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
				throw new ArgumentException( "Overlay image size should be equal to source image size" );

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, imageData.PixelFormat );

            // width and height to process
            int widthToProcess  = width;
            int heightToProcess = height;

            // if generator was specified, then generate a texture
            // otherwise use provided texture
            if ( textureGenerator != null )
            {
                texture = textureGenerator.Generate( width, height );
            }
            else
            {
                widthToProcess  = Math.Min( width, texture.GetLength( 1 ) );
                heightToProcess = Math.Min( height, texture.GetLength( 0 ) );
            }

            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int offset = imageData.Stride - widthToProcess * pixelSize;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );
            byte* ovr = (byte*) ovrData.Scan0.ToPointer( );

            // for each line
            for ( int y = 0; y < heightToProcess; y++ )
            {
                // for each pixel
                for ( int x = 0; x < widthToProcess; x++ )
                {
                    double t1 = texture[y, x];
                    double t2 = 1 - t1;

                    for ( int i = 0; i < pixelSize; i++, ptr++, ovr++ )
                    {
                        *ptr = (byte) Math.Min( 255.0f, *ptr * t1 + *ovr * t2 );
                    }
                }
                ptr += offset;
                ovr += offset;
            }

            overlayImage.UnlockBits( ovrData );
        }
	}
}
