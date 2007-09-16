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
    /// Hough line.
    /// </summary>
    /// 
    /// <remarks>Represents line of Hough transform using radial coordinates.</remarks>
    /// 
    public class HoughLine : IComparable
	{
        /// <summary>
        /// Line's slope.
        /// </summary>
		public readonly double  Theta;

        /// <summary>
        /// Line's distance from image center.
        /// </summary>
		public readonly short	Radius;

        /// <summary>
        /// Line's absolute intensity.
        /// </summary>
		public readonly short	Intensity;

        /// <summary>
        /// Line's relative intensity.
        /// </summary>
        public readonly double  RelativeIntensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLine"/> class.
        /// </summary>
        /// 
        /// <param name="theta">Line's slope.</param>
        /// <param name="radius">Line's distance from image center.</param>
        /// <param name="intensity">Line's absolute intensity.</param>
        /// <param name="relativeIntensity">Line's relative intensity.</param>
        /// 
        public HoughLine( double theta, short radius, short intensity, double relativeIntensity )
		{
			Theta = theta;
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
            return ( -Intensity.CompareTo( ((HoughLine) value).Intensity ) );
        }
	}

	/// <summary>
	/// Hough line transformation.
	/// </summary>
    ///
    /// <remarks><para>Hough line transformation allows to detect lines in image.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// HoughLineTransformation lineTransform = new HoughLineTransformation( );
    /// // apply Hough line transofrm
    /// lineTransform.ProcessImage( sourceImage );
    /// Bitmap houghLineImage = lineTransform.ToBitmap( );
    /// // get lines using relative intensity
    /// HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );
    /// 
    /// foreach ( HoughLine line in lines )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
	public class HoughLineTransformation
	{
        // Hough transformation quality settings
        private int     stepsPerDegree;
        private int     houghHeight;
        private double  thetaStep;

        // precalculated Sine and Cosine values
		private double[]	sinMap;
		private double[]	cosMap;
        // Hough map
		private short[,]	houghMap;
		private short		maxMapIntensity = 0;

		private int 		localPeakRadius = 4;
        private short       minLineIntensity = 10;
        private ArrayList   lines = new ArrayList( );

        /// <summary>
        /// Steps per degree.
        /// </summary>
        /// 
        /// <remarks><para>The value defines quality of Hough transform and its ability to detect
        /// line slope precisely.</para>
        /// <para>Default value is <b>1</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int StepsPerDegree
        {
            get { return stepsPerDegree; }
            set
            {
                stepsPerDegree = Math.Max( 1, Math.Min( 10, value ) );
                houghHeight = 180 * stepsPerDegree;
                thetaStep = Math.PI / houghHeight;

                // precalculate Sine and Cosine values
                sinMap = new double[houghHeight];
                cosMap = new double[houghHeight];

                for ( int i = 0; i < houghHeight; i++ )
                {
                    sinMap[i] = Math.Sin( i * thetaStep );
                    cosMap[i] = Math.Cos( i * thetaStep );
                }
            }
        }

        /// <summary>
        /// Minimum line's intensity in Hough map to recognize a line.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a line. If a value in Hough
        /// map has lower intensity, then it is not treated as a line.</para>
        /// <para>Default value is <b>10</b>.</para></remarks>
        ///
        public short MinLineIntensity
        {
            get { return minLineIntensity; }
            set { minLineIntensity = value; }
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
        /// Found lines count.
        /// </summary>
        /// 
        public int LinesCount
        {
            get { return lines.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLineTransformation"/> class.
        /// </summary>
        /// 
        public HoughLineTransformation( )
		{
            StepsPerDegree = 1;
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
			int width       = imageData.Width;
            int height      = imageData.Height;
            int srcOffset   = imageData.Stride - width;
			int halfWidth   = width / 2;
			int halfHeight  = height / 2;
			int toWidth     = width - halfWidth;
			int toHeight    = height - halfHeight;

            // calculate Hough map's width
            int halfHoughWidth = (int) Math.Sqrt( halfWidth * halfWidth + halfHeight * halfHeight );
            int houghWidth = halfHoughWidth * 2;

			houghMap = new short[houghHeight, houghWidth];

			// do the job
			unsafe
			{
                byte* src = (byte*) imageData.Scan0.ToPointer( );

				// for each row
				for ( int y = -halfHeight; y < toHeight; y++ )
				{
					// for each pixel
					for ( int x = -halfWidth; x < toWidth; x++, src++ )
					{
						if ( *src != 0 )
						{
							// for each Theta value
                            for ( int theta = 0; theta < houghHeight; theta++ )
							{
                                int radius = (int) ( cosMap[theta] * x - sinMap[theta] * y ) + halfHoughWidth;

                                if ( ( radius < 0 ) || ( radius >= houghWidth ) )
									continue;

                                houghMap[theta, radius]++;
							}
						}
					}
					src += srcOffset;
				}
			}

			// find max value in Hough map
            maxMapIntensity = 0;
			for ( int i = 0; i < houghHeight; i++ )
			{
				for ( int j = 0; j < houghWidth; j++ )
				{
                    if ( houghMap[i, j] > maxMapIntensity )
					{
                        maxMapIntensity = houghMap[i, j];
					}
				}
			}

            CollectLines( );
		}

        /// <summary>
        /// Ñonvert Hough map to bitmap. 
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
				byte * dst = (byte *) imageData.Scan0.ToPointer( );

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, dst ++ )
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
        /// Get specified amount of lines with highest intensity.
        /// </summary>
        /// 
        /// <param name="count">Amount of lines to get.</param>
        /// 
        /// <returns>Returns arrary of most intesive lines. If there are no lines detected,
        /// <b>null</b> is returned.</returns>
        /// 
        public HoughLine[] GetMostIntensiveLines( int count )
        {
            // lines count
            int n = Math.Min( count, lines.Count );

            if ( n == 0 )
                return null;

            // result array
            HoughLine[] dst = new HoughLine[n];
            lines.CopyTo( 0, dst, 0, n );

            return dst;
        }

        /// <summary>
        /// Get lines with relative intensity higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of lines.</param>
        /// 
        /// <returns>Returns array of lines. If there are no lines detected,
        /// <b>null</b> is returned.</returns>
        /// 
        public HoughLine[] GetLinesByRelativeIntensity( double minRelativeIntensity )
        {
            int count = 0, n = lines.Count;

            while ( ( count < n ) && ( ( (HoughLine) lines[count] ).RelativeIntensity >= minRelativeIntensity ) )
                count++;

            return GetMostIntensiveLines( count );
        }


        // Collect lines with intesities greater or equal then specified
        private void CollectLines( )
		{
			int		maxTheta = houghMap.GetLength( 0 );
			int		maxRadius = houghMap.GetLength( 1 );

			short	intensity;
			bool	foundGreater;

            int     halfHoughWidth = maxRadius >> 1;

            // clean lines collection
            lines.Clear( );

			// for each Theta value
			for ( int theta = 0; theta < maxTheta; theta++ )
			{
				// for each Radius value
                for ( int radius = 0; radius < maxRadius; radius++ )
				{
					// get current value
					intensity = houghMap[theta, radius];

                    if ( intensity < minLineIntensity )
						continue;

                    foundGreater = false;

					// check neighboors
					for ( int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius; tt < ttMax; tt++ )
					{
                        // break if it is not local maximum
                        if ( foundGreater == true )
                            break;

                        int cycledTheta = tt;
                        int cycledRadius = radius;

                        // check limits
                        if ( cycledTheta < 0 )
                        {
                            cycledTheta = maxTheta + cycledTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }
                        if ( cycledTheta >= maxTheta )
                        {
                            cycledTheta -= maxTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }

                        for ( int tr = cycledRadius - localPeakRadius, trMax = cycledRadius + localPeakRadius; tr < trMax; tr++ )
						{
                            // skip out of map values
                            if ( tr < 0 )
								continue;
							if ( tr >= maxRadius )
								break;

							// compare the neighboor with current value
                            if ( houghMap[cycledTheta, tr] > intensity )
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
                        lines.Add( new HoughLine( (double) theta / stepsPerDegree, (short) ( radius - halfHoughWidth ), intensity, (double) intensity / maxMapIntensity ) );
					}
				}
			}

            lines.Sort( );
		}
	}
}
