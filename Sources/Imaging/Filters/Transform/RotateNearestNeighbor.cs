// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Rotate image using nearest neighbor algorithm
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class RotateNearestNeighbor : FilterAnyToAnyNew
    {
        // rotation angle
        private double angle;
        // keep image size or not
        private bool keepSize = false;
        // fill color
        private Color fillColor = Color.FromArgb( 0, 0, 0 );

        /// <summary>
        /// Rotation angle
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set { angle = value % 360; }
        }

        /// <summary>
        /// Keep image size or not
        /// </summary>
        /// 
        /// <remarks>The property determines if source image's size will be kept
        /// or not. If the value is set to <b>false</b>, then new image will have
        /// new dimension according to rotation angle. If the valus is set to
        /// <b>true</b>, then new image will have the same size.</remarks>
        /// 
        public bool KeepSize
        {
            get { return keepSize; }
            set { keepSize = value; }
        }

        /// <summary>
        /// Fill color
        /// </summary>
        /// 
        /// <remarks>The fill color is used to fill areas of destination image,
        /// which don't have corresponsing pixels in source image.</remarks>
        /// 
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// 
		public RotateNearestNeighbor( double  angle )
		{
			this.angle = angle;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// <param name="keepSize">Keep image size or not</param>
        /// 
        public RotateNearestNeighbor( double angle, bool keepSize )
		{
			this.angle = angle;
			this.keepSize = keepSize;
		}

        /// <summary>
        /// Calculates new image size
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// 
        /// <returns>New image size</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( BitmapData sourceData )
        {
            // return same size if original image size should be kept
            if ( keepSize )
                return new Size( sourceData.Width, sourceData.Height );

            // angle's sine and cosine
            double angleRad = -angle * Math.PI / 180;
            double angleCos = Math.Cos( angleRad );
            double angleSin = Math.Sin( angleRad );

            // calculate half size
            double halfWidth    = (double) sourceData.Width / 2;
            double halfHeight   = (double) sourceData.Height / 2;

            // rotate corners
            double cx1 = halfWidth * angleCos;
            double cy1 = halfWidth * angleSin;

            double cx2 = halfWidth * angleCos - halfHeight * angleSin;
            double cy2 = halfWidth * angleSin + halfHeight * angleCos;

            double cx3 = -halfHeight * angleSin;
            double cy3 = halfHeight * angleCos;

            double cx4 = 0;
            double cy4 = 0;

            // recalculate image size
            halfWidth   = Math.Max( Math.Max( cx1, cx2 ), Math.Max( cx3, cx4 ) ) - Math.Min( Math.Min( cx1, cx2 ), Math.Min( cx3, cx4 ) );
            halfHeight  = Math.Max( Math.Max( cy1, cy2 ), Math.Max( cy3, cy4 ) ) - Math.Min( Math.Min( cy1, cy2 ), Math.Min( cy3, cy4 ) );

            return new Size( (int) ( halfWidth * 2 + 0.5 ), (int) ( halfHeight * 2 + 0.5 ) );
        }

        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // get source image size
            int     width       = sourceData.Width;
            int     height      = sourceData.Height;
            double  halfWidth   = (double) width / 2;
            double  halfHeight  = (double) height / 2;

            // get destination image size
            int     newWidth    = destinationData.Width;
            int     newHeight   = destinationData.Height;
            double  halfNewWidth    = (double) newWidth / 2;
            double  halfNewHeight   = (double) newHeight / 2;

            // angle's sine and cosine
            double  angleRad = -angle * Math.PI / 180;
			double	angleCos = Math.Cos( angleRad );
			double	angleSin = Math.Sin( angleRad );

            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride -
                ( ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? newWidth : newWidth * 3 );

            // fill values
            byte fillR = fillColor.R;
            byte fillG = fillColor.G;
            byte fillB = fillColor.B;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // destination pixel's coordinate relative to image center
            double cx, cy;
            // source pixel's coordinates
            int ox, oy;
            // temporary pointer
            byte* p;

            // check pixel format
            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale
                cy = -halfNewHeight;
                for ( int y = 0; y < newHeight; y++ )
                {
                    cx = -halfNewWidth;
                    for ( int x = 0; x < newWidth; x++, dst++ )
                    {
                        // coordinate of the nearest point
                        ox = (int) ( angleCos * cx + angleSin * cy + halfWidth );
                        oy = (int) ( -angleSin * cx + angleCos * cy + halfHeight );

                        // validate source pixel's coordinates
                        if ( ( ox < 0 ) || ( oy < 0 ) || ( ox >= width ) || ( oy >= height ) )
                        {
                            // fill destination image with filler
                            *dst = fillG;
                        }
                        else
                        {
                            // fill destination image with pixel from source image
                            *dst = src[oy * srcStride + ox];
                        }
                        cx++;
                    }
                    cy++;
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB
                cy = -halfNewHeight;
                for ( int y = 0; y < newHeight; y++ )
                {
                    cx = -halfNewWidth;
                    for ( int x = 0; x < newWidth; x++, dst += 3 )
                    {
                        // coordinate of the nearest point
                        ox = (int) ( angleCos * cx + angleSin * cy + halfWidth );
                        oy = (int) ( -angleSin * cx + angleCos * cy + halfHeight );

                        // validate source pixel's coordinates
                        if ( ( ox < 0 ) || ( oy < 0 ) || ( ox >= width ) || ( oy >= height ) )
                        {
                            // fill destination image with filler
                            dst[RGB.R] = fillR;
                            dst[RGB.G] = fillG;
                            dst[RGB.B] = fillB;
                        }
                        else
                        {
                            // fill destination image with pixel from source image
                            p = src + oy * srcStride + ox * 3;

                            dst[RGB.R] = p[RGB.R];
                            dst[RGB.G] = p[RGB.G];
                            dst[RGB.B] = p[RGB.B];
                        }
                        cx++;
                    }
                    cy++;
                    dst += dstOffset;
                }
            }
        }
    }
}
