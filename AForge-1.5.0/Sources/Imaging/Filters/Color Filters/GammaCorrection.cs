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
    /// Gamma correction filter.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class GammaCorrection : FilterAnyToAnyPartial
    {
        private double gamma;
        private byte[] table = new byte[256];

        /// <summary>
        /// Gamma value.
        /// </summary>
        /// 
        /// <remarks>Default value is 2.2</remarks>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set
            {
                // get gamma value
                gamma = Math.Max( 0.1, Math.Min( 5.0, value ) );

                // calculate tranformation table
                double g = 1 / gamma;
                for ( int i = 0; i < 256; i++ )
                {
                    table[i] = (byte) Math.Min( 255, (int) ( Math.Pow( i / 255.0, g ) * 255 + 0.5 ) );
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GammaCorrection"/> class.
        /// </summary>
        public GammaCorrection( )
        {
            Gamma = 2.2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GammaCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="gamma">Gamma value.</param>
        /// 
        public GammaCorrection( double gamma )
        {
            Gamma = gamma;
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
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop X,Y positions
            int startX  = rect.Left * pixelSize;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width * pixelSize;
            int stopY   = startY + rect.Height;
            int offset  = imageData.Stride - rect.Width * pixelSize;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // gamma correction
            for ( int y = startY; y < stopY; y++ )
            {
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    // process each pixel
                    *ptr = table[*ptr];
                }
                ptr += offset;
            }
        }
    }
}
