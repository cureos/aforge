// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Wood texture
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
	public class WoodTexture : ITextureGenerator
	{
        // Perlin noise function used for texture generation
		private AForge.Math.PerlinNoise	noise = new AForge.Math.PerlinNoise( 1.0 / 32, 0.05, 0.5, 8 );

        // randmom number generator
		private Random	rand = new Random( (int) DateTime.Now.Ticks );
		private int		r;

        // rings amount
		private double	rings = 12;

        /// <summary>
        /// Rings amount
        /// </summary>
        /// 
        /// <remarks>Default value is 12. Minimum value is 3</remarks>
        /// 
        public double Rings
        {
            get { return rings; }
            set { rings = Math.Max( 3, value ); }
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="WoodTexture"/> class
		/// </summary>
        /// 
		public WoodTexture( )
        {
            Reset( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoodTexture"/> class
        /// </summary>
        /// 
        /// <param name="rings">Rings amount</param>
        /// 
		public WoodTexture( double rings )
		{
			this.rings = rings;
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
			float[,]	texture = new float[height, width];
			int			w2 = width / 2;
			int			h2 = height / 2;

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					double xv = (double) ( x - w2 ) / width;
					double yv = (double) ( y - h2 ) / height;

					texture[y, x] = 
						Math.Min( 1.0f, (float)
						Math.Abs( Math.Sin( 
							( Math.Sqrt( xv * xv + yv * yv ) + noise.Function2D( x + r, y + r ) )
								* Math.PI * 2 * rings
						) )
						);
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
