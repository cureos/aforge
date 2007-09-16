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

    /// <summary>
    /// Salt and pepper noise.
    /// </summary>
    /// 
    /// <remarks><para>The filter adds random salt and pepper noise - sets
    /// maximum or minimum values to randomly selected pixels.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SaltAndPepperNoise filter = new SaltAndPepperNoise( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="salt_noise.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class SaltAndPepperNoise : FilterAnyToAnyPartial
    {
        // noise amount in percents
        private double noiseAmount = 10;

        // random number generator
        private Random rand = new Random( );

        /// <summary>
        /// Amount of noise to generate in percents.
        /// </summary>
        /// 
        public double NoiseAmount
        {
            get { return noiseAmount; }
            set { noiseAmount = Math.Max( 0, Math.Min( 100, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltAndPepperNoise"/> class.
        /// </summary>
        /// 
        public SaltAndPepperNoise( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltAndPepperNoise"/> class.
        /// </summary>
        /// 
        /// <param name="noiseAmount">Amount of noise to generate in percents.</param>
        /// 
        public SaltAndPepperNoise( double noiseAmount )
        {
            this.noiseAmount = noiseAmount;
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
            int startX  = rect.Left;
            int startY  = rect.Top;
            int width   = rect.Width;
            int height  = rect.Height;
            int stride  = imageData.Stride;

            int noisyPixels = (int) ( ( width * height * noiseAmount ) / 100 );

            // values to set
            byte[] values = new byte[2] { 0, 255 };

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int i = 0; i < noisyPixels; i++ )
                {
                    int x = startX + rand.Next( width );
                    int y = startY + rand.Next( height );

                    ptr[y * stride + x] = values[rand.Next( 2 )];
                }
            }
            else
            {
                // color image
                for ( int i = 0; i < noisyPixels; i++ )
                {
                    int x = startX + rand.Next( width );
                    int y = startY + rand.Next( height );
                    int colorPlane = rand.Next( 3 );

                    ptr[y * stride + x * 3 + colorPlane] = values[rand.Next( 2 )];
                }
            }
        }
    }
}
