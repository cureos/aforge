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
    /// Motion detector interface with support of motion zones.
    /// </summary>
    /// 
    /// <remarks>The interface defines set of properties and methods required
    /// from a motion detection algorithm, which supports motion detection in
    /// specified set of zones.
    /// </remarks>
    /// 
    public interface IZonesMotionDetector
    {
        /// <summary>
        /// Set of motion zones.
        /// </summary>
        /// 
        /// <remarks>The property keeps array of zones, which are observed for motion detection.
        /// Motion outside of these zones is ignored.</remarks>
        /// 
        Rectangle[] MotionZones { get; set; }

        /// <summary>
        /// Highligh motion zones or not.
        /// </summary>
        /// 
        /// <remarks>Specifies if zones, which are subject to be observed for motion,
        /// should be highlighted.</remarks>
        /// 
        bool HighlightMotionZones { get; set; }
    }
}
