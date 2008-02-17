// AForge Image Processing Library
// AForge.NET framework
//
// Copyright ©
//   Mladen Prajdic  (spirit1_fe@yahoo.com),
//   Andrew Kirillov (andrew.kirillov@gmail.com)
// 2005-2008
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Flat field correction filter.
    /// </summary>
    /// 
    /// <remarks><para>The goal of flat-field correction is to remove artifacts from 2-D images that
    /// are caused by variations in the pixel-to-pixel sensitivity of the detector and/or by distortions
    /// in the optical path. The filter requires two images for the input - source image, which represents
    /// acquisition of some objects (using microscope, for example), and background image, which is taken
    /// without any objects presented. The source image is corrected using the formula: <b>src = bgMean * src / bg</b>,
    /// where <b>src</b> - source image's pixel value, <b>bg</b> - background image's pixel value, <b>bgMean</b> - mean
    /// value of background image.</para>
    /// 
    /// <para><note>If background image is not provided, then it will be automatically generated on each filter run
    /// from source image. The automatically generated background image is produced running Gaussian Blur on the
    /// original image with (sigma value is set to 5, kernel size is set to 21). Before blurring the original image
    /// is resized to 1/3 of its original size and then the result of blurring is resized back to the original size.
    /// </note></para>
    /// 
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// FlatFieldCorrection filter = new FlatFieldCorrection( bgImage );
    /// // process image
    /// filter.ApplyInPlace( sourceImage );
    /// </code>
    /// 
    /// </remarks>
    /// 
    public class FlatFieldCorrection : FilterAnyToAny
    {
        Bitmap backgroundImage = null;

        /// <summary>
        /// Background image used for flat field correction.
        /// </summary>
        public Bitmap BackgoundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFieldCorrection"/> class.
        /// </summary>
        /// 
        /// <remarks><para>This constructor does not set background image, what means that background
        /// image will be generated on the fly on each filter run. The automatically generated background
        /// image is produced running Gaussian Blur on the original image with (sigma value is set to 5,
        /// kernel size is set to 21). Before blurring the original image is resized to 1/3 of its original size
        /// and then the result of blurring is resized back to the original size.</para></remarks>
        /// 
        public FlatFieldCorrection( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFieldCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="backgroundImage">Background image used for flat field correction.</param>
        /// 
        public FlatFieldCorrection( Bitmap backgroundImage )
        {
            this.backgroundImage = backgroundImage;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// 
        /// <exception cref="ArgumentException">Background image has different size or image format.</exception>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            Bitmap bgImage = null;

            // get image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int offset = imageData.Stride - ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );
            
            // check if we have provided background
            if ( backgroundImage == null )
            {
                // resize image to 1/3 of its original size to make bluring faster
                ResizeBicubic resizeFilter = new ResizeBicubic( (int) width / 3, (int) height / 3 );
                Bitmap tempImage = resizeFilter.Apply( imageData );

                // create background image from the input image blurring it with Gaussian 5 times
                GaussianBlur blur = new GaussianBlur( 5, 21 );

                // lock temporary image for faster processing
                BitmapData tempData = tempImage.LockBits( new Rectangle( 0, 0, tempImage.Width, tempImage.Height ),
                    ImageLockMode.ReadWrite, tempImage.PixelFormat );

                blur.ApplyInPlace( tempData );
                blur.ApplyInPlace( tempData );
                blur.ApplyInPlace( tempData );
                blur.ApplyInPlace( tempData );
                blur.ApplyInPlace( tempData );

                // resize the blurred image back to original size
                resizeFilter.NewWidth  = width;
                resizeFilter.NewHeight = height;
                bgImage = resizeFilter.Apply( tempData );

                // unlock temporary image and dispose it
                tempImage.UnlockBits( tempData );
                tempImage.Dispose( );
            }
            else
            {
                // check background image
                if ( ( width != backgroundImage.Width ) || ( height != backgroundImage.Height ) || ( imageData.PixelFormat != backgroundImage.PixelFormat ) )
                {
                    throw new ArgumentException( "Source image and background images must have the same size and pixel format" );
                }

                bgImage = backgroundImage;
            }

            // lock background image
            BitmapData bgData = bgImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, imageData.PixelFormat );

            // get background image's statistics (mean value is used as correction factor)
            ImageStatistics bgStatistics = new ImageStatistics( bgData );

            byte* src = (byte*) imageData.Scan0.ToPointer( );
            byte* bg  = (byte*) bgData.Scan0.ToPointer( );

            // do the job
            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                double mean = bgStatistics.Gray.Mean;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src++, bg++ )
                    {
                        if ( *bg != 0 )
                        {
                            *src = (byte) Math.Min( mean * *src / *bg, 255 );
                        }
                    }
                    src += offset;
                    bg  += offset;
                }
            }
            else
            {
                // color image
                double meanR = bgStatistics.Red.Mean;
                double meanG = bgStatistics.Green.Mean;
                double meanB = bgStatistics.Blue.Mean;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src += 3, bg += 3 )
                    {
                        // red
                        if ( bg[RGB.R] != 0 )
                        {
                            src[RGB.R] = (byte) Math.Min( meanR * src[RGB.R] / bg[RGB.R], 255 );
                        }
                        // green
                        if ( bg[RGB.G] != 0 )
                        {
                            src[RGB.G] = (byte) Math.Min( meanG * src[RGB.G] / bg[RGB.G], 255 );
                        }
                        // blue
                        if ( bg[RGB.B] != 0 )
                        {
                            src[RGB.B] = (byte) Math.Min( meanB * src[RGB.B] / bg[RGB.B], 255 );
                        }
                    }
                    src += offset;
                    bg  += offset;
                }
            }

            bgImage.UnlockBits( bgData );
            // dispose background image if it was not set manually
            if ( backgroundImage == null )
            {
                bgImage.Dispose( );
            }
        }
    }
}
