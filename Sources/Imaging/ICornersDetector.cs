// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Corners detector's interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies set of methods, which should be implemented by different
    /// corners detection algorithms.</para></remarks>
    /// 
    public interface ICornersDetector
    {
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        /// 
        Point[] ProcessImage( Bitmap image );

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        /// 
        Point[] ProcessImage( BitmapData imageData );
    }
}
