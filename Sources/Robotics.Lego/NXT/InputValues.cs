// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.NXT
{
    using System;

    /// <summary>
    /// Structure, which describes input values received from Lego NXT input port.
    /// </summary>
    /// 
    public struct InputValues
    {
        /// <summary>
        /// Specifies if data value should be treated as valid data.
        /// </summary>
        public bool IsValid;

        /// <summary>
        /// Specifies if calibration file was found and used for <see cref="Calibrated"/>
        /// field calculation.
        /// </summary>
        public bool IsCalibrated;

        /// <summary>
        /// Sensor type.
        /// </summary>
        public SensorType SensorType;

        /// <summary>
        /// Sensor mode.
        /// </summary>
        public SensorMode SensorMode;

        /// <summary>
        /// Raw A/D value (device dependent).
        /// </summary>
        public ushort Raw;

        /// <summary>
        /// Normalized A/D value (sensor type dependent).
        /// </summary>
        /// 
        /// <remarks>The value is in the range from 0 to 1023.</remarks>
        ///
        public ushort Normalized;

        /// <summary>
        /// Scaled value (sensor mode dependent).
        /// </summary>
        public short Scaled;

        /// <summary>
        /// Value scaled according to calibration.
        /// </summary>
        /// 
        /// <remarks><note>According to Lego notes the value is currently unused.</note></remarks>
        /// 
        public short Calibrated;
    }
}
