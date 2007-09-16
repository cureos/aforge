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
    /// Rotate image using bilinear interpolation
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class RotateBilinear : FilterRotate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle</param>
        /// 
		public RotateBilinear( double  angle ) :
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
        public RotateBilinear( double angle, bool keepSize ) :
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
            // coordinates of source points
            double  ox, oy, dx1, dy1, dx2, dy2;
            int     ox1, oy1, ox2, oy2;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointers
            byte* p1, p2, p3, p4;

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
                        // coordinates of source point
                        ox =  angleCos * cx + angleSin * cy + halfWidth;
                        oy = -angleSin * cx + angleCos * cy + halfHeight;

                        // top-left coordinate
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
                            // bottom-right coordinate
                            ox2 = ( ox1 == xmax ) ? ox1 : ox1 + 1;
                            oy2 = ( oy1 == ymax ) ? oy1 : oy1 + 1;

                            if ( ( dx1 = ox - (double) ox1 ) < 0 )
                                dx1 = 0;
                            dx2 = 1.0 - dx1;

                            if ( ( dy1 = oy - (double) oy1 ) < 0 )
                                dy1 = 0;
                            dy2 = 1.0 - dy1;

                            p1 = src + oy1 * srcStride;
                            p2 = src + oy2 * srcStride;

                            // interpolate using 4 points
                            *dst = (byte) (
                                dy2 * ( dx2 * p1[ox1] + dx1 * p1[ox2] ) +
                                dy1 * ( dx2 * p2[ox1] + dx1 * p2[ox2] ) );
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

                        // top-left coordinate
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
                            // bottom-right coordinate
                            ox2 = ( ox1 == xmax ) ? ox1 : ox1 + 1;
                            oy2 = ( oy1 == ymax ) ? oy1 : oy1 + 1;

                            if ( ( dx1 = ox - (float) ox1 ) < 0 )
                                dx1 = 0;
                            dx2 = 1.0f - dx1;

                            if ( ( dy1 = oy - (float) oy1 ) < 0 )
                                dy1 = 0;
                            dy2 = 1.0f - dy1;

                            // get four points
                            p1 = p2 = src + oy1 * srcStride;
                            p1 += ox1 * 3;
                            p2 += ox2 * 3;

                            p3 = p4 = src + oy2 * srcStride;
                            p3 += ox1 * 3;
                            p4 += ox2 * 3;

                            // interpolate using 4 points

                            // red
                            dst[RGB.R] = (byte) (
                                dy2 * ( dx2 * p1[RGB.R] + dx1 * p2[RGB.R] ) +
                                dy1 * ( dx2 * p3[RGB.R] + dx1 * p4[RGB.R] ) );

                            // green
                            dst[RGB.G] = (byte) (
                                dy2 * ( dx2 * p1[RGB.G] + dx1 * p2[RGB.G] ) +
                                dy1 * ( dx2 * p3[RGB.G] + dx1 * p4[RGB.G] ) );

                            // blue
                            dst[RGB.B] = (byte) (
                                dy2 * ( dx2 * p1[RGB.B] + dx1 * p2[RGB.B] ) +
                                dy1 * ( dx2 * p3[RGB.B] + dx1 * p4[RGB.B] ) );
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
