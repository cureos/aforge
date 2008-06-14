// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego
{
    using System;
    using System.Text;
    using AForge.Robotics.Lego.Internals;

    /// <summary>
    /// Manipulation of Lego Mindstorms RCX device.
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// 
    /// <para><note>Only communication through USB IR tower is supported at this point.</note></para>
    /// 
    /// </remarks>
    /// 
    public class RCXBrick
    {
        // Ghost communication stack
        private IntPtr stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="RCXBrick"/> class.
        /// </summary>
        /// 
        public RCXBrick( )
        {
        }

        /// <summary>
        /// Destroys the instance of the <see cref="RCXBrick"/> class.
        /// </summary>
        /// 
        ~RCXBrick( )
		{
            Disconnect( );
		}

        /// <summary>
        /// Connect to Lego RCX brick.
        /// </summary>
        /// 
        /// <returns>Returns <b>true</b> on successful connection or <b>false</b>
        /// otherwise.</returns>
        /// 
        public bool Connect( )
        {
            // check if we are already connected
            if ( stack != IntPtr.Zero )
                return true;

            uint status;

            // create stack
            status = GhostAPI.GhCreateStack(
                "LEGO.Pbk.CommStack.Port.USB",
                "LEGO.Pbk.CommStack.Protocol.IR",
                "LEGO.Pbk.CommStack.Session",
                out stack );

            if ( !GhostAPI.PBK_SUCCEEDED( status ) )
                return false;

            // select first available device
            StringBuilder sb = new StringBuilder( 200 );
            status = GhostAPI.GhSelectFirstDevice( stack, sb, sb.Length );

            if ( !GhostAPI.PBK_SUCCEEDED( status ) )
            {
                Disconnect( );
                return false;
            }

            // open stack, set interleave, set wait mode and check if the brick is alive
            if (
                !GhostAPI.PBK_SUCCEEDED( GhostAPI.GhOpen( stack ) ) ||
                !GhostAPI.PBK_SUCCEEDED( GhostAPI.GhSetWaitMode( stack, IntPtr.Zero ) ) ||
                !GhostAPI.PBK_SUCCEEDED( GhostAPI.GhSetInterleave( stack, 1, 0 ) ) ||
                !IsAlive( )
                )
            {
                Disconnect( );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnnect from Lego RCX brick.
        /// </summary>
        public void Disconnect( )
        {
            if ( stack != IntPtr.Zero )
            {
                Internals.GhostAPI.GhClose( stack );
                stack = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Check if the RCX brick is alive and responds to messages.
        /// </summary>
        /// 
        /// <remarks><para>
        /// <note>The check is done by sending command with <b>0x18</b> opcode.</note>
        /// </para></remarks>
        /// 
        /// <returns>Returns <b>true</b> if device is alive or <b>false</b> otherwise.</returns>
        /// 
        public bool IsAlive( )
        {
            return SendCommand( new byte[] { 0x18 }, new byte[1], 1 );
        }

        /// <summary>
        /// Send command to Lego RCX brick.
        /// </summary>
        /// 
        /// <param name="command">Command to send.</param>
        /// <param name="reply">Buffer to receive reply into.</param>
        /// <param name="expectedReplyLen">Expected reply length.</param>
        /// 
        /// <returns>Returns <b>true</b> if the command was sent successfully and reply was
        /// received, otherwise <b>false</b>.</returns>
        /// 
        /// <exception cref="ArgumentException">Reply buffer size is smaller than the reply data size.</exception>
        /// <exception cref="ApplicationException">Reply does not correspond to command (first byte of reply
        /// should be complement (bitwise NOT) to first byte of command.</exception>
        /// 
        protected bool SendCommand( byte[] command, byte[] reply, int expectedReplyLen )
        {
            bool result = false;
            uint status;
            IntPtr queue;

            // create command queue
            status = GhostAPI.GhCreateCommandQueue( out queue );

            if ( !GhostAPI.PBK_SUCCEEDED( status ) )
                return false;

            // append command to the queue
            status = GhostAPI.GhAppendCommand( queue, command, command.Length, expectedReplyLen );

            if ( GhostAPI.PBK_SUCCEEDED( status ) )
            {
                // execute command
                status = GhostAPI.GhExecute( stack, queue );

                if ( GhostAPI.PBK_SUCCEEDED( status ) )
                {
                    IntPtr commandHandle;
                    uint replyLen;

                    // get first command and its reply data lenght
                    if (
                        ( GhostAPI.PBK_SUCCEEDED( GhostAPI.GhGetFirstCommand( queue, out commandHandle ) ) ) &&
                        ( GhostAPI.PBK_SUCCEEDED( GhostAPI.GhGetCommandReplyLen( commandHandle, out replyLen ) ) )
                        )
                    {
                        // check provided reply buffer size
                        if ( reply.Length < replyLen )
                            throw new ArgumentException( "Reply buffer is too small" );

                        // get reply data
                        status = GhostAPI.GhGetCommandReply( commandHandle, reply, replyLen );

                        if ( GhostAPI.PBK_SUCCEEDED( status ) )
                        {
                            // check that reply corresponds εω command
                            if ( command[0] != (byte) ~reply[0] )
                                throw new ApplicationException( "Reply does not correspond to command" );

                            for ( int i = 0; i < replyLen; i++)
                                System.Diagnostics.Debug.Write( reply[i].ToString( "X2" ) + " " );
                            System.Diagnostics.Debug.WriteLine( "" );

                            result = true;
                        }
                    }
                }
            }

            // destroy command queue
            GhostAPI.GhDestroyCommandQueue( queue );

            return result;
        }
    }
}
