// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.NXT
{
    using System;

    /// <summary>
    /// Structure, which describes motor's state.
    /// </summary>
    /// 
    public struct MotorState
    {
        /// <summary>
        /// Motor's power set point in the range from -100 to 100.
        /// </summary>
        public sbyte Power;

        /// <summary>
        /// Motor's mode (bit field).
        /// </summary>
        public MotorMode Mode;

        /// <summary>
        /// Motor's regulation mode.
        /// </summary>
        public RegulationMode Regulation;

        /// <summary>
        /// Motor's turn ratio in the range from -100 to 100.
        /// </summary>
        public sbyte TurnRatio;

        /// <summary>
        /// Motor's run state.
        /// </summary>
        public RunState RunState;

        /// <summary>
        /// Tacho limit (0 - run forever).
        /// </summary>
        /// 
        /// <remarks>The value determines movement limit.</remarks>
        public int TachoLimit;

        /// <summary>
        /// Number of counts since last reset of motor counter.
        /// </summary>
        /// 
        /// <remarks><note>The value is ignored when motor's state is set. The value is
        /// provided when motor's state is retrieved.</note></remarks>
        public int TachoCount;

        /// <summary>
        /// Current position relative to last programmed movement.
        /// </summary>
        /// 
        /// <remarks><note>The value is ignored when motor's state is set. The value is
        /// provided when motor's state is retrieved.</note></remarks>
        public int BlockTachoCount;

        /// <summary>
        /// Current position relative to last reset of motor's rotation sensor.
        /// </summary>
        /// 
        /// <remarks><note>The value is ignored when motor's state is set. The value is
        /// provided when motor's state is retrieved.</note></remarks>
        public int RotationCount;
    }
}
