
// joan.charmant@gmail.com

namespace AForge.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;


    /// <summary>
    /// Block Matcher implementing the Exhaustive Search algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Exhaustive Search Block Matching Algorithm.</para>
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // Create block matcher's instance
    /// BlockMatcherES bm = new BlockMatcherES();
    /// // Process images searching for blocks matches
    /// Point[] vectors = bm.ProcessImage(sourceImage, coordinates, searchImage, relative);
    /// </code>
    /// </remarks>
    /// 
    public class BlockMatcherES : IBlockMatcher
    {
        #region Properties
        /// <summary>
        /// Search window shift from borders of block.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies a shift in all four directions, used to locate the search window.
        /// A value of n means the search window will start n pixels up and left to the top-left corner of the block to match,
        /// and end n pixels down and right to the bottom-right corner.</para>
        /// <para>Default value is set to <b>12</b>.</para>
        /// </remarks>
        public int SearchParameter
        {
            get { return searchParameter; }
            set {searchParameter = value; }
        }
        /// <summary>
        /// Size of the edge of a block.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the size of the edge of a block.
        /// The block is centered around the given point.</para>
        /// <para>Default value is set to <b>16</b>.</para>
        /// </remarks>
        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }
        #endregion

        #region Members
        // Search Parameter (maximum shift from base position, in all 4 directions) 
        private int searchParameter = 12;
        // Block Size
        private int blockSize = 16;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockMatcherES"/> class.
        /// </summary>
        public BlockMatcherES( ) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockMatcherES"/> class.
        /// </summary>
        /// 
        /// <param name="blockSize">Size of the edge of a block.</param>
        /// <param name="searchParameter">Search window shift from borders of block.</param>
        /// 
        public BlockMatcherES(int blockSize, int searchParameter)
        {
            this.blockSize = blockSize;
            this.searchParameter = searchParameter;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Process images matching blocks between images.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image with reference points.</param>
        /// <param name="coordinates">Array of reference points to be matched.</param>
        /// <param name="searchImage">Image in which the reference points will be looked for.</param>
        /// <param name="relative">True if results should be given as relative displacement, false for absolute coordinates.</param>
        /// 
        /// <returns>Returns array of relative displacements or absolute coordinates</returns>
        /// 
        /// <exception cref="ArgumentException">Source images sizes must match.</exception>
        /// <exception cref="ArgumentException">Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and overlay images must have same pixel format.</exception>
        /// 
        public Point[] ProcessImage(Bitmap sourceImage, Point[] coordinates, Bitmap searchImage, bool relative)
        {
            // Source images sizes must match.
            if ((sourceImage.Width != searchImage.Width) || (sourceImage.Height != searchImage.Height))
                throw new ArgumentException("Source images sizes must match.");

            // Sources images must be graysclae or color.
            if ((sourceImage.PixelFormat != PixelFormat.Format8bppIndexed) && (sourceImage.PixelFormat != PixelFormat.Format24bppRgb))
                throw new ArgumentException("Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only.");

            // Source images must have the same pixel format.
            if (sourceImage.PixelFormat != searchImage.PixelFormat)
                throw new ArgumentException("Source and overlay images must have same pixel format.");

            // lock source image
            BitmapData sourceImageDataBitmap = sourceImage.LockBits(
                new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                ImageLockMode.ReadOnly, sourceImage.PixelFormat);

            BitmapData searchImageDataBitmap = searchImage.LockBits(
                new Rectangle(0, 0, searchImage.Width, searchImage.Height),
                ImageLockMode.ReadOnly, searchImage.PixelFormat);

            // process the image
            Point[] matches = ProcessImage(sourceImageDataBitmap, coordinates, searchImageDataBitmap, relative);

            // unlock image
            sourceImage.UnlockBits(sourceImageDataBitmap);
            searchImage.UnlockBits(searchImageDataBitmap);

            return matches;
        }


        /// <summary>
        /// Process images matching blocks between images.
        /// </summary>
        /// 
        /// <param name="sourceImageDataBitmap">Source image with reference points.</param>
        /// <param name="coordinates">Array of reference points to be matched.</param>
        /// <param name="searchImageDataBitmap">Image in which the reference points will be looked for.</param>
        /// <param name="relative">True if result should be given as relative displacement, false for absolute coordinates.</param>
        /// 
        /// <returns>Returns array of relative displacements or absolute coordinates</returns>
        /// 
        /// <exception cref="ArgumentException">Source images sizes must match.</exception>
        /// <exception cref="ArgumentException">Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and overlay images must have same pixel format.</exception>
        /// 
        public Point[] ProcessImage(BitmapData sourceImageDataBitmap, Point[] coordinates, BitmapData searchImageDataBitmap, bool relative)
        {
            // Source images sizes must match.
            if ((sourceImageDataBitmap.Width != searchImageDataBitmap.Width) || (sourceImageDataBitmap.Height != searchImageDataBitmap.Height))
                throw new ArgumentException("Source images sizes must match");

            // Sources images must be graysclae or color.
            if ((sourceImageDataBitmap.PixelFormat != PixelFormat.Format8bppIndexed) && (sourceImageDataBitmap.PixelFormat != PixelFormat.Format24bppRgb))
                throw new ArgumentException("Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only");

            // Source images must have the same pixel format.
            if (sourceImageDataBitmap.PixelFormat != searchImageDataBitmap.PixelFormat)
                throw new ArgumentException("Source and overlay images must have same pixel format");

            
            // Output array
            Point[] matches = new Point[coordinates.Length];
            
            // Get source image size
            int width = sourceImageDataBitmap.Width;
            int height = sourceImageDataBitmap.Height;
            int stride = sourceImageDataBitmap.Stride;
            int pixelSize = (sourceImageDataBitmap.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;

            // Pre-compute some values to avoid doing it in the loops.
            int blockRadius = blockSize / 2;
            int maxCol = blockSize * pixelSize;
            int verticalOffset = stride - (blockSize * pixelSize);
            int searchWindowMaxShift = 2 * searchParameter;

            // do the job
            unsafe
            {
                byte* ptrSource = (byte*)sourceImageDataBitmap.Scan0.ToPointer();
                byte* ptrSearch = (byte*)searchImageDataBitmap.Scan0.ToPointer();

                // For each point fed
                for (int iPoint = 0; iPoint < coordinates.Length; iPoint++)
                {
                    Point refPoint = new Point(coordinates[iPoint].X, coordinates[iPoint].Y);
                    //Console.WriteLine("refPoint : {0} - X:{1}, Y:{2}", iPoint, refPoint.X, refPoint.Y); 

                    // Make sure the source block is inside the image.
                    if (
                        (refPoint.X - blockRadius < 0 || refPoint.X + blockRadius >= width) ||
                        (refPoint.Y - blockRadius < 0 || refPoint.Y + blockRadius >= height)
                        )
                    {
                        // Skip point.
                        matches[iPoint].X = refPoint.X;
                        matches[iPoint].Y = refPoint.Y;
                        continue;
                    }

                    // Search window
                    Point searchLocation = new Point(refPoint.X - blockRadius - searchParameter, refPoint.Y - blockRadius - searchParameter);
                    
                    // Output match 
                    Point bestMatch;
                    if (relative)
                    {
                        bestMatch = new Point(0, 0);
                    }
                    else
                    {
                        bestMatch = new Point(refPoint.X, refPoint.Y);
                    }
                    int minErr = int.MaxValue;


                    // Exhaustive Search Algorithm : We test each location within the search window.


                    // For each search window's row
                    for (int searchWindowRow = 0; searchWindowRow < searchWindowMaxShift; searchWindowRow++)
                    {
                        if ((searchLocation.Y + searchWindowRow < 0) || (searchLocation.Y + searchWindowRow + blockSize >= height))
                        {
                            // Skip row.
                            continue;
                        }

                        // For each search window's column
                        for (int searchWindowCol = 0; searchWindowCol < searchWindowMaxShift; searchWindowCol++)
                        {
                            // Tested block location in search image.
                            Point blockLocation = new Point(searchLocation.X + searchWindowCol, searchLocation.Y + searchWindowRow);

                            if ((blockLocation.X < 0) || (blockLocation.X + blockSize >= width))
                            {
                                // Skip column.
                                continue;
                            }

                            // Get memory location of the upper left refPoint of tested blocks.
                            byte* ptrSourceBlock = ptrSource + ((refPoint.Y - blockRadius) * stride) + ((refPoint.X - blockRadius) * pixelSize);
                            byte* ptrSearchBlock = ptrSearch + (blockLocation.Y * stride) + (blockLocation.X * pixelSize);

                            // Navigate this block, accumulating the error.
                            int err = 0;
                            for (int blockRow = 0; blockRow < blockSize; blockRow++)
                            {
                                for (int blockCol = 0; blockCol < maxCol; blockCol++, ptrSourceBlock++, ptrSearchBlock++)
                                {
                                    int diff = *ptrSourceBlock - *ptrSearchBlock;
                                    err += diff * diff;
                                }

                                // Move to next row.
                                ptrSourceBlock += verticalOffset;
                                ptrSearchBlock += verticalOffset;
                            }

                            // Check if the sum of error is mimimal.
                            if (err < minErr)
                            {
                                minErr = err;

                                // Keep best match so far.
                                if (relative)
                                {
                                    bestMatch.X = blockLocation.X + blockRadius - refPoint.X;
                                    bestMatch.Y = blockLocation.Y + blockRadius - refPoint.Y;
                                }
                                else
                                {
                                    bestMatch.X = blockLocation.X + blockRadius;
                                    bestMatch.Y = blockLocation.Y + blockRadius;
                                }
                            }
                        }
                    }

                    matches[iPoint].X = bestMatch.X;
                    matches[iPoint].Y = bestMatch.Y;   
                }
            }
            return matches;
        }
        #endregion
    }
}
