// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;

    /// <summary>
    /// RGB components.
    /// </summary>
    /// 
    /// <remarks><para>The class encapsulates <b>RGB</b> color components.</para>
    /// <para><note><see cref="System.Drawing.Imaging.PixelFormat">PixelFormat.Format24bppRgb</see>
    /// actually means BGR format.</note></para>
    /// </remarks>
    /// 
    public class RGB
    {
        /// <summary>
        /// Index of red component.
        /// </summary>
        public const short R = 2;

        /// <summary>
        /// Index of green component.
        /// </summary>
        public const short G = 1;

        /// <summary>
        /// Index of blue component.
        /// </summary>
        public const short B = 0;

        /// <summary>
        /// Index of alpha component for ARGB images.
        /// </summary>
        public const short A = 3;

        /// <summary>
        /// Red component.
        /// </summary>
        public byte Red;

        /// <summary>
        /// Green component.
        /// </summary>
        public byte Green;

        /// <summary>
        /// Blue component.
        /// </summary>
        public byte Blue;

        /// <summary>
        /// <see cref="System.Drawing.Color">Color</see> value of the class.
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return Color.FromArgb( Red, Green, Blue ); }
            set
            {
                Red   = value.R;
                Green = value.G;
                Blue  = value.B;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        public RGB( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red component.</param>
        /// <param name="green">Green component.</param>
        /// <param name="blue">Blue component.</param>
        /// 
        public RGB( byte red, byte green, byte blue )
        {
            this.Red   = red;
            this.Green = green;
            this.Blue  = blue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        /// 
        /// <param name="color">Initialize from specified <see cref="System.Drawing.Color">color.</see></param>
        /// 
        public RGB( System.Drawing.Color color )
        {
            this.Red   = color.R;
            this.Green = color.G;
            this.Blue  = color.B;
        }
    }

    /// <summary>
    /// HSL components.
    /// </summary>
    /// 
    /// <remarks>The class encapsulates <b>HSL</b> color components.</remarks>
    /// 
    public class HSL
    {
        /// <summary>
        /// Hue component.
        /// </summary>
        /// 
        /// <remarks>Hue is measured in the range of [0, 359].</remarks>
        /// 
        public int Hue;

        /// <summary>
        /// Saturation component.
        /// </summary>
        /// 
        /// <remarks>Saturation is measured in the range of [0, 1].</remarks>
        /// 
        public double Saturation;

        /// <summary>
        /// Luminance value.
        /// </summary>
        /// 
        /// <remarks>Luminance is measured in the range of [0, 1].</remarks>
        /// 
        public double Luminance;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSL"/> class.
        /// </summary>
        public HSL( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSL"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Hue component.</param>
        /// <param name="saturation">Saturation component.</param>
        /// <param name="luminance">Luminance component.</param>
        /// 
        public HSL( int hue, double saturation, double luminance )
        {
            this.Hue        = hue;
            this.Saturation = saturation;
            this.Luminance  = luminance;
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <param name="hsl">Destination color in <b>HSL</b> color space.</param>
        /// 
        /// <remarks><para>See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
        /// for information about the algorithm to convert from RGB to HSL.</para></remarks>
        /// 
        public static void FromRGB( RGB rgb, HSL hsl )
        {
            double r = ( rgb.Red / 255.0 );
            double g = ( rgb.Green / 255.0 );
            double b = ( rgb.Blue / 255.0 );

            double min = Math.Min( Math.Min( r, g ), b );
            double max = Math.Max( Math.Max( r, g ), b );
            double delta = max - min;

            // get luminance value
            hsl.Luminance = ( max + min ) / 2;

            if ( delta == 0 )
            {
                // gray color
                hsl.Hue = 0;
                hsl.Saturation = 0.0;
            }
            else
            {
                // get saturation value
                hsl.Saturation = ( hsl.Luminance <= 0.5 ) ? ( delta / ( max + min ) ) : ( delta / ( 2 - max - min ) );

                // get hue value
                double hue;

                if ( r == max )
                {
                    hue = ( ( g - b ) / 6 ) / delta;
                }
                else if ( g == max )
                {
                    hue = ( 1.0 / 3 ) + ( ( b - r ) / 6 ) / delta; 
                }
                else
                {
                    hue = ( 2.0 / 3 ) + ( ( r - g ) / 6 ) / delta;
                }

                // correct hue if needed
                if ( hue < 0 )
                    hue += 1;
                if ( hue > 1 )
                    hue -= 1;

                hsl.Hue = (int) ( hue * 360 );
            }
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// 
        /// <returns>Returns <see cref="HSL"/> instance, which represents converted color value.</returns>
        /// 
        public static HSL FromRGB( RGB rgb )
        {
            HSL hsl = new HSL( );
            FromRGB( rgb, hsl );
            return hsl;
        }

        /// <summary>
        /// Convert from HSL to RGB color space.
        /// </summary>
        /// 
        /// <param name="hsl">Source color in <b>HSL</b> color space.</param>
        /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
        /// 
        public static void ToRGB( HSL hsl, RGB rgb )
        {
            if ( hsl.Saturation == 0 )
            {
                // gray values
                rgb.Red = rgb.Green = rgb.Blue = (byte) ( hsl.Luminance * 255 );
            }
            else
            {
                double v1, v2;
                double hue = (double) hsl.Hue / 360;

                v2 = ( hsl.Luminance < 0.5 ) ?
                    ( hsl.Luminance * ( 1 + hsl.Saturation ) ) :
                    ( ( hsl.Luminance + hsl.Saturation ) - ( hsl.Luminance * hsl.Saturation ) );
                v1 = 2 * hsl.Luminance - v2;

                rgb.Red   = (byte) ( 255 * Hue_2_RGB( v1, v2, hue + ( 1.0 / 3 ) ) );
                rgb.Green = (byte) ( 255 * Hue_2_RGB( v1, v2, hue ) );
                rgb.Blue  = (byte) ( 255 * Hue_2_RGB( v1, v2, hue - ( 1.0 / 3 ) ) );
            }
        }

        /// <summary>
        /// Convert the color to <b>RGB</b> color space.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
        /// 
        public RGB ToRGB( )
        {
            RGB rgb = new RGB( );
            ToRGB( this, rgb );
            return rgb;
        }

        #region Private members
        // HSL to RGB helper routine
        private static double Hue_2_RGB( double v1, double v2, double vH )
        {
            if ( vH < 0 )
                vH += 1;
            if ( vH > 1 )
                vH -= 1;
            if ( ( 6 * vH ) < 1 )
                return ( v1 + ( v2 - v1 ) * 6 * vH );
            if ( ( 2 * vH ) < 1 )
                return v2;
            if ( ( 3 * vH ) < 2 )
                return ( v1 + ( v2 - v1 ) * ( ( 2.0 / 3 ) - vH ) * 6 );
            return v1;
        }
        #endregion
    }

    /// <summary>
    /// YCbCr components.
    /// </summary>
    /// 
    /// <remarks>The class encapsulates <b>YCbCr</b> color components.</remarks>
    /// 
    public class YCbCr
    {
        /// <summary>
        /// Index of <b>Y</b> component.
        /// </summary>
        public const short YIndex = 0;

        /// <summary>
        /// Index of <b>Cb</b> component.
        /// </summary>
        public const short CbIndex = 1;

        /// <summary>
        /// Index of <b>Cr</b> component.
        /// </summary>
        public const short CrIndex = 2;

        /// <summary>
        /// <b>Y</b> component.
        /// </summary>
        public double Y;

        /// <summary>
        /// <b>Cb</b> component.
        /// </summary>
        public double Cb;

        /// <summary>
        /// <b>Cr</b> component.
        /// </summary>
        public double Cr;

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCr"/> class.
        /// </summary>
        public YCbCr( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="y"><b>Y</b> component.</param>
        /// <param name="cb"><b>Cb</b> component.</param>
        /// <param name="cr"><b>Cr</b> component.</param>
        /// 
        public YCbCr( double y, double cb, double cr )
        {
            this.Y  = Math.Max(  0.0, Math.Min( 1.0, y ) );
            this.Cb = Math.Max( -0.5, Math.Min( 0.5, cb ) );
            this.Cr = Math.Max( -0.5, Math.Min( 0.5, cr ) );
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification). 
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <param name="ycbcr">Destination color in <b>YCbCr</b> color space.</param>
        /// 
        public static void FromRGB( RGB rgb, YCbCr ycbcr )
        {
            double r = (double) rgb.Red / 255;
            double g = (double) rgb.Green / 255;
            double b = (double) rgb.Blue / 255;

            ycbcr.Y  =  0.2989 * r + 0.5866 * g + 0.1145 * b;
            ycbcr.Cb = -0.1687 * r - 0.3313 * g + 0.5000 * b;
            ycbcr.Cr =  0.5000 * r - 0.4184 * g - 0.0816 * b;
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification).
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// 
        /// <returns>Returns <see cref="YCbCr"/> instance, which represents converted color value.</returns>
        /// 
        public static YCbCr FromRGB( RGB rgb )
        {
            YCbCr ycbcr = new YCbCr( );
            FromRGB( rgb, ycbcr );
            return ycbcr;
        }

        /// <summary>
        /// Convert from YCbCr to RGB color space.
        /// </summary>
        /// 
        /// <param name="ycbcr">Source color in <b>YCbCr</b> color space.</param>
        /// <param name="rgb">Destination color in <b>RGB</b> color spacs.</param>
        /// 
        public static void ToRGB( YCbCr ycbcr, RGB rgb )
        {
            // don't warry about zeros. compiler will remove them
            double r = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr ) );
            double g = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr ) );
            double b = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr ) );

            rgb.Red   = (byte) ( r * 255 );
            rgb.Green = (byte) ( g * 255 );
            rgb.Blue  = (byte) ( b * 255 );
        }

        /// <summary>
        /// Convert the color to <b>RGB</b> color space.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
        /// 
        public RGB ToRGB( )
        {
            RGB rgb = new RGB( );
            ToRGB( this, rgb );
            return rgb;
        }
    }
}
