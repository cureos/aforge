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
    using AForge.Math.Random;

    /// <summary>
    /// Additive noise filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter adds random value to each pixel of the source image.
    /// The distribution of random values can be specified by <see cref="Generator">random generator</see>.
    /// </para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create random generator
    /// IRandomNumberGenerator generator = new UniformGenerator( new DoubleRange( -50, 50 ) );
    /// // create filter
    /// AdditiveNoise filter = new AdditiveNoise( generator );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="additive_noise.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class AdditiveNoise : FilterAnyToAnyPartial
    {
        // random number generator to add noise
        IRandomNumberGenerator generator = new UniformGenerator( new DoubleRange( -10, 10 ) );

        /// <summary>
        /// Random number genertor used to add noise.
        /// </summary>
        /// 
        /// <remarks>Default generator is uniform generator in the range of (-10, 10).</remarks>
        /// 
        public IRandomNumberGenerator Generator
        {
            get { return generator; }
            set { generator = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveNoise"/> class.
        /// </summary>
        /// 
        public AdditiveNoise( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveNoise"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Random number genertor used to add noise.</param>
        /// 
        public AdditiveNoise( IRandomNumberGenerator generator )
        {
            this.generator = generator;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            int startY  = rect.Top;
            int stopY   = startY + rect.Height;

            int startX  = rect.Left * pixelSize;
            int stopX   = startX + rect.Width * pixelSize;

            int offset  = imageData.Stride - ( stopX - startX );

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + rect.Left * pixelSize );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    *ptr = (byte) Math.Max( 0, Math.Min( 255, *ptr + generator.Next( ) ) );
                }
                ptr += offset;
            }
        }
    }
}
