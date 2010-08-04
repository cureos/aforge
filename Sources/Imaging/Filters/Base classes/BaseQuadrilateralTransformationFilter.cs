// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge;

    /// <summary>
    /// Base class for quadrilateral transformation image processing filters.
    /// </summary>
    /// 
    /// <remarks>The abstract class is the base class for all image processing filters,
    /// which implement quadrilateral transformation algorithms.</remarks>
    /// 
    public abstract class BaseQuadrilateralTransformationFilter : BaseTransformationFilter
    {
        private bool automaticSizeCalculaton;

        /// <summary>
        /// New image width.
        /// </summary>
        protected int newWidth;

        /// <summary>
        /// New image height.
        /// </summary>
        protected int newHeight;

        /// <summary>
        /// Automatic calculation of destination image or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies how to calculate size of destination (transformed)
        /// image. If the property is set to <see langword="false"/>, then <see cref="NewWidth"/>
        /// and <see cref="NewHeight"/> properties have effect and destination image's size is
        /// specified by user. If the property is set to <see langword="true"/>, then setting the above
        /// mentioned properties does not have any effect, but destionation image's size is
        /// automatically calculated from <see cref="SourceCorners"/> property - width and height
        /// come from length of longest edges.
        /// </para></remarks>
        /// 
        public bool AutomaticSizeCalculaton
        {
            get { return automaticSizeCalculaton; }
            set
            {
                automaticSizeCalculaton = value;
                if ( value )
                {
                    CalculateDestinationSize( );
                }
            }
        }

        /// <summary>
        /// Quadrilateral's corners in source image.
        /// </summary>
        protected List<IntPoint> sourceCorners;

        /// <summary>
        /// Quadrilateral's corners in source image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies four corners of the quadrilateral area
        /// in the source image to be transformed.</para>
        /// </remarks>
        /// 
        public List<IntPoint> SourceCorners
        {
            get { return sourceCorners; }
            set
            {
                sourceCorners = value;
                if ( automaticSizeCalculaton )
                {
                    CalculateDestinationSize( );
                }
            }
        }

        /// <summary>
        /// Width of the new transformed image.
        /// </summary>
        /// 
        /// <remarks><para>The property defines width of the destination image, which gets
        /// transformed quadrilateral image.</para>
        /// 
        /// <para><note>Setting the property does not have any effect, if <see cref="AutomaticSizeCalculaton"/>
        /// property is set to <see langword="true"/>. In this case destination image's width
        /// is calculated automatically based on <see cref="SourceCorners"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set
            {
                if ( !automaticSizeCalculaton )
                {
                    newWidth = Math.Max( 1, value );
                }
            }
        }

        /// <summary>
        /// Height of the new transformed image.
        /// </summary>
        /// 
        /// <remarks><para>The property defines height of the destination image, which gets
        /// transformed quadrilateral image.</para>
        /// 
        /// <para><note>Setting the property does not have any effect, if <see cref="AutomaticSizeCalculaton"/>
        /// property is set to <see langword="true"/>. In this case destination image's height
        /// is calculated automatically based on <see cref="SourceCorners"/> property.</note></para>
        /// </remarks>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set
            {
                if ( !automaticSizeCalculaton )
                {
                    newHeight = Math.Max( 1, value );
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQuadrilateralTransformationFilter"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// <param name="newWidth">Width of the new transformed image.</param>
        /// <param name="newHeight">Height of the new transformed image.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="false"/>, which means that destination image will have width and
        /// height as specified by user.</para></remarks>
        /// 
        protected BaseQuadrilateralTransformationFilter( List<IntPoint> sourceCorners, int newWidth, int newHeight )
        {
            this.automaticSizeCalculaton = false;
            this.sourceCorners = sourceCorners;
            this.newWidth  = newWidth;
            this.newHeight = newHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQuadrilateralTransformationFilter"/> class.
        /// </summary>
        /// 
        /// <param name="sourceCorners">Corners of the source quadrilateral area.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="AutomaticSizeCalculaton"/> to
        /// <see langword="true"/>, which means that destination image will have width and
        /// height automatically calculated based on <see cref="SourceCorners"/> property.</para></remarks>
        ///
        protected BaseQuadrilateralTransformationFilter( List<IntPoint> sourceCorners )
        {
            this.automaticSizeCalculaton = true;
            this.sourceCorners = sourceCorners;
            CalculateDestinationSize( );
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        /// <exception cref="ArgumentException">The specified quadrilateral's corners are outside of the given image.</exception>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            // perform checking of source corners - they must feet into the image
            foreach ( IntPoint point in sourceCorners )
            {
                if ( ( point.X < 0 ) ||
                     ( point.Y < 0 ) ||
                     ( point.X >= sourceData.Width ) ||
                     ( point.Y >= sourceData.Height ) )
                {
                    throw new ArgumentException( "The specified quadrilateral's corners are outside of the given image." );
                }
            }

            return new Size( newWidth, newHeight );
        }

        // Calculates size of destination image
        private void CalculateDestinationSize( )
        {
            int maxXdiff = Math.Abs( sourceCorners[3].X - sourceCorners[0].X ); ;
            int maxYdiff = Math.Abs( sourceCorners[3].Y - sourceCorners[0].Y );

            for ( int i = 1; i < 4; i++ )
            {
                int xDiff = Math.Abs( sourceCorners[i - 1].X - sourceCorners[i].X );
                int yDiff = Math.Abs( sourceCorners[i - 1].Y - sourceCorners[i].Y );

                if ( xDiff > maxXdiff )
                {
                    maxXdiff = xDiff;
                }
                if ( yDiff > maxYdiff )
                {
                    maxYdiff = yDiff;
                }
            }

            newWidth  = maxXdiff;
            newHeight = maxYdiff;
        }
    }
}
