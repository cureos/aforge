using System;
using System.Collections.Generic;
using System.Text;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge;
using System.Drawing.Imaging;
using AForge.Math;
using System.Drawing;

namespace BMIPA
{
    public class HistogramEquilization : FilterGrayToGrayPartial
    {
        public HistogramEquilization()
        {
        }

        protected override unsafe void ProcessFilter(BitmapData imageData, Rectangle rect)
        {
            int startY = rect.Top;
            int stopY = startY + rect.Height;

            int startX = rect.Left;
            int stopX = startX + rect.Width;

            int offset = imageData.Stride - (stopX - startX);

            byte* ptr = (byte*)imageData.Scan0.ToPointer();
            // allign pointer to the first pixel to process
            ptr += (startY * imageData.Stride + rect.Left);

            // create histogram
            int[] histogram = new int[256];            
            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr++)
                {
                    int mean = ptr[0];
                    histogram[mean]++;
                }
                ptr += offset;
            }

            // calc new intensity levels for each intensity level (0-255)
            float[] LUT = Equilize(histogram, (stopX - startX) * (stopY - startY));
            ptr = (byte*)imageData.Scan0.ToPointer();
            // allign pointer to the first pixel to process
            ptr += (startY * imageData.Stride + rect.Left);

            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr++)
                {
                    int index = ptr[0];
                    byte nValue = (byte)LUT[index];
                    if (LUT[index] > 255)
                        nValue = 255;
                    ptr[0] = nValue;
                }

                ptr += offset;
            }
        }

        private float[] Equilize(int[] histogram, long numPixel)
        {
            float[] hist = new float[256];

            hist[0] = histogram[0] * histogram.Length / numPixel;
            long prev = histogram[0];

            for (int i = 1; i < hist.Length; i++)
            {
                prev += histogram[i];
                hist[i] = prev * histogram.Length / numPixel;
            }
            return hist;

        }
    }
}
