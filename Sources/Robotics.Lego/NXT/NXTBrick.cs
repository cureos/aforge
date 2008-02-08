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
    /// Manipulation of Lego Mindstorms NXT device.
    /// </summary>
    /// 
    public class NXTBrick
    {
        // communication interfaced used for communication with NXT brick
        private INXTCommunicationInterface communicationInterface;
        // internal buffer used for communication
        private byte[] communicationBuffer = new byte[64];
        // last device error
        private DeviceError lastError = DeviceError.Success;

        /// <summary>
        /// Check if connection to NXT brick is established.
        /// </summary>
        /// 
        public bool IsConnected
        {
            get { return ( communicationInterface.IsConnected ); }
        }

        /// <summary>
        /// Last error code returned from device.
        /// </summary>
        /// 
        /// <remarks>The property keeps last error code returned by NXT brick during communication. The property
        /// is updated every time, when one of class's methods returns <see cref="CommunicationStatus.DeviceError"/>
        /// or <see cref="CommunicationStatus.UnknownDeviceError"/> error code.</remarks>
        /// 
        public DeviceError LastDeviceError
        {
            get { return lastError; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NXTBrick"/> class.
        /// </summary>
        /// 
        /// <param name="communicationInterface">Communication interface to use for communication with NXT device.</param>
        /// 
        public NXTBrick( INXTCommunicationInterface communicationInterface )
        {
            this.communicationInterface = communicationInterface;
        }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if connection was done successfully, or
        /// <see cref="CommunicationStatus.Failed"/> otherwise.</returns>
        /// 
        /// <remarks>If connection to NXT brick is established before the call, existing connection will be reused.
        /// If it is required to force reconnection, then <see cref="Disconnect"/> method should be called before.
        /// </remarks>
        /// 
        public CommunicationStatus Connect( )
        {
            // reset last device error
            lastError = DeviceError.Success;

            return communicationInterface.Connect( );
        }

        /// <summary>
        /// Disconnect from NXT brick.
        /// </summary>
        /// 
        public void Disconnect( )
        {
            communicationInterface.Disconnect( );
        }

        /// <summary>
        /// Get firmware version of NXT brick.
        /// </summary>
        /// 
        /// <param name="protocolVersion">Protocol version.</param>
        /// <param name="firmwareVersion">Firmware version.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        ///
        public CommunicationStatus GetFirmwareVersion( ref string protocolVersion, ref string firmwareVersion )
        {
            CommunicationStatus status = CommunicationStatus.Success;

            // prepare message
            communicationBuffer[0] = (byte) CommandType.SystemCommand;
            communicationBuffer[1] = (byte) SystemCommand.GetFirmwareVersion;

            status = SendMessageAndGetReply( communicationBuffer, 2, communicationBuffer );

            if ( status == CommunicationStatus.Success )
            {
                protocolVersion = string.Format( "{0}.{1}", communicationBuffer[4], communicationBuffer[3] );
                firmwareVersion = string.Format( "{0}.{1}", communicationBuffer[6], communicationBuffer[5] );
            }
            return status;
        }

        /// <summary>
        /// Get information about NXT device.
        /// </summary>
        /// 
        /// <param name="deviceName">Device name.</param>
        /// <param name="btAddress">Bluetooth address.</param>
        /// <param name="btSignalStrength">Bluetooth signal strength.</param>
        /// <param name="freeUserFlash">Free user Flash.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus GetDeviceInformation( ref string deviceName, ref byte[] btAddress, ref int btSignalStrength, ref int freeUserFlash )
        {
            CommunicationStatus status = CommunicationStatus.Success;

            // prepare message
            communicationBuffer[0] = (byte) CommandType.SystemCommand;
            communicationBuffer[1] = (byte) SystemCommand.GetDeviceInfo;

            status = SendMessageAndGetReply( communicationBuffer, 2, communicationBuffer );

            if ( status == CommunicationStatus.Success )
            {
                // devince name
                deviceName = System.Text.ASCIIEncoding.ASCII.GetString( communicationBuffer, 3, 15 );
                // Bluetooth address
                Array.Copy( communicationBuffer, 18, btAddress, 0, 7 );
                // Bluetooth signal strength
                btSignalStrength = communicationBuffer[25] | ( communicationBuffer[26] << 8 ) |
                    ( communicationBuffer[27] << 16 ) | ( communicationBuffer[28] << 24 );
                // free user Flash
                freeUserFlash = communicationBuffer[29] | ( communicationBuffer[30] << 8 ) |
                    ( communicationBuffer[31] << 16 ) | ( communicationBuffer[32] << 24 );
            }
            return status;
        }

        /// <summary>
        /// Get battery level of NXT device.
        /// </summary>
        /// 
        /// <param name="batteryLevel">Battery level in millivolts.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus GetBatteryLevel( ref int batteryLevel )
        {
            CommunicationStatus status = CommunicationStatus.Success;

            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.GetBatteryLevel;

            status = SendMessageAndGetReply( communicationBuffer, 2, communicationBuffer );

            if ( status == CommunicationStatus.Success )
            {
                batteryLevel = communicationBuffer[3] | ( communicationBuffer[4] << 8 );
            }
            return status;
        }


        /// <summary>
        /// Set name of NXT device.
        /// </summary>
        /// 
        /// <param name="deviceName">Device name to set for the brick.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus SetBrickName( string deviceName )
        {
            // prepare message
            Array.Clear( communicationBuffer, 0, 18 );
            communicationBuffer[0] = (byte) CommandType.SystemCommand;
            communicationBuffer[1] = (byte) SystemCommand.SetBrickName;
            // convert string to bytes
            System.Text.ASCIIEncoding.ASCII.GetBytes( deviceName, 0, Math.Min( deviceName.Length, 14 ), communicationBuffer, 2 );

            return SendMessageAndGetReply( communicationBuffer, 18, communicationBuffer );
        }

        /// <summary>
        /// Reset motor position.
        /// </summary>
        /// 
        /// <param name="motorPort">Motor to reset.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus ResetMotorPosition( OutputPort motorPort )
        {
            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.ResetMotorPosition;
            communicationBuffer[2] = (byte) motorPort;

            return SendMessageAndGetReply( communicationBuffer, 3, communicationBuffer );
        }

        /// <summary>
        /// Set motor state.
        /// </summary>
        /// 
        /// <param name="motorPort">Motor to set state for.</param>
        /// <param name="motorState">Motor's state.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus SetMotorState( OutputPort motorPort, MotorState motorState )
        {
            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.SetOutputState;
            communicationBuffer[2] = (byte) motorPort;
            communicationBuffer[3] = (byte) motorState.Power;
            communicationBuffer[4] = (byte) motorState.Mode;
            communicationBuffer[5] = (byte) motorState.Regulation;
            communicationBuffer[6] = (byte) motorState.TurnRatio;
            communicationBuffer[7] = (byte) motorState.RunState;
            // tacho limit
            communicationBuffer[8]  = (byte) ( motorState.TachoLimit & 0xFF );
            communicationBuffer[9]  = (byte) ( ( motorState.TachoLimit >> 8 ) & 0xFF );
            communicationBuffer[10] = (byte) ( ( motorState.TachoLimit >> 16 ) & 0xFF );
            communicationBuffer[11] = (byte) ( ( motorState.TachoLimit >> 24 ) & 0xFF ); 

            return SendMessageAndGetReply( communicationBuffer, 12, communicationBuffer );
        }

        /// <summary>
        /// Get motor state.
        /// </summary>
        /// 
        /// <param name="motorPort">Motor to get state for.</param>
        /// <param name="motorState">Motor's state.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus GetMotorState( OutputPort motorPort, out MotorState motorState )
        {
            motorState = new MotorState( );

            // check motor port
            if ( motorPort == OutputPort.All )
            {
                return CommunicationStatus.InvalidArgument;
            }

            CommunicationStatus status = CommunicationStatus.Success;

            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.GetOutputState;
            communicationBuffer[2] = (byte) motorPort;

            status = SendMessageAndGetReply( communicationBuffer, 3, communicationBuffer );

            if ( status == CommunicationStatus.Success )
            {
                motorState.Power        = (sbyte) communicationBuffer[4];
                motorState.Mode         = (MotorMode) communicationBuffer[5];
                motorState.Regulation   = (RegulationMode) communicationBuffer[6];
                motorState.TurnRatio    = (sbyte) communicationBuffer[7];
                motorState.RunState     = (RunState) communicationBuffer[8];

                // tacho limit
                motorState.TachoLimit = communicationBuffer[9] | ( communicationBuffer[10] << 8 ) |
                        ( communicationBuffer[11] << 16 ) | ( communicationBuffer[12] << 24 );
                // tacho count
                motorState.TachoCount = communicationBuffer[13] | ( communicationBuffer[14] << 8 ) |
                        ( communicationBuffer[15] << 16 ) | ( communicationBuffer[16] << 24 );
                // block tacho count
                motorState.BlockTachoCount = communicationBuffer[17] | ( communicationBuffer[18] << 8 ) |
                        ( communicationBuffer[19] << 16 ) | ( communicationBuffer[20] << 24 );
                // rotation count
                motorState.RotationCount = communicationBuffer[21] | ( communicationBuffer[22] << 8 ) |
                        ( communicationBuffer[23] << 16 ) | ( communicationBuffer[24] << 24 );
            }

            return status;
        }

        /// <summary>
        /// Set input mode for specified input port.
        /// </summary>
        /// 
        /// <param name="inputPort">Input port to set mode for.</param>
        /// <param name="type">Sensor's type.</param>
        /// <param name="mode">Sensor's mode.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus SetInputMode( InputPort inputPort, SensorType type, SensorMode mode )
        {
            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.SetInputMode;
            communicationBuffer[2] = (byte) inputPort;
            communicationBuffer[3] = (byte) type;
            communicationBuffer[4] = (byte) mode;

            return SendMessageAndGetReply( communicationBuffer, 5, communicationBuffer );
        }

        /// <summary>
        /// Get input values of specified input port.
        /// </summary>
        /// 
        /// <param name="inputPort">Input port to get values of.</param>
        /// <param name="inputValues">Retrieved input values.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus GetInputValues( InputPort inputPort, out InputValues inputValues )
        {
            CommunicationStatus status = CommunicationStatus.Success;

            inputValues = new InputValues( );

            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.GetInputValues;
            communicationBuffer[2] = (byte) inputPort;

            status = SendMessageAndGetReply( communicationBuffer, 3, communicationBuffer );

            if ( status == CommunicationStatus.Success )
            {
                inputValues.IsValid         = ( communicationBuffer[4] != 0 );
                inputValues.IsCalibrated    = ( communicationBuffer[5] != 0 );
                inputValues.SensorType      = (SensorType) communicationBuffer[6];
                inputValues.SensorMode      = (SensorMode) communicationBuffer[7];
                inputValues.Raw             = (ushort) ( communicationBuffer[8] | ( communicationBuffer[9] << 8 ) );
                inputValues.Normalized      = (ushort) ( communicationBuffer[10] | ( communicationBuffer[11] << 8 ) );
                inputValues.Scaled          = (short) ( communicationBuffer[12] | ( communicationBuffer[13] << 8 ) );
                inputValues.Calibrated      = (short) ( communicationBuffer[14] | ( communicationBuffer[15] << 8 ) );
            }

            return status;
        }

        /// <summary>
        /// Reset input scaled value of specified input port. 
        /// </summary>
        /// 
        /// <param name="inputPort">Input port to reset.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if communication with NXT device was
        /// successful, or another value describing error. In the case if <see cref="CommunicationStatus.DeviceError"/> or
        /// <see cref="CommunicationStatus.UnknownDeviceError"/> status is returned, <see cref="LastDeviceError"/>
        /// property is updated with device error code.</returns>
        /// 
        public CommunicationStatus ResetInputScaledValue( InputPort inputPort )
        {
            // prepare message
            communicationBuffer[0] = (byte) CommandType.DirectCommand;
            communicationBuffer[1] = (byte) DirectCommand.ResetInputScaledValue;
            communicationBuffer[2] = (byte) inputPort;

            return SendMessageAndGetReply( communicationBuffer, 3, communicationBuffer );
        }

        #region Private methods

        /// <summary>
        /// Sends prepared message to NXT message and reads reply.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing message to send.</param>
        /// <param name="messageLength">Message length in the buffer.</param>
        /// <param name="reply">Buffer, which receives reply message.</param>
        /// 
        /// <returns>Returns communication status.</returns>
        /// 
        private CommunicationStatus SendMessageAndGetReply( byte[] message, int messageLength, byte[] reply )
        {
            CommunicationStatus status = CommunicationStatus.Success;

            // send message to NXT brick
            status = communicationInterface.SendMessage( message, messageLength );

            if ( status == CommunicationStatus.Success )
            {
                int bytesRead = 0;

                // read message
                status = communicationInterface.ReadMessage( reply, ref bytesRead );

                if ( status == CommunicationStatus.Success )
                {
                    // check for errors
                    if ( reply[2] != 0 )
                    {
                        // set last error
                        lastError = (DeviceError) reply[2];
                        // set status
                        status = ( Enum.IsDefined( typeof( DeviceError ), reply[2] ) ) ?
                            CommunicationStatus.DeviceError : CommunicationStatus.UnknownDeviceError;
                    }
                }
            }

            return status;
        }

        #endregion
    }
}
