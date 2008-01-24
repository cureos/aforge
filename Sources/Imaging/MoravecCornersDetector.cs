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
    using System.Collections.Generic;

    /// <summary>
    /// Moravec corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Moravec corners detector. For information about algorithm's
    /// details its <a href="http://www.cim.mcgill.ca/~dparks/CornerDetector/mainMoravec.htm">description</a>
    /// should be studied.</para>
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corner detector's instance
    /// MoravecCornersDetector mcd = new MoravecCornersDetector( );
    /// // process image searching for corners
    /// Point[] corners = mcd.ProcessImage( image );
    /// // process points
    /// foreach ( Point corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class MoravecCornersDetector : ICornersDetector
    {
        // window size
        private int windowSize = 3;
        // threshold which is used to filter interest points
        private int threshold = 500;

        /// <summary>
        /// Window size used to determine if point is interesting.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies window size, which is used for initial searching of
        /// corners candidates and then for searching local maximums.</para>
        /// <para>Default value is set to <b>3</b>. Value’s acceptable range is [3, 15].</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Setting value is not odd.</exception>
        /// 
        public int WindowSize
        {
            get { return windowSize; }
            set
            {
                // check if value is odd
                if ( ( value & 1 ) == 0 )
                    throw new ArgumentException( "The value shoule be odd" );

                windowSize = Math.Max( 3, Math.Min( 15, value ) );
            }
        }

        /// <summary>
        /// Threshold value, which is used to filter out uninteresting points.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to filter uninteresting points - points which have value below
        /// specified threshold value are treated as not corners candidates.</para>
        /// <para>Default value is set to <b>500</b>.</para>
        /// </remarks>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        private static int[] xDelta = new int[8] { -1,  0,  1, 1, 1, 0, -1, -1 };
        private static int[] yDelta = new int[8] { -1, -1, -1, 0, 1, 1,  1,  0 };

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        public MoravecCornersDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value, which is used to filter out uninteresting points.</param>
        /// 
        public MoravecCornersDetector( int threshold ) :
            this( threshold, 3 ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value, which is used to filter out uninteresting points.</param>
        /// <param name="windowSize">Window size used to determine if point is interesting.</param>
        /// 
        public MoravecCornersDetector( int threshold, int windowSize )
        {
            this.Threshold = threshold;
            this.WindowSize = windowSize;
        }


        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public Point[] ProcessImage( Bitmap image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb )
                )
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            // process the image
            Point[] corners = ProcessImage( imageData );

            // unlock image
            image.UnlockBits( imageData );

            return corners;
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public Point[] ProcessImage( BitmapData imageData )
        {
            // check image format
            if (
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb )
                )
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

            // get source image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int stride = imageData.Stride;
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            // window radius
            int windowRadius = windowSize / 2;

            // offset
            int offset = stride - windowSize * pixelSize;

            // create moravec cornerness map
            int[,] moravecMap = new int[height, width];

            // do the job
            unsafe
            {
                byte* ptr = (byte*) imageData.Scan0.ToPointer( );

			    // for each row
                for ( int y = windowRadius, maxY = height - windowRadius; y < maxY; y++ )
                {
                    // for each pixel
                    for ( int x = windowRadius, maxX = width - windowRadius; x < maxX; x++ )
                    {
                        int minSum = int.MaxValue;

                        // go through 8 possible shifting directions
                        for ( int k = 0; k < 8; k++ )
                        {
                            // calculate center of shifted window
                            int sy = y + yDelta[k];
                            int sx = x + xDelta[k];

                            // check if shifted window is within the image
                            if (
                                ( sy < windowRadius ) || ( sy >= maxY ) ||
                                ( sx < windowRadius ) || ( sx >= maxX )
                            )
                            {
                                // skip this shifted window
                                continue;
                            }

                            int sum = 0;

                            byte* ptr1 = ptr + ( y  - windowRadius ) * stride + (  x - windowRadius ) * pixelSize;
                            byte* ptr2 = ptr + ( sy - windowRadius ) * stride + ( sx - windowRadius ) * pixelSize;

                            // for each windows' rows
                            for ( int i = 0; i < windowSize; i++ )
                            {
                                // for each windows' pixels
                                for ( int j = 0, maxJ = windowSize * pixelSize; j < maxJ; j++, ptr1++, ptr2++ )
                                {
                                    int dif = *ptr1 - *ptr2;
                                    sum += dif * dif;
                                }
                                ptr1 += offset;
                                ptr2 += offset;
                            }

                            // check if the sum is mimimal
                            if ( sum < minSum )
                            {
                                minSum = sum;
                            }
                        }

                        // threshold the minimum sum
                        if ( minSum < threshold )
                        {
                            minSum = 0;
                        }

                        moravecMap[y, x] = minSum;
                    }
                }
            }

            // collect interesting points - only those points, which are local maximums
            List<Point> cornersList = new List<Point>( );

            // for each row
            for ( int y = windowRadius, maxY = height - windowRadius; y < maxY; y++ )
            {
                // for each pixel
                for ( int x = windowRadius, maxX = width - windowRadius; x < maxX; x++ )
                {
                    int currentValue = moravecMap[y, x];

                    // for each windows' rows
                    for ( int i = -windowRadius; ( currentValue != 0 ) && ( i <= windowRadius ); i++ )
                    {
                        // for each windows' pixels
                        for ( int j = -windowRadius; j <= windowRadius; j++ )
                        {
                            if ( moravecMap[y + i, x + j] > currentValue )
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if ( currentValue != 0 )
                    {
                        cornersList.Add( new Point( x, y ) );
                    }
                }
            }

            // convert list to array
            Point[] corners = new Point[cornersList.Count];
            cornersList.CopyTo( corners );

            return corners;
        }
    }
}
