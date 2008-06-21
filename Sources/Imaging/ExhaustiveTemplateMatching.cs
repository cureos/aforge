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
    /// Exhaustive template matching.
    /// </summary>
    /// 
    /// <remarks><para>The class implements exhaustive template matching algorithm,
    /// which performs complete scan of source image, comparing each pixel with corresponding
    /// pixel of template.</para>
    /// 
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create template matching algorithm's instance
    /// ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching( 0.9 );
    /// // find all matchings with specified above similarity
    /// Matching[] matchings = tm.ProcessImage( sourceImage, templateImage );
    /// // highlight found matchings
    /// BitmapData data = sourceImage.LockBits(
    ///     new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
    ///     ImageLockMode.ReadWrite, sourceImage.PixelFormat );
    /// foreach ( Matching m in matchings )
    /// {
    ///     Drawing.Rectangle( data, m.Rectangle, Color.White );
    ///     // do something else with matching
    /// }
    /// sourceImage.UnlockBits( data );
    /// </code>
    /// 
    /// <para>The class also can be used to get similarity level between two image of the same
    /// size, which can be useful to get information how different/similar are images:</para>
    /// <code>
    /// // compare two images
    /// Matching[] matchings = tm.ProcessImage( image1, image2 );
    /// // check similarity level
    /// if ( matchings[0].Similarity > 0.95 )
    /// {
    ///     // do something with quite similar images
    /// }
    /// </code>
    /// 
    /// </remarks>
    /// 
    public class ExhaustiveTemplateMatching : ITemplateMatching
    {
        private float similarityThreshold = 0.9f;

        /// <summary>
        /// Similarity threshold, [0..1].
        /// </summary>
        /// 
        /// <remarks><para>The property sets the minimal similarity between template
        /// and potential found candidate. If similarity is lower than this value,
        /// then object is not treated as matching with template.
        /// </para>
        /// <para>Default value is set to 0.9.</para>
        /// </remarks>
        /// 
        public float SimilarityThreshold
        {
            get { return similarityThreshold; }
            set { similarityThreshold = Math.Min( 1, Math.Max( 0, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matching"/> class.
        /// </summary>
        /// 
        /// <param name="similarityThreshold">Similarity threshold.</param>
        /// 
        public ExhaustiveTemplateMatching( float similarityThreshold )
        {
            this.similarityThreshold = similarityThreshold;
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="template">Template image to search for.</param>
        /// 
        /// <returns>Returns array of found matchings. The array is sorted by similarity
        /// of found matchings in descending order.</returns>
        /// 
        public Matching[] ProcessImage( Bitmap image, Bitmap template )
        {
            // check image format
            if (
                ( ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                  ( image.PixelFormat != PixelFormat.Format24bppRgb ) ) ||
                ( image.PixelFormat != template.PixelFormat ) )
            {
                throw new ArgumentException( "Source and template images can be grayscale (8 bpp indexed) or color (24 bpp) images only ans should have the same pixel format" );
            }

            // check template's size
            if ( ( template.Width > image.Width ) || ( template.Height > image.Height ) )
            {
                throw new ArgumentException( "Template image should be smaller than source image" );
            }

            // lock source and template images
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );
            BitmapData templateData = template.LockBits(
                new Rectangle( 0, 0, template.Width, template.Height ),
                ImageLockMode.ReadOnly, template.PixelFormat );

            // process the image
            Matching[] matchings = ProcessImage( imageData, templateData );

            // unlock images
            image.UnlockBits( imageData );
            template.UnlockBits( templateData );

            return matchings;
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// <param name="templateData">Template image to search for.</param>
        /// 
        /// <returns>Returns array of found matchings. The array is sorted by similarity
        /// of found matchings in descending order.</returns>
        /// 
        public Matching[] ProcessImage( BitmapData imageData, BitmapData templateData )
        {
            // check image format
            if (
                ( ( imageData.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                  ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) ) ||
                ( imageData.PixelFormat != templateData.PixelFormat ) )
            {
                throw new ArgumentException( "Source and template images can be grayscale (8 bpp indexed) or color (24 bpp) images only ans should have the same pixel format" );
            }

            // get source and template image size
            int sourceWidth = imageData.Width;
            int sourceHeight = imageData.Height;
            int templateWidth = templateData.Width;
            int templateHeight = templateData.Height;

            // check template's size
            if ( ( templateWidth > sourceWidth ) || ( templateHeight > sourceHeight ) )
            {
                throw new ArgumentException( "Template image should be smaller than source image" );
            }

            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int sourceStride = imageData.Stride;

            // similarity map. its size is increased by 4 from each side to increase
            // performance of non-maximum suppresion
            int mapWidth  = sourceWidth  - templateWidth  + 1;
            int mapHeight = sourceHeight - templateHeight + 1;
            int[,] map = new int[mapHeight + 4, mapWidth + 4];

            // maximum possible difference with template
            int max = templateWidth * templateHeight * pixelSize * 255;

            // do the job
            unsafe
            {
                byte* baseSrc = (byte*) imageData.Scan0.ToPointer( );
                byte* baseTpl = (byte*) templateData.Scan0.ToPointer( );

                int sourceOffset = imageData.Stride - templateWidth * pixelSize;
                int templateOffset = templateData.Stride - templateWidth * pixelSize;

                // for each row of the source image
                for ( int y = 0; y < mapHeight; y++ )
                {
                    // for each pixel of the source image
                    for ( int x = 0; x < mapWidth; x++ )
                    {
                        byte* src = baseSrc + sourceStride * y + pixelSize * x;
                        byte* tpl = baseTpl;

                        // compare template with source image starting from current X,Y
                        int dif = 0;

                        // for each row of the template
                        for ( int i = 0; i < templateHeight; i++ )
                        {
                            // for each pixel of the template
                            for ( int j = 0, maxJ = templateWidth * pixelSize; j < maxJ; j++, src++, tpl++ )
                            {
                                int d = *src - *tpl;
                                if ( d > 0 )
                                {
                                    dif += d;
                                }
                                else
                                {
                                    dif -= d;
                                }
                            }
                            src += sourceOffset;
                            tpl += templateOffset;
                        }

                        map[y + 2, x + 2] = max - dif;
                    }
                }
            }

            // collect interesting points - only those points, which are local maximums
            List<Matching> matchingsList = new List<Matching>( );

            // for each row
            for ( int y = 2, maxY = mapHeight + 2; y < maxY; y++ )
            {
                // for each pixel
                for ( int x = 2, maxX = mapWidth + 2; x < maxX; x++ )
                {
                    int currentValue = map[y, x];

                    // for each windows' row
                    for ( int i = -2; ( currentValue != 0 ) && ( i <= 2 ); i++ )
                    {
                        // for each windows' pixel
                        for ( int j = -2; j <= 2; j++ )
                        {
                            if ( map[y + i, x + j] > currentValue )
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if ( currentValue != 0 )
                    {
                        float similarity = (float) currentValue / max;

                        // check its similarity passes threshold
                        if ( similarity >= similarityThreshold )
                        {
                            matchingsList.Add( new Matching(
                                new Rectangle( x - 2, y - 2, templateWidth, templateHeight ),
                                similarity ) );
                        }
                    }
                }
            }

            // convert list to array
            Matching[] matchings = new Matching[matchingsList.Count];
            matchingsList.CopyTo( matchings );
            // sort in descending order
            Array.Sort( matchings, new MatchingsSorter( ) );

            return matchings;
        }

        // Sorter of found matchings
        private class MatchingsSorter : System.Collections.IComparer
        {
            public int Compare( Object x, Object y )
            {
                float diff = ( (Matching) y ).Similarity - ( (Matching) x ).Similarity;

                return ( diff > 0 ) ? 1 : ( diff < 0 ) ? -1 : 0;
            }
        }
    }
}
