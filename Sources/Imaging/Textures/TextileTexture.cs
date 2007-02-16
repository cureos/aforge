// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Åextile texture
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
	public class TextileTexture : ITextureGenerator
	{
        // Perlin noise function used for texture generation
        private AForge.Math.PerlinNoise noise = new AForge.Math.PerlinNoise( 1.0 / 8, 1.0, 0.65, 3 );

        // randmom number generator
        private Random rand = new Random( (int) DateTime.Now.Ticks );
        private int r;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextileTexture"/> class
        /// </summary>
        /// 
		public TextileTexture( )
		{
			Reset( );
		}

        /// <summary>
        /// Generate texture
        /// </summary>
        /// 
        /// <param name="width">Texture's width</param>
        /// <param name="height">Texture's height</param>
        /// 
        /// <returns>Two dimensional array of intensities</returns>
        /// 
        /// <remarks>Generates new texture with specified dimension.</remarks>
        /// 
        public float[,] Generate( int width, int height )
		{
			float[,] texture = new float[height, width];

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					texture[y, x] = 
						Math.Max( 0.0f, Math.Min( 1.0f,
							(
								(float) Math.Sin( x + noise.Function2D( x + r, y + r ) ) +
								(float) Math.Sin( y + noise.Function2D( x + r, y + r ) )
							) * 0.25f + 0.5f
						) );

				}
			}
			return texture;
		}

        /// <summary>
        /// Reset generator
        /// </summary>
        /// 
        /// <remarks>Regenerates internal random numbers.</remarks>
        /// 
        public void Reset( )
		{
			r = rand.Next( 5000 );
		}
	}
}
