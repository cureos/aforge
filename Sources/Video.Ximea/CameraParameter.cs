// AForge Kinect Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

using System;

namespace AForge.Video.Ximea
{
    /// <summary>
    /// Set of available configuration options for XIMEA cameras.
    /// </summary>
    /// 
    /// <remarks><para><note>The list of configuration options may not be complete. See XIMEA API
    /// for the complete up to date list of possible configuration options.</note></para></remarks>
    /// 
    public static class CameraParameter
    {
        /// <summary>
        /// Set/Get exposure time in microseconds. Type integer.
        /// </summary>
        public const string Exposure = "exposure";

        /// <summary>
        /// Set/Get camera gain in dB. Type float. 
        /// </summary>
        public const string Gain = "gain";
    }
}
