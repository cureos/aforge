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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Text;
    using AForge.Video.Ximea.Internal;

    /// <summary>
    /// The class provides access to XIMEA cameras.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to perform image acquisition from <a href="http://www.ximea.com/">XIMEA</a> cameras.
    /// It wraps XIMEA'a xiAPI, which means that users of this class will also require <b>m3api.dll</b> and a correct
    /// TM file for the camera model connected to the system (both are provided with XIMEA API software package).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// XimeaCamera camera = new XimeaCamera( );
    /// 
    /// // open camera and start data acquisition
    /// camera.Open( 0 );
    /// camera.StartAcquisition( );
    /// 
    /// // set exposure time to 10 milliseconds
    /// camera.SetParam( CameraParameter.Exposure, 10 * 1000 );
    /// 
    /// // get image from the camera
    /// Bitmap bitmap = camera.GetImage( );
    /// // process the image
    /// // ...
    /// 
    /// // dispose the image when it is no longer needed
    /// bitmap.Dispose( );
    /// 
    /// // stop data acquisition and close the camera
    /// camera.StopAcquisition( );
    /// camera.Close( );
    /// </code>
    /// </remarks>
    /// 
    public class XimeaCamera
    {
        private IntPtr deviceHandle = IntPtr.Zero;
        private bool isAcquisitionStarted = false;
        private int deviceID = 0;

        /// <summary>
        /// Get number of XIMEA camera connected to the system.
        /// </summary>
        public static int CamerasCount
        {
            get
            {
                int count;

                int errorCode = XimeaAPI.xiGetNumberDevices( out count );
                HandleError( errorCode );

                return count;
            }
        }

        /// <summary>
        /// Specifies if camera's data acquisition is currently active for the opened camera (if any).
        /// </summary>
        public bool IsAcquisitionStarted
        {
            get { return isAcquisitionStarted; }
        }

        /// <summary>
        /// Specifies if a camera is currently opened by the instance of the class.
        /// </summary>
        public bool IsDeviceOpen
        {
            get { return ( deviceHandle != IntPtr.Zero ); }
        }

        /// <summary>
        /// ID of the the recently opened XIMEA camera.
        /// </summary>
        public int DeviceID
        {
            get { return deviceID; }
        }

        /// <summary>
        /// Open XIMEA camera.
        /// </summary>
        /// 
        /// <param name="deviceID">Camera ID to open.</param>
        /// 
        /// <remarks><para>Opens the specified XIMEA camera preparing it for starting video acquisition
        /// which is done using <see cref="StartAcquisition"/> method. The <see cref="IsDeviceOpen"/>
        /// property can be used at any time to find if a camera was opened or not.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        ///
        public void Open( int deviceID )
        {
            IntPtr deviceHandle;
            int errorCode = XimeaAPI.xiOpenDevice( deviceID, out deviceHandle );
            HandleError( errorCode );
            // save the device handle is everything is fine
            this.deviceHandle = deviceHandle;
            this.isAcquisitionStarted = false;
            this.deviceID = deviceID;
        }

        /// <summary>
        /// Close opened camera (if any) and release allocated resources.
        /// </summary>
        /// 
        /// <remarks><para><note>The method also calls <see cref="StopAcquisition"/> method if it was not
        /// done by user.</note></para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        ///
        public void Close( )
        {
            if ( deviceHandle != IntPtr.Zero )
            {
                if ( isAcquisitionStarted )
                {
                    try
                    {
                        StopAcquisition( );
                    }
                    catch
                    {
                    }
                }

                try
                {
                    int errorCode = XimeaAPI.xiCloseDevice( deviceHandle );
                    HandleError( errorCode );
                }
                finally
                {
                    deviceHandle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Begin camera's work cycle and start data acquisition from it.
        /// </summary>
        /// 
        /// <remarks><para>The <see cref="IsAcquisitionStarted"/> property can be used at any time to find if the
        /// acquisition was started or not.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// 
        public void StartAcquisition( )
        {
            CheckConnection( );

            int errorCode = XimeaAPI.xiStartAcquisition( deviceHandle );
            HandleError( errorCode );

            isAcquisitionStarted = true;
        }

        /// <summary>
        /// End camera's work cycle and stops data acquisition.
        /// </summary>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// 
        public void StopAcquisition( )
        {
            CheckConnection( );

            try
            {
                int errorCode = XimeaAPI.xiStopAcquisition( deviceHandle );
                HandleError( errorCode );
            }
            finally
            {
                isAcquisitionStarted = false;
            }
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Integer parameter value.</param>
        /// 
        /// <remarks><para>The method allows to control different camera's parameters, like exposure time, gain value, etc.
        /// See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.
        /// </para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public void SetParam( string parameterName, int value )
        {
            CheckConnection( );

            int errorCode = XimeaAPI.xiSetParam( deviceHandle, parameterName, ref value, 4, ParameterType.Integer );
            HandleError( errorCode );
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Float parameter value.</param>
        /// 
        /// <remarks><para>The method allows to control different camera's parameters, like exposure time, gain value, etc.
        /// See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.
        /// </para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public void SetParam( string parameterName, float value )
        {
            CheckConnection( );

            int errorCode = XimeaAPI.xiSetParam( deviceHandle, parameterName, ref value, 4, ParameterType.Float );
            HandleError( errorCode );
        }

        /// <summary>
        /// Get camera's parameter as integer value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns integer value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public int GetParamInt( string parameterName )
        {
            CheckConnection( );

            int value;
            int size;
            ParameterType type = ParameterType.Integer;

            int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, out value, out size, ref type );
            HandleError( errorCode );

            return value;
        }

        /// <summary>
        /// Get camera's parameter as float value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns float value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public float GetParamFloat( string parameterName )
        {
            CheckConnection( );

            float value;
            int size;
            ParameterType type = ParameterType.Float;

            int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, out value, out size, ref type );
            HandleError( errorCode );

            return value;
        }

        /// <summary>
        /// Get camera's parameter as string value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns string value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public string GetParamString( string parameterName )
        {
            CheckConnection( );

            byte[] bytes = new byte[260];
            int size = bytes.Length;
            ParameterType type = ParameterType.String;

            unsafe
            {
                fixed ( byte* ptr = bytes )
                {
                    int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, ptr, out size, ref type );
                    HandleError( errorCode );
                }
            }

            return Encoding.ASCII.GetString( bytes, 0, size );
        }

        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>The method calls <see cref="GetImage(int)"/> method specifying 5000 as the timeout
        /// value.</para></remarks>
        ///
        public unsafe Bitmap GetImage( )
        {
            return GetImage( 5000 );
        }

        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <param name="timeout">Maximum time to wait in milliseconds till image becomes available.</param>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>The method calls <see cref="GetImage(int,bool)"/> method specifying <see langword="true"/>
        /// the <b>makeCopy</b> parameter.</para></remarks>
        ///
        public unsafe Bitmap GetImage( int timeout )
        {
            return GetImage( timeout, true );
        }
        
        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <param name="timeout">Maximum time to wait in milliseconds till image becomes available.</param>
        /// <param name="makeCopy">Make a copy of the camera's image or not.</param>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>If the <paramref name="makeCopy"/> is set to <see langword="true"/>, then the method
        /// creates a managed copy of the camera's image, so the managed image stays valid even when the camera
        /// is closed. However, setting this parameter to <see langword="false"/> creates a managed image which is
        /// just a wrapper around camera's unmanaged image. So if camera is closed and its resources are freed, the
        /// managed image becomes no longer valid and accessing it will generate an exception.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// <exception cref="TimeoutException">Time out value reached - no image is available within specified time value.</exception>
        ///
        public Bitmap GetImage( int timeout, bool makeCopy )
        {
            CheckConnection( );

            int errorCode;

            XimeaImage ximeaImage = new XimeaImage( );
            unsafe
            {
                ximeaImage.StructSize = sizeof( XimeaImage );
            }

            // get image from XIMEA camera
            try
            {
                errorCode = XimeaAPI.xiGetImage( deviceHandle, timeout, ref ximeaImage );
            }
            catch ( AccessViolationException )
            {
                errorCode = 9;
            }

            // handle error if any
            HandleError( errorCode );

            // create managed bitmap for the unmanaged image provided by camera
            PixelFormat pixelFormat = PixelFormat.Undefined;
            int stride = 0;

            switch ( ximeaImage.PixelFormat )
            {
                case XimeaImageFormat.Grayscale8:
                    pixelFormat = PixelFormat.Format8bppIndexed;
                    stride = ximeaImage.Width;
                    break;

                case XimeaImageFormat.RGB32:
                    pixelFormat = PixelFormat.Format32bppRgb;
                    stride = ximeaImage.Width * 4;
                    break;

                default:
                    throw new VideoException( "Unsupported pixel format." );
            }

            Bitmap bitmap = null;

            if ( !makeCopy )
            {
                bitmap = new Bitmap( ximeaImage.Width, ximeaImage.Height, stride, pixelFormat, ximeaImage.BitmapData );
            }
            else
            {
                bitmap = new Bitmap( ximeaImage.Width, ximeaImage.Height, pixelFormat );

                // lock destination bitmap data
                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle( 0, 0, ximeaImage.Width, ximeaImage.Height ),
                    ImageLockMode.ReadWrite, pixelFormat );

                int dstStride = bitmapData.Stride;
                int lineSize  = Math.Min( stride, dstStride );

                unsafe
                {
                    byte* dst = (byte*) bitmapData.Scan0.ToPointer( );
                    byte* src = (byte*) ximeaImage.BitmapData.ToPointer( );

                    if ( stride != dstStride )
                    {
                        // copy image
                        for ( int y = 0; y < ximeaImage.Height; y++ )
                        {
                            AForge.SystemTools.CopyUnmanagedMemory( dst, src, lineSize );
                            dst += dstStride;
                            src += stride;
                        }
                    }
                    else
                    {
                        AForge.SystemTools.CopyUnmanagedMemory( dst, src, stride * ximeaImage.Height );
                    }
                }

                // unlock destination images
                bitmap.UnlockBits( bitmapData );
            }

            // set palette for grayscale image
            if ( ximeaImage.PixelFormat == XimeaImageFormat.Grayscale8 )
            {
                ColorPalette palette = bitmap.Palette;
                for ( int i = 0; i < 256; i++ )
                {
                    palette.Entries[i] = Color.FromArgb( i, i, i );
                }
                bitmap.Palette = palette;
            }

            return bitmap;
        }


        // Handle errors from XIMEA API
        private static void HandleError( int errorCode )
        {
            if ( errorCode != 0 )
            {
                if ( errorCode == 10 )
                {
                    throw new TimeoutException( "Time out while waiting for camera response." ); 
                }

                throw new VideoException( "Error code: " + errorCode );
            }
        }

        // Check if a camera is open or not
        private void CheckConnection( )
        {
            if ( deviceHandle == IntPtr.Zero )
            {
                throw new NotConnectedException( "No established connection to XIMEA camera." );
            }
        }
    }
}
