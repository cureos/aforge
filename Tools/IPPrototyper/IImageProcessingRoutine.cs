// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Drawing;

namespace AForge.Imaging.IPPrototyper
{
    /// <summary>
    /// Interface to implement by image processing routine.
    /// </summary>
    public interface IImageProcessingRoutine
    {
        /// <summary>
        /// Image processing routine's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Perform image processing routine.
        /// </summary>
        /// 
        /// <param name="image">Source image to perform image processing on.</param>
        /// <param name="log">Logger to use to put information about image processing steps/results.</param>
        /// 
        void Process( Bitmap image, IImageProcessingLog log );
    }
}
