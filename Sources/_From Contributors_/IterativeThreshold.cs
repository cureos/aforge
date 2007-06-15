// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Markus Falkensteiner, 2007
// mfalkensteiner@gmx.at
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

	/// <summary>
	/// Iterative Threshold search and binarization
	/// The algorithm works in the following way:
	/// 1) select any start threshold
	/// 2) compute average value of Background (µB) and Object (µO) values
	///		all pixel with a value that is below T, belong to the background values.
	///		all pixel greater oe equal T belong to the Object values
	///	3) calculate new Thresghold: T_1 = (µB+µO) / 2
	/// 4) if Abs(T- T_1) less than a given max. allowed error. exit iteration process.
	///		now create the binary image with the new threshold
	///
	/// see also: Digital Image Processing, Gonzalez/Woods. Ch.10 page:599
	/// </summary>
	/// 
	/// <remarks></remarks>
    public class IterativeThreshold : Threshold
    {
        private byte m_iMinError = 0;

        /// <summary>
        /// Minimum error, value when iterative threshold search is stopped
        /// </summary>
        public byte MinimumError
        {
            get { return m_iMinError; }
            set { m_iMinError = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class
        /// </summary>
        /// 
        public IterativeThreshold() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class
        /// </summary>
        /// 
        /// <param name="iMinError">Minimum allowed error, that ends the iteration process</param>
        public IterativeThreshold(byte iMinError)
        {
            m_iMinError = iMinError;
        }

               /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="imageData">Image data</param>
        /// 
        protected override unsafe void ProcessFilter(BitmapData imageData)
        {
            // get image width and height
            int width = imageData.Width;
            int height = imageData.Height;
            int offset = imageData.Stride - width;


            int iObjectValue = 0;
            int iNumberObjects = 0;

            int iBackgroundValue = 0;
            int iNumberBackground = 0;

            int newThreshold = 0;

			// currently i only support grayscale image
			if (imageData.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				base.ProcessFilter(imageData);
				return;
			}

            bool first = true;
            do
            {
                iObjectValue = 0;
                iNumberObjects = 0;
                iBackgroundValue = 0;
                iNumberBackground = 0;

                // do the job
                byte* ptr = (byte*)imageData.Scan0.ToPointer();

                if (!first)
                    ThresholdValue = (byte) newThreshold;

                first = false;
                // for each line	
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, ptr++)
                    {
                        if (*ptr >= ThresholdValue)
                        {
                            iObjectValue += (int)*ptr;
                            iNumberObjects++;
                        }
                        else
                        {
                            iBackgroundValue += (int)*ptr;
                            iNumberBackground++;
                        }
                    }
                    ptr += offset;
                }

				byte iMeanObject = 0;
				byte iMeanBackground = 0;
				
				if (iNumberObjects > 0)
					iMeanObject = Convert.ToByte(iObjectValue / iNumberObjects);

                if (iNumberBackground > 0)
					iMeanBackground = Convert.ToByte(iBackgroundValue / iNumberBackground);

                newThreshold = (iMeanBackground + iMeanObject) / 2;

            } while (Math.Abs(ThresholdValue - newThreshold) > m_iMinError);

            ThresholdValue = Convert.ToByte(newThreshold);
            base.ProcessFilter(imageData);
        }
    }
}
