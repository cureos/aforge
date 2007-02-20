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
    /// Rotate image using bicubic interpolation
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class RotateBicubic : FilterRotate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// 
		public RotateBicubic( double  angle ) :
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
        public RotateBicubic( double angle, bool keepSize )
            :
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
            // coordinates of source points and cooefficiens
            double  ox, oy, dx, dy, k1, k2;
            int     ox1, oy1, ox2, oy2;
            // destination pixel values
            double r, g, b;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointer
            byte* p;

            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale
                cy = -halfNewHeight;
                for ( int y = 0; y < newHeight; y++ )
                {
                    cx = -halfNewWidth;
                    for ( int x = 0; x < newWidth; x++, dst++ )
                    {
                        // coordinates of source point
                        ox =  angleCos * cx + angleSin * cy + halfWidth;
                        oy = -angleSin * cx + angleCos * cy + halfHeight;

                        ox1 = (int) ox;
                        oy1 = (int) oy;

                        // validate source pixel's coordinates
                        if ( ( ox1 < 0 ) || ( oy1 < 0 ) || ( ox1 >= width ) || ( oy1 >= height ) )
                        {
                            // fill destination image with filler
                            *dst = fillG;
                        }
                        else
                        {
                            dx = ox - (double) ox1;
                            dy = oy - (double) oy1;

                            // initial pixel value
                            g = 0;

                            for ( int n = -1; n < 3; n++ )
                            {
                                // get Y cooefficient
                                k1 = Interpolation.BiCubicKernel( dy - (double) n );

                                oy2 = oy1 + n;
                                if ( oy2 < 0 )
                                    oy2 = 0;
                                if ( oy2 > ymax )
                                    oy2 = ymax;

                                for ( int m = -1; m < 3; m++ )
                                {
                                    // get X cooefficient
                                    k2 = k1 * Interpolation.BiCubicKernel( (double) m - dx );

                                    ox2 = ox1 + m;
                                    if ( ox2 < 0 )
                                        ox2 = 0;
                                    if ( ox2 > xmax )
                                        ox2 = xmax;

                                    g += k2 * src[oy2 * srcStride + ox2];
                                }
                            }
                            *dst = (byte) g;
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
                        // coordinates of source point
                        ox =  angleCos * cx + angleSin * cy + halfWidth;
                        oy = -angleSin * cx + angleCos * cy + halfHeight;

                        ox1 = (int) ox;
                        oy1 = (int) oy;

                        // validate source pixel's coordinates
                        if ( ( ox1 < 0 ) || ( oy1 < 0 ) || ( ox1 >= width ) || ( oy1 >= height ) )
                        {
                            // fill destination image with filler
                            dst[RGB.R] = fillR;
                            dst[RGB.G] = fillG;
                            dst[RGB.B] = fillB;
                        }
                        else
                        {
                            dx = ox - (float) ox1;
                            dy = oy - (float) oy1;

                            // initial pixel value
                            r = g = b = 0;

                            for ( int n = -1; n < 3; n++ )
                            {
                                // get Y cooefficient
                                k1 = Interpolation.BiCubicKernel( dy - (float) n );

                                oy2 = oy1 + n;
                                if ( oy2 < 0 )
                                    oy2 = 0;
                                if ( oy2 > ymax )
                                    oy2 = ymax;

                                for ( int m = -1; m < 3; m++ )
                                {
                                    // get X cooefficient
                                    k2 = k1 * Interpolation.BiCubicKernel( (float) m - dx );

                                    ox2 = ox1 + m;
                                    if ( ox2 < 0 )
                                        ox2 = 0;
                                    if ( ox2 > xmax )
                                        ox2 = xmax;

                                    // get pixel of original image
                                    p = src + oy2 * srcStride + ox2 * 3;

                                    r += k2 * p[RGB.R];
                                    g += k2 * p[RGB.G];
                                    b += k2 * p[RGB.B];
                                }
                            }
                            dst[RGB.R] = (byte) r;
                            dst[RGB.G] = (byte) g;
                            dst[RGB.B] = (byte) b;
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
