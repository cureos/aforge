// AForge Vision Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//
namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Motion detector interface with objects' counting capabilities.
    /// </summary>
    /// 
    /// <remarks>The interface defines set of properties and methods required
    /// from a motion detection algorithm with object's counting capabilities.
    /// </remarks>
    /// 
    public interface ICountingMotionDetector : IMotionDetector
    {
        /// <summary>
        /// Specifies minimum width of acceptable object.
        /// </summary>
        /// 
        int MinObjectsWidth { get; set; }

        /// <summary>
        /// Specifies minimum height of acceptable object.
        /// </summary>
        /// 
        int MinObjectsHeight { get; set; }

        /// <summary>
        /// Specifies maximum width of acceptable object.
        /// </summary>
        /// 
        int MaxObjectsWidth { get; set; }

        /// <summary>
        /// Specifies maximum height of acceptable object.
        /// </summary>
        /// 
        int MaxObjectsHeight { get; set; }

        /// <summary>
        /// Amount of objects found.
        /// </summary>
        /// 
        int ObjectCount { get; }

        /// <summary>
        /// Rectangles of moving objects.
        /// </summary>
        /// 
        Rectangle[] ObjectRectangles { get; }
    }
}
