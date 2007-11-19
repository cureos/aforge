// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Math;
    using AForge.Imaging.ComplexFilters;

    /// <summary>
    /// Complex imageþ
    /// </summary>
    /// 
    /// <remarks><para>The class is used to keep image represented in complex numbers sutable for Fourier
    /// transformations.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create complex image
    /// ComplexImage complexImage = ComplexImage.FromBitmap( image );
    /// // do forward Fourier transformation
    /// complexImage.ForwardFourierTransform( );
    /// // get complex image as bitmat
    /// Bitmap fourierImage = complexImage.ToBitmap( );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample3.jpg" width="256" height="256" />
    /// <para><b>Fourier image:</b></para>
    /// <img src="fourier.jpg" width="256" height="256" />
    /// </remarks>
    /// 
    public class ComplexImage : ICloneable
    {
        // image complex data
        private Complex[,] data;
        // image dimension
        private int width;
        private int height;
        // current state of the image (transformed with Fourier ot not)
        private bool fourierTransformed = false;

        /// <summary>
        /// Image width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Status of the image - Fourier transformed or not.
        /// </summary>
        /// 
        public bool FourierTransformed
        {
            get { return fourierTransformed; }
        }

        /// <summary>
        /// Complex image's data.
        /// </summary>
        /// 
        /// <remarks>Return's 2D array of [<b>height</b>, <b>width</b>] size, which keeps image's
        /// complex data.</remarks>
        /// 
        public Complex[,] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <remarks>The constractor is protected, what makes it imposible to instantiate this
        /// class directly. To create an instance of this class <see cref="FromBitmap"/> method
        /// should be used.</remarks>
        ///
        protected ComplexImage( int width, int height )
        {
            this.width  = width;
            this.height = height;
            this.data   = new Complex[height, width];
            this.fourierTransformed = false;
        }

        /// <summary>
        /// Clone the complex image.
        /// </summary>
        /// 
        /// <returns>Returns copy of the complex image.</returns>
        /// 
        public object Clone( )
        {
            // create new complex image
            ComplexImage dstImage = new ComplexImage( width, height );
            Complex[,] data = dstImage.data;

            for ( int i = 0; i < height; i++ )
            {
                for ( int j = 0; j < width; j++ )
                {
                    data[i, j] = this.data[i, j];
                }
            }

            // clone mode as well
            dstImage.fourierTransformed = fourierTransformed;

            return dstImage;
        }

        /// <summary>
        /// Create complex image from grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="srcImage">Source grayscale bitmap.</param>
        /// 
        /// <returns>Returns an instance of complex image.</returns>
        /// 
        public static ComplexImage FromBitmap( Bitmap srcImage )
        {
            // get source image size
            int width   = srcImage.Width;
            int height  = srcImage.Height;

            // check image format
            if ( srcImage.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new ArgumentException( "Source image can be graysclae (8bpp indexed) image only" );
            }

            // check image size
            if ( ( !Tools.IsPowerOf2( width ) ) || ( !Tools.IsPowerOf2( height ) ) )
            {
                throw new ArgumentException( "Image width and height should be power of 2" );
            }

            // create new complex image
            ComplexImage dstImage = new ComplexImage( width, height );
            Complex[,] data = dstImage.data;

            // lock source bitmap data
            BitmapData srcData = srcImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            int offset = srcData.Stride - width;

            // do the job
            unsafe
            {
                byte* src = (byte*) srcData.Scan0.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++ )
                    {
                        data[y, x].Re = (float) *src / 255;
                    }
                    src += offset;
                }
            }
            // unlock source images
            srcImage.UnlockBits( srcData );

            return dstImage;
        }

        /// <summary>
        /// Convert complex image to bitmap.
        /// </summary>
        /// 
        /// <returns>Returns grayscale bitmap.</returns>
        /// 
        public Bitmap ToBitmap( )
        {
            // create new image
            Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            int offset = dstData.Stride - width;
            double scale = ( fourierTransformed ) ? Math.Sqrt( width * height ) : 1;

            // do the job
            unsafe
            {
                byte* dst = (byte*) dstData.Scan0.ToPointer( );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) System.Math.Max( 0, System.Math.Min( 255, data[y, x].Magnitude * scale * 255 ) );
                    }
                    dst += offset;
                }
            }
            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Applies forward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void ForwardFourierTransform( )
        {
            if ( !fourierTransformed )
            {
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++ )
                    {
                        if ( ( ( x + y ) & 0x1 ) != 0 )
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }

                FourierTransform.FFT2( data, FourierTransform.Direction.Forward );
                fourierTransformed = true;
            }
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void BackwardFourierTransform( )
        {
            if ( fourierTransformed )
            {
                FourierTransform.FFT2( data, FourierTransform.Direction.Backward );
                fourierTransformed = false;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++ )
                    {
                        if ( ( ( x + y ) & 0x1 ) != 0 )
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs frequency filter
        /// </summary>
        /// 
        /// <param name="range">Frequency range to keep</param>
        /// 
        /// <remarks>Frequency filter zeros all values which frequencies are
        /// outside of the spefied range.</remarks>
        /// 
        [Obsolete( "Frequency filter in Complex Filters namespace should be used instead of this method." )]
        public void FrequencyFilter( IntRange range )
        {
            if ( fourierTransformed )
            {
                FrequencyFilter filter = new FrequencyFilter( range );
                filter.Apply( this );
            }
        }
    }
}
