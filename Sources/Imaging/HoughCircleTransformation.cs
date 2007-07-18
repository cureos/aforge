// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Hough circle.
    /// </summary>
    /// 
    /// <remarks>Represents circle of Hough transform.</remarks>
    /// 
    public class HoughCircle : IComparable
    {
        /// <summary>
        /// Circle center's X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Circle center's Y coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Circle's radius.
        /// </summary>
        public readonly int Radius;

        /// <summary>
        /// Line's absolute intensity.
        /// </summary>
        public readonly short Intensity;

        /// <summary>
        /// Line's relative intensity.
        /// </summary>
        public readonly double RelativeIntensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughCircle"/> class.
        /// </summary>
        /// 
        /// <param name="x">Circle's X coordinate.</param>
        /// <param name="y">Circle's Y coordinate.</param>
        /// <param name="radius">Circle's radius.</param>
        /// <param name="intensity">Circle's absolute intensity.</param>
        /// <param name="relativeIntensity">Circle's relative intensity.</param>
        /// 
        public HoughCircle( int x, int y, int radius, short intensity, double relativeIntensity )
        {
            X = x;
            Y = y;
            Radius = radius;
            Intensity = intensity;
            RelativeIntensity = relativeIntensity;
        }

        /// <summary>
        /// Compare the object with another instance of this class.
        /// </summary>
        /// 
        /// <param name="value">Object to compare with.</param>
        /// 
        /// <returns><para>A signed number indicating the relative values of this instance and <b>value</b>: 1) greater than zero - 
        /// this instance is greater than <b>value</b>; 2) zero - this instance is equal to <b>value</b>;
        /// 3) greater than zero - this instance is less than <b>value</b>.</para>
        /// <para><b>Note</b>:</para> the sort order is descending.</returns>
        /// 
        public int CompareTo( object value )
        {
            return ( -Intensity.CompareTo( ( (HoughCircle) value ).Intensity ) );
        }
    }

    /// <summary>
    /// Hough circle transformation.
    /// </summary>
    ///
    /// <remarks><para>Hough circle transformation allows to detect circles in image.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// HoughCircleTransformation circleTransform = new HoughCircleTransformation( );
    /// // apply Hough circle transform
    /// circleTransform.ProcessImage( sourceImage );
    /// Bitmap houghCirlceImage = circleTransform.ToBitmap( );
    /// // get circles using relative intensity
    /// HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughCircle circle in circles )
    /// {
    ///     // ..
    /// }
    /// 
    /// </code>
    /// </remarks>
    /// 
    public class HoughCircleTransformation
    {
        // circle radius to detect
        private int radiusToDetect;

        // Hough map
        private short[,] houghMap;
        private short maxMapIntensity = 0;

        // Hough map's width and height
        private int width;
        private int height;

        private int localPeakRadius = 4;
        private short minCircleIntensity = 10;
        private ArrayList circles = new ArrayList( );

        /// <summary>
        /// Minimum circles's intensity in Hough map to recognize a circle.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a circle. If a value in Hough
        /// map has lower intensity, then it is not treated as a circle.</para>
        /// <para>Default value is <b>10</b>.</para></remarks>
        ///
        public short MinCircleIntensity
        {
            get { return minCircleIntensity; }
            set { minCircleIntensity = value; }
        }

        /// <summary>
        /// Radius for searching local peak value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
        /// if the map's value is a maximum in specified area.</para>
        /// <para>Default value is <b>4</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int LocalPeakRadius
        {
            get { return localPeakRadius; }
            set { localPeakRadius = Math.Max( 1, Math.Min( 10, value ) ); }
        }

        /// <summary>
        /// Maximum found intensity in Hough map.
        /// </summary>
        /// 
        public short MaxIntensity
        {
            get { return maxMapIntensity; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughCircleTransformation"/> class.
        /// </summary>
        /// 
        public HoughCircleTransformation( int radiusToDetect )
		{
            this.radiusToDetect = radiusToDetect;
		}

        /// <summary>
        /// Found circles count.
        /// </summary>
        /// 
        public int CirclesCount
        {
            get { return circles.Count; }
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        public void ProcessImage( Bitmap image )
        {
            // check image format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "Pixel format of source image should be 8 bpp indexed" );

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // process the image
            ProcessImage( imageData );

            // unlock image
            image.UnlockBits( imageData );
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        public void ProcessImage( BitmapData imageData )
        {
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "Pixel format of source image should be 8 bpp indexed" );

			// get source image size
			width   = imageData.Width;
            height  = imageData.Height;

            int srcOffset = imageData.Stride - width;

            // allocate Hough map of the same size like image
            houghMap = new short[height, width];

			// do the job
            unsafe
            {
                byte* src = (byte*) imageData.Scan0.ToPointer( );

				// for each row
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++ )
                    {
                        if ( *src != 0 )
                        {
                            DrawHoughCircle( x, y );
                        }
                    }
                    src += srcOffset;
                }
            }

            // find max value in Hough map
            maxMapIntensity = 0;
            for ( int i = 0; i < height; i++ )
            {
                for ( int j = 0; j < width; j++ )
                {
                    if ( houghMap[i, j] > maxMapIntensity )
                    {
                        maxMapIntensity = houghMap[i, j];
                    }
                }
            }

            CollectCircles( );
        }

        /// <summary>
        /// —onvert Hough map to bitmap. 
        /// </summary>
        /// 
        /// <returns>Returns a bitmap, which shows Hough map.</returns>
        /// 
        public Bitmap ToBitmap( )
        {
            // check if Hough transformation was made already
            if ( houghMap == null )
            {
                throw new ApplicationException( "Hough transformation was not done yet" );
            }

            int width = houghMap.GetLength( 1 );
            int height = houghMap.GetLength( 0 );

            // create new image
            Bitmap image = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            int offset = imageData.Stride - width;
            float scale = 255.0f / maxMapIntensity;

            // do the job
            unsafe
            {
                byte* dst = (byte*) imageData.Scan0.ToPointer( );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) System.Math.Min( 255, (int) ( scale * houghMap[y, x] ) );
                    }
                    dst += offset;
                }
            }

            // unlock destination images
            image.UnlockBits( imageData );

            return image;
        }

        /// <summary>
        /// Get specified amount of circles with highest intensity.
        /// </summary>
        /// 
        /// <param name="count">Amount of circles to get.</param>
        /// 
        /// <returns>Returns arrary of most intesive circles. If there are no circles detected,
        /// <b>null</b> is returned.</returns>
        /// 
        public HoughCircle[] GetMostIntensiveCircles( int count )
        {
            // lines count
            int n = Math.Min( count, circles.Count );

            if ( n == 0 )
                return null;

            // result array
            HoughCircle[] dst = new HoughCircle[n];
            circles.CopyTo( 0, dst, 0, n );

            return dst;
        }

        /// <summary>
        /// Get circles with relative intensity higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of circles.</param>
        /// 
        /// <returns>Returns array of circles. If there are no circles detected,
        /// <b>null</b> is returned.</returns>
        /// 
        public HoughCircle[] GetCirclesByRelativeIntensity( double minRelativeIntensity )
        {
            int count = 0, n = circles.Count;

            while ( ( count < n ) && ( ( (HoughCircle) circles[count] ).RelativeIntensity >= minRelativeIntensity ) )
                count++;

            return GetMostIntensiveCircles( count );
        }


        // Collect circles with intesities greater or equal then specified
        private void CollectCircles( )
        {
            short intensity;
            bool foundGreater;

            // clean circles collection
            circles.Clear( );

			// for each Y coordinate
            for ( int y = 0; y < height; y++ )
            {
                // for each X coordinate
                for ( int x = 0; x < width; x++ )
                {
                    // get current value
                    intensity = houghMap[y, x];

                    if ( intensity < minCircleIntensity )
                        continue;

                    foundGreater = false;

					// check neighboors
                    for ( int ty = y - localPeakRadius, tyMax = y + localPeakRadius; ty < tyMax; ty++ )
                    {
                        // continue if the coordinate is out of map
                        if ( ty < 0 )
                            continue;
                        // break if it is not local maximum or coordinate is out of map
                        if ( ( foundGreater == true ) || ( ty >= height ) )
                            break;

                        for ( int tx = x - localPeakRadius, txMax = x + localPeakRadius; tx < txMax; tx++ )
                        {
                            // continue or break if the coordinate is out of map
                            if ( x < 0 )
                                continue;
                            if ( x >= width )
                                break;

                            // compare the neighboor with current value
                            if ( houghMap[ty, tx] > intensity )
                            {
                                foundGreater = true;
                                break;
                            }
                        }
                    }

                    // was it local maximum ?
                    if ( !foundGreater )
                    {
                        // we have local maximum
                        circles.Add( new HoughCircle( x, y, radiusToDetect, intensity, (double) intensity / maxMapIntensity ) );
                    }
                }
            }

            circles.Sort( );
        }

        // Draw Hough circle:
        // http://www.cs.unc.edu/~mcmillan/comp136/Lecture7/circle.html
        //
        // TODO: more optimizations of circle drawing could be done.
        //
        private void DrawHoughCircle( int xCenter, int yCenter )
        {
            int x = 0;
            int y = radiusToDetect;
            int p = ( 5 - radiusToDetect * 4 ) / 4;

            SetHough—irclePoints( xCenter, yCenter, x, y );

            while ( x < y )
            {
                x++;
                if ( p < 0 )
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * ( x - y ) + 1;
                }
                SetHough—irclePoints( xCenter, yCenter, x, y );
            }
        }

        // Set circle points
        private void SetHough—irclePoints( int cx, int cy, int x, int y )
        {
            if ( x == 0 )
            {
                SetHoughPoint( cx, cy + y );
                SetHoughPoint( cx, cy - y );
                SetHoughPoint( cx + y, cy );
                SetHoughPoint( cx - y, cy );
            }
            else if ( x == y )
            {
                SetHoughPoint( cx + x, cy + y );
                SetHoughPoint( cx - x, cy + y );
                SetHoughPoint( cx + x, cy - y );
                SetHoughPoint( cx - x, cy - y );
            }
            else if ( x < y )
            {
                SetHoughPoint( cx + x, cy + y );
                SetHoughPoint( cx - x, cy + y );
                SetHoughPoint( cx + x, cy - y );
                SetHoughPoint( cx - x, cy - y );
                SetHoughPoint( cx + y, cy + x );
                SetHoughPoint( cx - y, cy + x );
                SetHoughPoint( cx + y, cy - x );
                SetHoughPoint( cx - y, cy - x );
            }
        }

        // Set point
        private void SetHoughPoint( int x, int y )
        {
            if ( ( x >= 0 ) && ( y >= 0 ) && ( x < width ) && ( y < height ) )
            {
                houghMap[y, x]++;
            }
        }
    }
}
