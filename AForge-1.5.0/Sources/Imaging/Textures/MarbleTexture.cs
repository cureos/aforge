// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Marble texture
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
	public class MarbleTexture : ITextureGenerator
	{
        // Perlin noise function used for texture generation
        private AForge.Math.PerlinNoise noise = new AForge.Math.PerlinNoise( 1.0 / 32, 1.0, 0.65, 2 );

        // randmom number generator
        private Random rand = new Random( (int) DateTime.Now.Ticks );
		private int		r;

		private double	xPeriod = 5.0;
		private double	yPeriod = 10.0;

        /// <summary>
        /// XPeriod value
        /// </summary>
        /// 
        /// <remarks>Default value is 5. Minimum value is 2.</remarks>
        /// 
		public double XPeriod
		{
			get { return xPeriod; }
			set { xPeriod = Math.Max( 2.0, value); }
		}

        /// <summary>
        /// YPeriod value
        /// </summary>
        /// 
        /// <remarks>Default value is 10. Minimum value is 2.</remarks>
        /// 
        public double YPeriod
		{
			get { return yPeriod; }
			set { yPeriod = Math.Max( 2.0, value ); }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MarbleTexture"/> class
        /// </summary>
        /// 
		public MarbleTexture( )
        {
            Reset( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarbleTexture"/> class
        /// </summary>
        /// 
        /// <param name="xPeriod">XPeriod value</param>
        /// <param name="yPeriod">YPeriod value</param>
        /// 
		public MarbleTexture( double xPeriod, double yPeriod )
		{
			this.xPeriod = xPeriod;
			this.yPeriod = yPeriod;
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
			double		xFact = xPeriod / width;
			double		yFact = yPeriod / height;

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					texture[y, x] = 
						Math.Min( 1.0f, (float)
							Math.Abs( Math.Sin( 
								( x * xFact + y * yFact + noise.Function2D( x + r, y + r ) ) * Math.PI
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
