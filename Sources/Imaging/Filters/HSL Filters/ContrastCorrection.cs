// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Contrast adjusting in HSL color space.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and adjusts
    /// pixels contrast value using luminance value of HSL color space, increasing it
    /// or decreasing by specified factor. The filters is based on <see cref="HSLLinear"/>
    /// filter, passing work to it after recalculating contrast
    /// <see cref="Factor">factor</see> to input/output ranges of the
    /// <see cref="HSLLinear"/> filter.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ContrastCorrection filter = new ContrastCorrection( 2.0f );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/contrast_correction.jpg" width="480" height="361" />
    /// </remarks>
    ///
    /// 
    public class ContrastCorrection : BaseInPlacePartialFilter
    {
        private HSLLinear baseFilter = new HSLLinear( );
        private float factor;

        /// <summary>
        /// Contrast adjusting factor.
        /// </summary>
        /// 
        /// <remarks><para>Factor which is used to adjust contrast. Factor values greater than
        /// 1 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 1 decrease contrast - decreasing variety of contrast.</para>
        /// 
        /// <para>Default value is set to <b>1.25</b>.</para></remarks>
        /// 
        public float Factor
        {
            get { return factor; }
            set
            {
                factor = Math.Max( 0.000001f, value );

                // create luminance filter
                baseFilter.InLuminance  = new Range( 0.0f, 1.0f );
                baseFilter.OutLuminance = new Range( 0.0f, 1.0f );

                if ( factor > 1 )
                {
                    baseFilter.InLuminance = new Range( 0.5f - ( 0.5f / factor ), 0.5f + ( 0.5f / factor ) );
                }
                else
                {
                    baseFilter.OutLuminance = new Range( 0.5f - ( 0.5f * factor ), 0.5f + ( 0.5f * factor ) );
                }
            }
        }

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        /// 
        public ContrastCorrection( ) : this( 1.25f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="factor">Contrast adjusting factor.</param>
        /// 
        public ContrastCorrection( float factor )
        {
            Factor = factor;

            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            baseFilter.ApplyInPlace( image, rect );
        }
    }
}
