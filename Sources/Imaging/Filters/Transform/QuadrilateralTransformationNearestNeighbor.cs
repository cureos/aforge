// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//
// Based on Olivier Thill's code from CodeGuru forums
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge;

    /// <summary>
    /// Performs quadrilateral transformation using nearest neighbor algorithm for interpolation.
    /// </summary>
    /// 
    /// <remarks><para>The class implements quadrilateral transformation algorithm, which
    /// extracts any quadrilateral from the specified source image and puts it into
    /// rectangular destination image. The class does not perform any interpolation for pixels
    /// of destination image - nearest neighbor pixels from the source image is taken.</para>
    /// 
    /// <para>The image processing filter accepts 8 grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // define quadrilateral's corners
    /// List&lt;IntPoint&gt; corners = new List&lt;IntPoint&gt;( );
    /// corners.Add( new IntPoint(  99,  99 ) );
    /// corners.Add( new IntPoint( 156,  79 ) );
    /// corners.Add( new IntPoint( 184, 126 ) );
    /// corners.Add( new IntPoint( 122, 150 ) );
    /// // create filter
    /// QuadrilateralTransformationNearestNeighbor filter =
    ///     new QuadrilateralTransformationNearestNeighbor( corners, 200, 200 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample18.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/quadrilateral_nearest.png" width="200" height="200" />
    /// </remarks>
    /// 
    /// <seealso cref="QuadrilateralTransformationBilinear"/>
    ///
    public class QuadrilateralTransformationNearestNeighbor : BaseQuadrilateralTransformationFilter
    {
        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformationNearestNeighbor"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// <param name="newWidth">Width of the new transformed image.</param>
        /// <param name="newHeight">Height of the new transformed image.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="BaseQuadrilateralTransformationFilter.AutomaticSizeCalculaton"/> to
        /// <see langword="false"/>, which means that destination image will have width and
        /// height as specified by user.</para></remarks>
        /// 
        public QuadrilateralTransformationNearestNeighbor( List<IntPoint> sourceCorners, int newWidth, int newHeight ) :
            base( sourceCorners, newWidth, newHeight )
		{
            InitFormatTranslationsTable( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrilateralTransformationNearestNeighbor"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="BaseQuadrilateralTransformationFilter.AutomaticSizeCalculaton"/> to
        /// <see langword="true"/>, which means that destination image will have width and
        /// height automatically calculated based on <see cref="BaseQuadrilateralTransformationFilter.SourceCorners"/> property.</para></remarks>
        ///
        public QuadrilateralTransformationNearestNeighbor( List<IntPoint> sourceCorners ) :
            base( sourceCorners )
        {
            InitFormatTranslationsTable( );
        }

        // Initialize table of formats' translations
        private void InitFormatTranslationsTable( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get source and destination images size
            int srcWidth  = sourceData.Width;
            int srcHeight = sourceData.Height;
            int dstWidth  = destinationData.Width;
            int dstHeight = destinationData.Height;

            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;
            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            // find equations of four quadrilateral's edges ( f(x) = k*x + b )
            double kTop,    bTop;
            double kBottom, bBottom;
            double kLeft,   bLeft;
            double kRight,  bRight;

            // top edge
            if ( sourceCorners[1].X == sourceCorners[0].X )
            {
                kTop = 0;
                bTop = sourceCorners[1].X;
            }
            else
            {
                kTop = (double) ( sourceCorners[1].Y - sourceCorners[0].Y ) /
                                ( sourceCorners[1].X - sourceCorners[0].X );
                bTop = (double) sourceCorners[0].Y - kTop * sourceCorners[0].X;
            }

            // bottom edge
            if ( sourceCorners[2].X == sourceCorners[3].X )
            {
                kBottom = 0;
                bBottom = sourceCorners[2].X;
            }
            else
            {
                kBottom = (double) ( sourceCorners[2].Y - sourceCorners[3].Y ) /
                                   ( sourceCorners[2].X - sourceCorners[3].X );
                bBottom = (double) sourceCorners[3].Y - kBottom * sourceCorners[3].X;
            }

            // left edge
            if ( sourceCorners[3].X == sourceCorners[0].X )
            {
                kLeft = 0;
                bLeft = sourceCorners[3].X;
            }
            else
            {
                kLeft = (double) ( sourceCorners[3].Y - sourceCorners[0].Y ) /
                                 ( sourceCorners[3].X - sourceCorners[0].X );
                bLeft = (double) sourceCorners[0].Y - kLeft * sourceCorners[0].X;
            }

            // right edge
            if ( sourceCorners[2].X == sourceCorners[1].X )
            {
                kRight = 0;
                bRight = sourceCorners[2].X;
            }
            else
            {
                kRight = (double) ( sourceCorners[2].Y - sourceCorners[1].Y ) /
                                  ( sourceCorners[2].X - sourceCorners[1].X );
                bRight = (double) sourceCorners[1].Y - kRight * sourceCorners[1].X;
            }

            // some precalculated values
            double leftFactor  = (double) ( sourceCorners[3].Y - sourceCorners[0].Y ) / dstHeight;
            double rightFactor = (double) ( sourceCorners[2].Y - sourceCorners[1].Y ) / dstHeight;

            int srcY0 = sourceCorners[0].Y;
            int srcY1 = sourceCorners[1].Y;

            // do the job
            byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );
            byte* baseDst = (byte*) destinationData.ImageData.ToPointer( );

            // for each line
            for ( int y = 0; y < dstHeight; y++ )
            {
                byte* dst = baseDst + dstStride * y;
                byte* p;

                // find corresponding Y on the left edge of the quadrilateral
                double yHorizLeft = leftFactor * y + srcY0;
                // find corresponding X on the left edge of the quadrilateral
                double xHorizLeft = ( kLeft == 0 ) ? bLeft : ( yHorizLeft - bLeft ) / kLeft;

                // find corresponding Y on the right edge of the quadrilateral
                double yHorizRight = rightFactor * y + srcY1;
                // find corresponding X on the left edge of the quadrilateral
                double xHorizRight = ( kRight == 0 ) ? bRight : ( yHorizRight - bRight ) / kRight;

                // find equation of the line joining points on the left and right edges
                double kHoriz, bHoriz;

                if ( xHorizLeft == xHorizRight )
                {
                    kHoriz = 0;
                    bHoriz = xHorizRight;
                }
                else
                {
                    kHoriz = ( yHorizRight - yHorizLeft ) / ( xHorizRight - xHorizLeft );
                    bHoriz = yHorizLeft - kHoriz * xHorizLeft;
                }

                double horizFactor = ( xHorizRight - xHorizLeft ) / dstWidth;

                for ( int x = 0; x < dstWidth; x++ )
                {
                    double xs = horizFactor * x + xHorizLeft;
                    double ys = kHoriz * xs + bHoriz;

                    // get pointer to the pixel in the source image
                    p = baseSrc + ( (int) ys * srcStride + (int) xs * pixelSize );
                    // copy pixel's values
                    for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                    {
                        *dst = *p;
                    }
                }
            }
        }
    }
}
