// AForge Image Processing Library
// AForge.NET framework
//
// Copyright ©
//   Andrew Kirillov (andrew.kirillov@gmail.com),
//   Mladen Prajdic  (spirit1_fe@yahoo.com)
// 2005-2008
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Histogram equalization filter.
    /// </summary>
    ///
    /// <remarks><para>The filter does histogram equalization increasing local contrast in images. The effect
    /// of histogram equalization can be better seen on images, where pixel values have close contrast values.
    /// Through this adjustment, pixels intensities can be better distributed on the histogram. This allows for
    /// areas of lower local contrast to gain a higher contrast without affecting the global contrast.
    /// </para>
    /// 
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HistogramEqualization filter = new HistogramEqualization( );
    /// // process image
    /// filter.ApplyInPlace( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="grayscale.jpg" width="480" height="387" />
    /// <para><b>Result image:</b></para>
    /// <img src="equalized.jpg" width="480" height="387" />
    /// </remarks>
    ///
    public class HistogramEqualization : FilterAnyToAnyPartial
    {
        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int startX = rect.Left;
            int startY = rect.Top;
            int stopX  = startX + rect.Width;
            int stopY  = startY + rect.Height;
            int stride = imageData.Stride;
            int offset = stride - rect.Width * ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3 );

            int numberOfPixels = ( stopX - startX ) * ( stopY - startY );

            // check image format
            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                byte* ptr = (byte*) imageData.Scan0.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX );

                // calculate histogram
                int[] histogram = new int[256];
                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        histogram[*ptr]++;
                    }
                    ptr += offset;
                }

                // calculate new intensity levels
                byte[] equalizedHistogram = Equalize( histogram, numberOfPixels );

                // update pixels' intensities
                ptr = (byte*) imageData.Scan0.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX );

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        *ptr = equalizedHistogram[*ptr];
                    }
                    ptr += offset;
                }
            }
            else
            {
                // color image
                byte* ptr = (byte*) imageData.Scan0.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX * 3 );

                // calculate histogram
                int[] histogramR = new int[256];
                int[] histogramG = new int[256];
                int[] histogramB = new int[256];

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += 3 )
                    {
                        histogramR[ptr[RGB.R]]++;
                        histogramG[ptr[RGB.G]]++;
                        histogramB[ptr[RGB.B]]++;
                    }
                    ptr += offset;
                }

                // calculate new intensity levels
                byte[] equalizedHistogramR = Equalize( histogramR, numberOfPixels );
                byte[] equalizedHistogramG = Equalize( histogramG, numberOfPixels );
                byte[] equalizedHistogramB = Equalize( histogramB, numberOfPixels );

                // update pixels' intensities
                ptr = (byte*) imageData.Scan0.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX * 3 );

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += 3 )
                    {
                        ptr[RGB.R] = equalizedHistogramR[ptr[RGB.R]];
                        ptr[RGB.G] = equalizedHistogramG[ptr[RGB.G]];
                        ptr[RGB.B] = equalizedHistogramB[ptr[RGB.B]];
                    }
                    ptr += offset;
                }
            }
        }

        
        // Histogram 
        private byte[] Equalize( int[] histogram, long numPixel )
        {
            byte[] equalizedHistogram = new byte[256];
            float coef = 255.0f / numPixel;

            // calculate the first value
            float prev = histogram[0] * coef;
            equalizedHistogram[0] = (byte) prev;

            // calcualte the rest of values
            for ( int i = 1; i < 256; i++ )
            {
                prev += histogram[i] * coef;
                equalizedHistogram[i] = (byte) prev;
            }

            return equalizedHistogram;
        }
    }
}
