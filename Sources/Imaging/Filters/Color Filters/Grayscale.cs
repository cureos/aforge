// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for image grayscaling.
    /// </summary>
    /// 
    /// <remarks><para>This class is the base class for image grayscaling. Other
    /// classes should inherit from this class and specify <b>RGB</b>
    /// coefficients used for color image conversion to grayscale.</para>
    /// 
    /// <para>The filter accepts 24, 32, 48 and 64 bpp color images and produces
    /// 8 (if source is 24 or 32 bpp image) or 16 (if source is 48 or 64 bpp image)
    /// bpp grayscale image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create grayscale filter (BT709)
    /// Grayscale filter = new Grayscale( 0.2125, 0.7154, 0.0721 );
    /// // apply the filter
    /// Bitmap grayImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// </remarks>
    ///
    /// <seealso cref="GrayscaleBT709"/>
    /// <seealso cref="GrayscaleRMY"/>
    /// <seealso cref="GrayscaleY"/>
    ///
    public class Grayscale : BaseFilter
    {
        // RGB coefficients for grayscale transformation
        private double cr;
        private double cg;
        private double cb;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        /// 
        /// <param name="cr">Red coefficient.</param>
        /// <param name="cg">Green coefficient.</param>
        /// <param name="cb">Blue coefficient.</param>
        /// 
        public Grayscale( double cr, double cg, double cb )
        {
            this.cr = cr;
            this.cg = cg;
            this.cb = cb;

            // initialize format translation dictionary
            formatTransalations[PixelFormat.Format24bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format32bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format48bppRgb]  = PixelFormat.Format16bppGrayScale;
            formatTransalations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
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
            // get width and height
            int width  = sourceData.Width;
            int height = sourceData.Height;
            PixelFormat srcPixelFormat = sourceData.PixelFormat;

            if (
                ( srcPixelFormat == PixelFormat.Format24bppRgb ) ||
                ( srcPixelFormat == PixelFormat.Format32bppRgb ) ||
                ( srcPixelFormat == PixelFormat.Format32bppArgb ) )
            {
                int pixelSize = ( srcPixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
                int srcOffset = sourceData.Stride - width * pixelSize;
                int dstOffset = destinationData.Stride - width;

                // do the job
                byte* src = (byte*) sourceData.ImageData.ToPointer( );
                byte* dst = (byte*) destinationData.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        *dst = (byte) ( cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B] );
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                int pixelSize = ( srcPixelFormat == PixelFormat.Format48bppRgb ) ? 3 : 4;
                int srcBase   = (int) sourceData.ImageData.ToPointer( );
                int dstBase   = (int) destinationData.ImageData.ToPointer( );
                int srcStride = sourceData.Stride;
                int dstStride = destinationData.Stride;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    ushort* src = (ushort*) ( srcBase + y * srcStride );
                    ushort* dst = (ushort*) ( dstBase + y * dstStride );

                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        *dst = (ushort) ( cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B] );
                    }
                }
            }
        }
    }
}
