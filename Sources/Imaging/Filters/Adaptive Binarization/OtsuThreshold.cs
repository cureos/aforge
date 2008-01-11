// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Otsu thresholding.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Otsu thresholding, which is described in
    /// <b>N. Otsu, "A threshold selection method from gray-level histograms", IEEE Trans. Systems,
    /// Man and Cybernetics 9(1), pp. 62–66, 1979.</b></para>
    /// <para>This implementation instead of minimizing the weighted within-class variance
    /// does maximization of between-class variance, what gives the same result. The approach is
    /// described in <a href="http://sampl.ece.ohio-state.edu/EE863/2004/ECE863-G-segclust2.ppt">this presentation</a>.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// OtsuThreshold filter = new OtsuThreshold( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// // check threshold value
    /// byte t = filter.ThresholdValue;
    /// // ...
    /// </code>
    /// </remarks>
    /// 
    public class OtsuThreshold : FilterGrayToGrayPartial
    {
        private byte threshold;

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The property is read only and represents the value, which
        /// was automaticaly calculated using Otsu algorithm.</remarks>
        /// 
        public byte ThresholdValue
        {
            get { return threshold; }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            // get start and stop X-Y coordinates
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = imageData.Stride - rect.Width;

            // histogram array
            int[] integerHistogram = new int[256];
            double[] histogram = new double[256];

            // collect histogram first
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // for each line	
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    integerHistogram[*ptr]++;
                }
                ptr += offset;
            }

            // pixels count in the processing region
            int pixelCount = ( stopX - startX ) * ( stopY - startY );
            // mean value of the processing region
            double imageMean = 0;

            for ( int i = 0; i < 256; i++ )
            {
                histogram[i] = (double) integerHistogram[i] / pixelCount;
                imageMean += histogram[i] * i; 
            }

            double max = double.MinValue;
            threshold = 0;

            // initial class probabilities
            double class1ProbabiltyInit = 0;
            double class2ProbabiltyInit = 1;

            // initial class 1 mean value
            double class1MeanInit = 0;

            // check all thresholds
            for ( int t = 0; t < 256; t++ )
            {
                // calculate class probabilities for the given threshold
                double class1Probability = class1ProbabiltyInit;
                double class2Probability = class2ProbabiltyInit;

                // calculate class means for the given threshold
                double class1Mean = class1MeanInit;
                double class2Mean = ( imageMean - ( class1Mean * class1Probability ) ) / class2Probability;

                // calculate between class variance
                double betweenClassVariance = ( class1Probability ) * ( 1.0 - class1Probability ) * Math.Pow( class1Mean - class2Mean, 2 );

                // check if we found new threshold candidate
                if ( betweenClassVariance > max )
                {
                    max = betweenClassVariance;
                    threshold = (byte) t;
                }

                // update initial probabilities and mean value
                class1MeanInit *= class1ProbabiltyInit;

                class1ProbabiltyInit += histogram[t];
                class2ProbabiltyInit -= histogram[t];

                class1MeanInit += (double) t * (double) histogram[t];
                class1MeanInit /= class1ProbabiltyInit;
            }

            // --- 2nd pass - thresholding
            ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for all pixels
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    *ptr = (byte) ( ( *ptr >= threshold ) ? 255 : 0 );
                }
                ptr += offset;
            }
        }
    }
}
