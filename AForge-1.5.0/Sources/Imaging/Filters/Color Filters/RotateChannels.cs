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
    /// Rotate RGB channels.
    /// </summary>
    /// 
    /// <remarks><para>The filter rotates RGB channels: red channel is replaced with green,
    /// green channel is replaced with blue, blue channel is replaced with red.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// RotateChannels filter = new RotateChannels( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="rotate_channels.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public sealed class RotateChannels : FilterColorToColorPartial
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
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = imageData.Stride - rect.Width * 3;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );
            byte t;

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX * 3 );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += 3 )
                {
                    // rotate colors of each pixel
                    t = ptr[RGB.R];
                    ptr[RGB.R] = ptr[RGB.G];
                    ptr[RGB.G] = ptr[RGB.B];
                    ptr[RGB.B] = t;
                }
                ptr += offset;
            }
        }
    }
}
