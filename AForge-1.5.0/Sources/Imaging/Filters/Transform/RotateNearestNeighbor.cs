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
    public class RotateNearestNeighbor : FilterRotate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// 
		public RotateNearestNeighbor( double  angle ) :
            base( angle )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// <param name="keepSize">Keep image size or not</param>
        /// 
        public RotateNearestNeighbor( double angle, bool keepSize ) :
            base( angle, keepSize )
		{
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
            double angleRad = -angle * Math.PI / 180;
			double angleCos = Math.Cos( angleRad );
			double angleSin = Math.Sin( angleRad );

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
