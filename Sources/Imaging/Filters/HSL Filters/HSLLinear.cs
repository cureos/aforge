// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Luminance and saturation linear correction.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and provides
    /// with the facility of luminance and saturation linear correction - mapping specified channels'
    /// input ranges to specified output ranges.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HSLLinear filter = new HSLLinear( );
    /// // configure the filter
    /// filter.InLuminance   = new DoubleRange( 0, 0.85 );
    /// filter.OutSaturation = new DoubleRange( 0.25, 1 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hsl_linear.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="LevelsLinear"/>
    /// <seealso cref="YCbCrLinear"/>
    /// 
    public class HSLLinear : BaseInPlacePartialFilter
    {
        private DoubleRange inLuminance   = new DoubleRange( 0.0, 1.0 );
        private DoubleRange inSaturation  = new DoubleRange( 0.0, 1.0 );
        private DoubleRange outLuminance  = new DoubleRange( 0.0, 1.0 );
        private DoubleRange outSaturation = new DoubleRange( 0.0, 1.0 );

        #region Public Propertis

        /// <summary>
        /// Luminance input range.
        /// </summary>
        /// 
        /// <remarks>Luminance component is measured in the range of [0, 1].</remarks>
        /// 
        public DoubleRange InLuminance
        {
            get { return inLuminance; }
            set { inLuminance = value; }
        }

        /// <summary>
        /// Luminance output range.
        /// </summary>
        /// 
        /// <remarks>Luminance component is measured in the range of [0, 1].</remarks>
        /// 
        public DoubleRange OutLuminance
        {
            get { return outLuminance; }
            set { outLuminance = value; }
        }

        /// <summary>
        /// Saturation input range.
        /// </summary>
        /// 
        /// <remarks>Saturation component is measured in the range of [0, 1].</remarks>
        /// 
        public DoubleRange InSaturation
        {
            get { return inSaturation; }
            set { inSaturation = value; }
        }

        /// <summary>
        /// Saturation output range.
        /// </summary>
        /// 
        /// <remarks>Saturation component is measured in the range of [0, 1].</remarks>
        /// 
        public DoubleRange OutSaturation
        {
            get { return outSaturation; }
            set { outSaturation = value; }
        }

        #endregion

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSLLinear"/> class.
        /// </summary>
        /// 
        public HSLLinear( )
        {
            formatTransalations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTransalations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTransalations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            RGB rgb = new RGB( );
            HSL hsl = new HSL( );

            double kl = 0, bl = 0;
            double ks = 0, bs = 0;

            // luminance line parameters
            if ( inLuminance.Max != inLuminance.Min )
            {
                kl = ( outLuminance.Max - outLuminance.Min ) / ( inLuminance.Max - inLuminance.Min );
                bl = outLuminance.Min - kl * inLuminance.Min;
            }
            // saturation line parameters
            if ( inSaturation.Max != inSaturation.Min )
            {
                ks = ( outSaturation.Max - outSaturation.Min ) / ( inSaturation.Max - inSaturation.Min );
                bs = outSaturation.Min - ks * inSaturation.Min;
            }

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    rgb.Red   = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue  = ptr[RGB.B];

                    // convert to HSL
                    AForge.Imaging.HSL.FromRGB( rgb, hsl );

                    // do luminance correction
                    if ( hsl.Luminance >= inLuminance.Max )
                        hsl.Luminance = outLuminance.Max;
                    else if ( hsl.Luminance <= inLuminance.Min )
                        hsl.Luminance = outLuminance.Min;
                    else
                        hsl.Luminance = kl * hsl.Luminance + bl;

                    // do saturation correct correction
                    if ( hsl.Saturation >= inSaturation.Max )
                        hsl.Saturation = outSaturation.Max;
                    else if ( hsl.Saturation <= inSaturation.Min )
                        hsl.Saturation = outSaturation.Min;
                    else
                        hsl.Saturation = ks * hsl.Saturation + bs;

                    // convert back to RGB
                    AForge.Imaging.HSL.ToRGB( hsl, rgb );

                    ptr[RGB.R] = rgb.Red;
                    ptr[RGB.G] = rgb.Green;
                    ptr[RGB.B] = rgb.Blue;
                }
                ptr += offset;
            }
        }
    }
}
