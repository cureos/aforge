// AForge Image Processing Library
// AForge.NET framework
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
    /// Base class for image grayscaling.
    /// </summary>
    /// 
    /// <remarks><para>This class is the base class for image grayscaling. Other
    /// classes should inherit from this class and specify <b>RGB</b>
    /// coefficients used for image conversion to grayscale.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create grayscale filter (BT709)
    /// Grayscale filter = new Grayscale( 0.2125, 0.7154, 0.0721 );
    /// // apply the filter
    /// Bitmap grayImage = filter.Apply( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class Grayscale : FilterColorToGray
    {
        // RGB coefficients for grayscale transformation
        private double cr;
        private double cg;
        private double cb;

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
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcOffset = sourceData.Stride - width * 3;
            int dstOffset = destinationData.Stride - width;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, src += 3, dst++ )
                {
                    *dst = (byte) ( cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B] );
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }
    }
}
