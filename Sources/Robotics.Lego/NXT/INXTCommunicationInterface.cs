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
    /// Enumeration, which describes communication statuses (error codes).
    /// </summary>
    /// 
    public enum CommunicationStatus
    {
        /// <summary>
        /// Successful communication.
        /// </summary>
        Success,

        /// <summary>
        /// Communication interface is not connected to device.
        /// </summary>
        NotConnected,

        /// <summary>
        /// Communication operation finished with timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// Generic communication error.
        /// </summary>
        Failed,

        /// <summary>
        /// Successful communication, but error occurred on device side.
        /// </summary>
        DeviceError,

        /// <summary>
        /// Successful communication, but unknown error occurred on device side.
        /// </summary>
        UnknownDeviceError,

        /// <summary>
        /// To small buffer was provided to read entire message from NXT.
        /// </summary>
        TooSmallBuffer,

        /// <summary>
        /// Message is too big for sending over communication interface.
        /// </summary>
        TooBigMessage,

        /// <summary>
        /// Invalid argument was passed.
        /// </summary>
        InvalidArgument
    }

    /// <summary>
    /// Interface, which wraps communication functions with Lego Mindstorms NXT brick.
    /// </summary>
    /// 
    public interface INXTCommunicationInterface
    {
        /// <summary>
        /// Get connection status.
        /// </summary>
        /// 
        bool IsConnected { get; }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if connection was done successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus Connect( );

        /// <summary>
        /// Disconnect from NXT brick.
        /// </summary>
        /// 
        void Disconnect( );

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// 
        /// <remarks>This method assumes that message starts from the start of the
        /// specified buffer and occupies entire buffer.</remarks>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if message was sent successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus SendMessage( byte[] message );

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// <param name="length">Length of the message to send.</param>
        /// 
        /// <remarks>This method assumes that message starts from the start of the
        /// specified buffer.</remarks>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if message was sent successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus SendMessage( byte[] message, int length );

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// <param name="offset">Offset of the message in the buffer.</param>
        /// <param name="length">Length of the message to send.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if message was sent successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus SendMessage( byte[] message, int offset, int length );

        /// <summary>
        /// Read message from NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="buffer">Buffer used for reading message.</param>
        /// <param name="length">On successful return the variable keeps message length.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if message was read successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus ReadMessage( byte[] buffer, ref int length );

        /// <summary>
        /// Read message from NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="buffer">Buffer used for reading message.</param>
        /// <param name="offset">Offset in the buffer for message.</param>
        /// <param name="length">On successful return the variable keeps message length.</param>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if message was read successfully, or
        /// another value describing error.</returns>
        /// 
        CommunicationStatus ReadMessage( byte[] buffer, int offset, ref int length );
    }
}
