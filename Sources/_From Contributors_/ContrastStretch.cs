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
    public class ContrastStretch : FilterGrayToGrayPartial
    {
        public ContrastStretch()
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

            float min = 255;
            float max = 0;

            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr++)
                {
                    if (min > ptr[0])
                        min = ptr[0];

                    if (max < ptr[0])
                        max = ptr[0];
                }
                ptr += offset;
            }

            ptr = (byte*)imageData.Scan0.ToPointer();
            // allign pointer to the first pixel to process
            ptr += (startY * imageData.Stride + rect.Left);

            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr++)
                {
                    ptr[0] = (byte)((ptr[0] - min) * (255 / (max - min)));
                }
                ptr += offset;
            }

        }
    }
}
