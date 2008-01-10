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
    using System.Drawing.Imaging;

    /// <summary>
    /// Integral image.
    /// </summary>
    /// 
    /// <remarks><para>The class implements integral image concept, which is described by
    /// Viola and Jones in: <b>P. Viola and M. J. Jones, "Robust real-time face detection",
    /// Int. Journal of Computer Vision 57(2), pp. 137–154, 2004</b>.</para>
    /// <para><i>"An integral image <b>I</b> of an input image <b>G</b> is defined as the image in which the
    /// intensity at a pixel position is equal to the sum of the intensities of all the pixels
    /// above and to the left of that position in the original image."</i></para>
    /// <para>So the intensity at position (x, y) can be written as:</para>
    /// <code>
    ///           x    y
    /// I(x,y) = SUM( SUM( G(i,j) ) )
    ///          i=0  j=0
    /// </code>
    /// <para><note>The class uses 32-bit integers to represent integral image.</note></para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create integral image
    /// IntegralImage im = IntegralImage.FromBitmap( image );
    /// </code>
    /// </remarks>
    /// 
    public class IntegralImage
    {
        /// <summary>
        /// Intergral image's array.
        /// </summary>
        /// 
        /// <remarks>See remarks to <see cref="InternalData"/> property.</remarks>
        /// 
        protected uint[,] integralImage = null;

        // image's width and height
        private int width;
        private int height;

        /// <summary>
        /// Provides access to internal array keeping integral image data.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>The array should be accessed by [y, x] indexing.</note></para>
        /// <para><note>The array's size is [height+1, width+1]. The first row and column are filled with
        /// zeros, what is done for more efficient calculation of rectangles' sums.</note></para>
        /// </remarks>
        /// 
        public uint[,] InternalData
        {
            get { return integralImage; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegralImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <remarks>The constractor is protected, what makes it imposible to instantiate this
        /// class directly. To create an instance of this class <see cref="FromBitmap(Bitmap)"/> or
        /// <see cref="FromBitmap(BitmapData)"/> method should be used.</remarks>
        ///
        protected IntegralImage( int width, int height )
        {
            this.width  = width;
            this.height = height;
            integralImage = new uint[height + 1, width + 1];
        }

        /// <summary>
        /// Construct integral image from source grayscale image.
        /// </summary>
        /// 
        /// <param name="image">Source grayscale image (8 bpp indexed).</param>
        /// 
        /// <returns>Returns integral image.</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public static IntegralImage FromBitmap( Bitmap image )
        {
            // check image format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new ArgumentException( "Source image can be graysclae (8bpp indexed) image only" );
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // process the image
            IntegralImage im = FromBitmap( imageData );

            // unlock image
            image.UnlockBits( imageData );

            return im;
        }

        /// <summary>
        /// Construct integral image from source grayscale image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data (8 bpp indexed).</param>
        /// 
        /// <returns>Returns integral image.</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public static IntegralImage FromBitmap( BitmapData imageData )
        {
            // check image format
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new ArgumentException( "Source image can be graysclae (8bpp indexed) image only" );
            }

            // get source image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int offset = imageData.Stride - width;

            // create integral image
            IntegralImage im = new IntegralImage( width, height );
            uint[,] integralImage = im.integralImage;

            // do the job
            unsafe
            {
                byte* src = (byte*) imageData.Scan0.ToPointer( );

                // for each line
                for ( int y = 1; y <= height; y++ )
                {
                    // for each pixel
                    for ( int x = 1; x <= width; x++, src++ )
                    {
                        integralImage[y, x] = *src + integralImage[y, x - 1] + integralImage[y - 1, x] - integralImage[y - 1, x - 1];
                    }
                    src += offset;
                }
            }

            return im;
        }

        /// <summary>
        /// Calculate sum of pixels in the specified rectangle.
        /// </summary>
        /// 
        /// <param name="x1">X coordinate of left-top rectangle's corner.</param>
        /// <param name="y1">Y coordinate of left-top rectangle's corner.</param>
        /// <param name="x2">X coordinate of right-bottom rectangle's corner.</param>
        /// <param name="y2">Y coordinate of right-bottom rectangle's corner.</param>
        /// 
        /// <returns>Returns sum of pixels in the specified rectangle.</returns>
        /// 
        /// <remarks>Both specified points are included into the rectangle calculation rectangle.</remarks>
        /// 
        public uint GetRectangleSum( int x1, int y1, int x2, int y2 )
        {
            if ( x1 < 0 ) x1 = 0;
            if ( y1 < 0 ) y1 = 0;

            x2++;
            y2++;

            if ( x2 > width )  x2 = width;
            if ( y2 > height ) y2 = height;

            return integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2];
        }

        /// <summary>
        /// Calculate sum of pixels in the specified rectangle.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of central point of the rectangle.</param>
        /// <param name="y">Y coordinate of central point of the rectangle.</param>
        /// <param name="radius">Radius of the rectangle.</param>
        /// 
        /// <returns>Returns sum of pixels in the specified rectangle.</returns>
        /// 
        /// <remarks>The method calculates sum of pixels in square rectangle with
        /// odd width and height. In the case if it is required to calculate sum of
        /// 3x3 rectangle, then it is required to specify its center and radius equal to 1.
        /// </remarks>
        /// 
        public uint GetRectangleSum( int x, int y, int radius )
        {
            return GetRectangleSum( x - radius, y - radius, x + radius, y + radius );
        }

        /// <summary>
        /// Calculate mean value of pixels in the specified rectangle.
        /// </summary>
        /// 
        /// <param name="x1">X coordinate of left-top rectangle's corner.</param>
        /// <param name="y1">Y coordinate of left-top rectangle's corner.</param>
        /// <param name="x2">X coordinate of right-bottom rectangle's corner.</param>
        /// <param name="y2">Y coordinate of right-bottom rectangle's corner.</param>
        /// 
        /// <returns>Returns mean value of pixels in the specified rectangle.</returns>
        /// 
        /// <remarks>Both specified points are included into the rectangle calculation rectangle.</remarks>
        /// 
        public float GetRectangleMean( int x1, int y1, int x2, int y2 )
        {
            if ( x1 < 0 ) x1 = 0;
            if ( y1 < 0 ) y1 = 0;

            x2++;
            y2++;

            if ( x2 > width )  x2 = width;
            if ( y2 > height ) y2 = height;

            // return sum divided by actual rectangles size
            return (float) ( (double) ( integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2] ) /
                (double) ( ( x2 - x1 ) * ( y2 - y1 ) ) );
        }

        /// <summary>
        /// CCalculate mean value of pixels in the specified rectangle.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of central point of the rectangle.</param>
        /// <param name="y">Y coordinate of central point of the rectangle.</param>
        /// <param name="radius">Radius of the rectangle.</param>
        /// 
        /// <returns>Returns mean value of pixels in the specified rectangle.</returns>
        /// 
        /// <remarks>The method calculates mean value of pixels in square rectangle with
        /// odd width and height. In the case if it is required to calculate mean value of
        /// 3x3 rectangle, then it is required to specify its center and radius equal to 1.
        /// </remarks>
        /// 
        public float GetRectangleMean( int x, int y, int radius )
        {
            return GetRectangleMean( x - radius, y - radius, x + radius, y + radius );
        }
    }
}
