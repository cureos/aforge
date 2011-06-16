// AForge Kinect Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Ximea
{
    using System;
    
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
        /// Get camera model name. Type string.
        /// </summary>
        public const string DeviceName = "device_name:info";

        /// <summary>
        /// Get device serial number in decimal format. Type string, integer, float
        /// </summary>
        public const string DeviceSerialNumber = "device_sn:info";

        /// <summary>
        /// Returns device type (1394, USB2.0, CURRERA…..). Type string.
        /// </summary>
        public const string DeviceType = "device_type:info";

        /// <summary>
        /// Set/Get exposure time in microseconds. Type integer.
        /// </summary>
        public const string Exposure = "exposure";

        /// <summary>
        /// Get longest possible exposure to be set on camera in microseconds. Type integer.
        /// </summary>
        public const string ExposureMax = "exposure:max";

        /// <summary>
        /// Get shortest possible exposure to be set on camera in microseconds. Type integer. 
        /// </summary>
        public const string ExposureMin = "exposure:min";

        /// <summary>
        /// Set/Get camera gain in dB. Type float. 
        /// </summary>
        public const string Gain = "gain";

        /// <summary>
        /// Get highest possible camera gain in dB. Type float.
        /// </summary>
        public const string GainMax = "gain:max";

        /// <summary>
        /// Get lowest possible camera gain in dB. Type float.
        /// </summary>
        public const string GainMin = "gain:min";

        /// <summary>
        /// Set/Get width of the image provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string Width = "width";

        /// <summary>
        /// Get maximal image width provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string WidthMax = "width:max";

        /// <summary>
        /// Get minimum image width provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string WidthMin = "width:min";

        /// <summary>
        /// Set/Get height of the image provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string Height = "height";

        /// <summary>
        /// Get maximal image height provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string HeightMax = "height:max";

        /// <summary>
        /// Get minimum image height provided by the camera (in pixels). Type integer.
        /// </summary>
        public const string HeightMin = "height:min";

        /// <summary>
        /// Set/Get image resolution by binning or skipping. Type integer. 
        /// </summary>
        public const string Downsampling = "downsampling";

        /// <summary>
        /// Get highest value for binning or skipping. Type integer.
        /// </summary>
        public const string DownsamplingMax = "downsampling:max";

        /// <summary>
        /// Get lowest value for binning or skipping. Type integer.
        /// </summary>
        public const string DownsamplingMin = "downsampling:min";

        /// <summary>
        /// Get frames per second. Type float. 
        /// </summary>
        public const string Framerate = "framerate";

        /// <summary>
        /// Get highest possible framerate for current camera settings. Type float.
        /// </summary>
        public const string FramerateMax = "framerate:max";

        /// <summary>
        /// Get lowest framerate for current camera settings. Type float.
        /// </summary>
        public const string FramerateMin = "framerate:min";

        /// <summary>
        /// Set/Get horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetX = "offsetX";

        /// <summary>
        /// Get maximum horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetXMax = "offsetX:max";

        /// <summary>
        /// Get minimum horizontal offset from the origin to the area of interest (in pixels). Type integer. 
        /// </summary>
        public const string OffsetXMin = "offsetX:min";

        /// <summary>
        /// Set/Get vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetY = "offsetY";

        /// <summary>
        /// Get maximum vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetYMax = "offsetY:max";

        /// <summary>
        /// Get minimal vertical offset from the origin to the area of interest (in pixels). Type integer.
        /// </summary>
        public const string OffsetYMin = "offsetY:min";

        /// <summary>
        /// Set/Get white balance blue coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceBlue = "wb_kb";

        /// <summary>
        /// Set/Get white balance red coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceREd = "wb_kr";

        /// <summary>
        /// Set/Get white balance green coefficient. Type float.
        /// </summary>
        public const string WhiteBalanceGreen = "wb_kg";

        /// <summary>
        /// Set/Get sharpness strenght. Type float. 
        /// </summary>
        public const string Sharpness = "sharpness";
    }
}
