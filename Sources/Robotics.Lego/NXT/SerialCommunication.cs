// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.NXT
{
    using System;
    using System.IO;
    using System.IO.Ports;

    /// <summary>
    /// Implementation of serial communication interface with LEGO Mindstorm NXT brick.
    /// </summary>
    /// 
    public class SerialCommunication : INXTCommunicationInterface
    {
        // serial port for communication with NXT brick
        SerialPort port = null;

        /// <summary>
        /// Maximum message size, which can be sent over this communication interface to NXT
        /// brick.
        /// </summary>
        /// 
        public const int MaxMessageSize = 64;

        /// <summary>
        /// Serial port name used for communication.
        /// </summary>
        /// 
        public string PortName
        {
            get { return port.PortName; }
            set { port.PortName = value; }
        }

        /// <summary>
        /// Get connection status.
        /// </summary>
        /// 
        public bool IsConnected
        {
            get { return port.IsOpen; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialCommunication"/> class.
        /// </summary>
        /// 
        /// <param name="portName">Serial port name to use for communication.</param>
        /// 
        /// <remarks>This constructor initializes serial port with default write and read
        /// timeout values, which are 1000 milliseconds.</remarks>
        /// 
        public SerialCommunication( string portName ) :
            this( portName, 1000, 1000 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialCommunication"/> class.
        /// </summary>
        /// 
        /// <param name="portName">Serial port name to use for communication.</param>
        /// <param name="writeTimeout">Timeout value used for write operations.</param>
        /// <param name="readTimeout">Timeout value used for read operations.</param>
        /// 
        public SerialCommunication( string portName, int writeTimeout, int readTimeout )
        {
            this.port = new SerialPort( portName );
            this.port.WriteTimeout = writeTimeout;
            this.port.ReadTimeout  = readTimeout;
        }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="CommunicationStatus.Success"/> if connection was done successfully, or
        /// <see cref="CommunicationStatus.Failed"/> otherwise.</returns>
        /// 
        /// <remarks>If communication interface is connected before the call, existing connection will be reused.
        /// If it is required to force reconnection, then <see cref="Disconnect"/> method should be called before.
        /// </remarks>
        /// 
        public CommunicationStatus Connect( )
        {
            if ( !port.IsOpen )
            {
                // try to connect 
                try
                {
                    port.Open( );
                }
                catch
                {
                    return CommunicationStatus.Failed;
                }
            }
            return CommunicationStatus.Success;
        }

        /// <summary>
        /// Disconnect from NXT brick.
        /// </summary>
        public void Disconnect( )
        {
            if ( port.IsOpen )
            {
                port.Close( );
            }
        }

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
        public CommunicationStatus SendMessage( byte[] message )
        {
            return SendMessage( message, 0, message.Length );
        }

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
        public CommunicationStatus SendMessage( byte[] message, int length )
        {
            return SendMessage( message, 0, length );
        }

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
        public CommunicationStatus SendMessage( byte[] message, int offset, int length )
        {
            // check connection status
            if ( !port.IsOpen )
            {
                return CommunicationStatus.NotConnected;
            }

            // check message size
            if ( length > MaxMessageSize )
            {
                return CommunicationStatus.TooBigMessage;
            }

            try
            {
                // send 2 bytes of message length
                byte[] messageLength = new byte[2] { (byte) length, 0 };
                port.Write( messageLength, 0, 2 );
                // send actual message
                port.Write( message, offset, length );
            }
            catch ( TimeoutException )
            {
                return CommunicationStatus.Timeout;
            }
            catch ( IOException )
            {
                return CommunicationStatus.Failed;
            }

            return CommunicationStatus.Success;
        }

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
        /// <remarks><note>In the case if <see cref="CommunicationStatus.TooSmallBuffer"/> error code was returned,
        /// the message is discarded, so it is not possible to re-read it with subsequent read.</note></remarks>
        /// 
        public CommunicationStatus ReadMessage( byte[] buffer, ref int length )
        {
            return ReadMessage( buffer, 0, ref length );
        }

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
        /// <remarks><note>In the case if <see cref="CommunicationStatus.TooSmallBuffer"/> error code was returned,
        /// the message is discarded, so it is not possible to re-read it with subsequent read.</note></remarks>
        /// 
        public CommunicationStatus ReadMessage( byte[] buffer, int offset, ref int length )
        {
            // check connection status
            if ( !port.IsOpen )
            {
                return CommunicationStatus.NotConnected;
            }

            try
            {
                // read 2 bytes of message length
                // - first byte keeps the length
                int toRead = port.ReadByte( );
                // - second byte is zero
                port.ReadByte( );
                // check buffer size
                if ( toRead > buffer.Length - offset )
                {
                    // remove incomming message from the port
                    while ( toRead != 0 )
                    {
                        port.ReadByte( );
                        toRead--;
                    }
                    return CommunicationStatus.TooSmallBuffer;
                }
                // read the message
                length = port.Read( buffer, offset, toRead );

                while ( length != toRead )
                {
                    buffer[offset + length] = (byte) port.ReadByte( );
                    length++;
                }
            }
            catch ( TimeoutException )
            {
                return CommunicationStatus.Timeout;
            }
            catch ( IOException )
            {
                return CommunicationStatus.Failed;
            }

            return CommunicationStatus.Success;
        }
    }
}
