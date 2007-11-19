// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.ComplexFilters
{
    using System;
    using AForge;
    using AForge.Math;

    /// <summary>
    /// Filtering of frequencies outside of specified range in complex Fourier
    /// transformed image.
    /// </summary>
    /// 
    /// <remarks><para>The filer keeps on only specified range of frequencies in complex
    /// Fourier transformed image. The rest of frequencies are zeroed.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create complex image
    /// ComplexImage complexImage = ComplexImage.FromBitmap( image );
    /// // do forward Fourier transformation
    /// complexImage.ForwardFourierTransform( );
    /// // create filter
    /// FrequencyFilter filter = FrequencyFilter( new IntRange( 40, 128 ) );
    /// // apply filter
    /// filter.Apply( complexImage );
    /// // do backward Fourier transformation
    /// complexImage.BackwardFourierTransform( );
    /// // get complex image as bitmat
    /// Bitmap fourierImage = complexImage.ToBitmap( );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample3.jpg" width="256" height="256" />
    /// <para><b>Fourier image:</b></para>
    /// <img src="frequency_filter.jpg" width="256" height="256" />
    /// </remarks>
    /// 
    public class FrequencyFilter : IComplexFilter
    {
        private IntRange frequencyRange = new IntRange( 0, 1024 );

        /// <summary>
        /// Range of frequencies to keep.
        /// </summary>
        /// 
        /// <remarks><para>The range specifies the range of frequencies to keep. Values is frequencies
        /// outside of this range are zeroed.</para>
        /// <para>Default value is [0, 1024].</para></remarks>
        /// 
        public IntRange FrequencyRange
        {
            get { return frequencyRange; }
            set { frequencyRange = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencyFilter"/> class.
        /// </summary>
        /// 
        public FrequencyFilter( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencyFilter"/> class.
        /// </summary>
        /// 
        /// <param name="frequencyRange">Range of frequencies to keep.</param>
        /// 
        public FrequencyFilter( IntRange frequencyRange )
        {
            this.frequencyRange = frequencyRange;
        }

        /// <summary>
        /// Apply filter to complex image.
        /// </summary>
        /// 
        /// <param name="complexImage">Complex image to apply filter to.</param>
        /// 
        public void Apply( ComplexImage complexImage )
        {
            if ( !complexImage.FourierTransformed )
            {
                throw new ArgumentException( "The source complex image should be Fourier transformed" );
            }

            // get image dimenstion
            int width   = complexImage.Width;
            int height  = complexImage.Height;

            // half of dimensions
            int hw = width >> 1;
            int hh = height >> 1;

            // min and max frequencies
            int min = frequencyRange.Min;
            int max = frequencyRange.Max;

            // complex data to process
            Complex[,] data = complexImage.Data;

            // process all data
            for ( int i = 0; i < height; i++ )
            {
                int y = i - hh;

                for ( int j = 0; j < width; j++ )
                {
                    int x = j - hw;
                    int d = (int) Math.Sqrt( x * x + y * y );

                    // filter values outside the range
                    if ( ( d > max ) || ( d < min ) )
                    {
                        data[i, j].Re = 0;
                        data[i, j].Im = 0;
                    }
                }
            }
        }
    }
}
