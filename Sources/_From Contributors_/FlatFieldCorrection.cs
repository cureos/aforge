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
    public class FlatFieldCorrection : FilterGrayToGray
    {
        Bitmap _baseImage = null;

        /// <summary>
        /// Base image is created from the input image and is blurred 5 times with the maximum Gauss blur
        /// </summary>
        public FlatFieldCorrection()
        {
        }
        
        /// <summary>
        /// Base image of the background with no objects
        /// </summary>
        /// <param name="baseImage"></param>
        public FlatFieldCorrection(Bitmap baseImage)
        {
            _baseImage = baseImage;
        }

        protected override unsafe void ProcessFilter(BitmapData imageData)
        {
            if (_baseImage == null)
            {
                // shrink image to 1/3 its original size to make bluring faster
                ResizeBicubic rbc = new ResizeBicubic((int)imageData.Width / 3, (int)imageData.Height / 3);
                _baseImage = rbc.Apply(imageData);
                // create base image from the input image blurred with Maximum Gauss 5 times
                GaussianBlur gb = new GaussianBlur(5, 25);
                // first blur the original image to the base image
                _baseImage = gb.Apply(_baseImage);
                // blur the base image for 4 more times just to get a better mean
                gb.ApplyInPlace(_baseImage);
                gb.ApplyInPlace(_baseImage);
                gb.ApplyInPlace(_baseImage);
                gb.ApplyInPlace(_baseImage);
                
                // resize the blured image back to original size
                rbc.NewWidth = imageData.Width;
                rbc.NewHeight = imageData.Height;
                _baseImage = rbc.Apply(_baseImage);
            }
            // lock base image
            BitmapData baseImageData = _baseImage.LockBits(
                new Rectangle(0, 0, imageData.Width, imageData.Height),
                ImageLockMode.ReadOnly, imageData.PixelFormat);

            // get base image mean to use it as the correction factor
            ImageStatistics imgStats = new ImageStatistics(baseImageData);
            byte mean = Convert.ToByte(imgStats.Gray.Mean);

            int offsetSrc = imageData.Stride - imageData.Width;
            byte* ptrSrc = (byte*)(imageData.Scan0);

            int offsetBase = baseImageData.Stride - baseImageData.Width;
            byte* ptrBase = (byte*)(baseImageData.Scan0);

            for (int y = 0; y < imageData.Height; y++)
            {
                for (int x = 0; x < imageData.Width; x++, ptrSrc++, ptrBase++)
                {
                    // avoid divide by zero error
                    if (*ptrBase == 0)
                        continue;
                    *ptrSrc = (byte)Math.Min(((float)*ptrSrc / (float)*ptrBase) * mean, 255);
                }
                ptrSrc += offsetSrc;
                ptrBase += offsetBase;
            }
            _baseImage.UnlockBits(baseImageData);
        }
    }
}
