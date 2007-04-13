// AForge Vision Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//
namespace AForge.Vision
{
    using System;
	using System.Drawing;

	/// <summary>
	/// Motion detector interface.
	/// </summary>
    /// 
    /// <remarks>The interface defines common methods for all motion detection
    /// algorithms.</remarks>
    /// 
	public interface IMotionDetector
	{
        /// <summary>
        /// Highlight motion regions or not.
        /// </summary>
        /// 
        bool HighlightMotionRegions{ get; set; }

		/// <summary>
		/// Motion level value.
		/// </summary>
        /// 
        /// <remarks>Amount of changes in the last processed frame in percents.</remarks>
        /// 
		double MotionLevel{ get; }

		/// <summary>
		/// Process new frame.
		/// </summary>
        /// 
        /// <remarks>Process new frame of video source and detect motion.</remarks>
        /// 
		void ProcessFrame( Bitmap image );

		/// <summary>
		/// Reset detector to initial state.
		/// </summary>
        /// 
        /// <remarks>Resets internal state and variables of motion detection algorithm.</remarks>
        /// 
		void Reset( );
	}
}
