// AForge Video Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video
{
	using System;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Net;

    /// <summary>
    /// MJPEG video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source downloads JPEG images from the specified URL, which represents
    /// MJPEG stream.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create MJPEG video source
    /// MJPEGStream stream = new MJPEGStream( "some url" );
    /// // set event handlers
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// stream.Start( );
    /// // ...
    /// // signal to stop
    /// stream.SignalToStop( );
    /// </code>
    /// <para><b>Note</b>: Some cameras produce HTTP header, which does conform strictly to
    /// standard, what leads to .NET exception. To avoid this exception the <b>useUnsafeHeaderParsing</b>
    /// configuration option of <b>httpWebRequest</b> should be set, what may be done using application
    /// configuration file.</para>
    /// <code>
    /// &lt;configuration&gt;
    /// 	&lt;system.net&gt;
    /// 		&lt;settings&gt;
    /// 			&lt;httpWebRequest useUnsafeHeaderParsing="true" /&gt;
    /// 		&lt;/settings&gt;
    /// 	&lt;/system.net&gt;
    /// &lt;/configuration&gt;
    /// </code>
    /// </remarks>
    /// 
    public class MJPEGStream : IVideoSource
	{
        // URL for MJPEG stream
        private string source;
        // login and password for HTTP authentication
        private string login = null;
		private string password = null;
        // user data associated with the video source
        private object userData = null;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private int bytesReceived;
        // use separate HTTP connection group or use default
        private bool useSeparateConnectionGroup = true;
        // timeout value for web request
        private int requestTimeout = 10000;

        // buffer size used to download MJPEG stream
        private const int bufSize = 512 * 1024;
        // size of portion to read at once
        private const int readSize = 1024;

		private Thread	thread = null;
		private ManualResetEvent stopEvent = null;
		private ManualResetEvent reloadEvent = null;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks>Notifies client about new available frame from video source.</remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>The event is used to notify client about any type error occurred in
        /// video source object, for example exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Use or not separate connection group.
        /// </summary>
        /// 
        /// <remarks>The property indicates to open web request in separate connection group.</remarks>
        /// 
        public bool SeparateConnectionGroup
		{
			get { return useSeparateConnectionGroup; }
			set { useSeparateConnectionGroup = value; }
		}

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>URL, which provides MJPEG stream.</remarks>
        /// 
        public string Source
		{
			get { return source; }
			set
			{
				source = value;
				// signal to reload
				if ( thread != null )
					reloadEvent.Set( );
			}
		}

        /// <summary>
        /// Login value.
        /// </summary>
        /// 
        /// <remarks>Login required to access video source.</remarks>
        /// 
        public string Login
		{
			get { return login; }
			set { login = value; }
		}

        /// <summary>
        /// Password value.
        /// </summary>
        /// 
        /// <remarks>Password required to access video source.</remarks>
        /// 
        public string Password
		{
			get { return password; }
			set { password = value; }
		}

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
		{
			get
			{
				int frames = framesReceived;
				framesReceived = 0;
				return frames;
			}
		}

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int BytesReceived
		{
			get
			{
				int bytes = bytesReceived;
				bytesReceived = 0;
				return bytes;
			}
		}

        /// <summary>
        /// User data.
        /// </summary>
        /// 
        /// <remarks>The property allows to associate user data with video source object.</remarks>
        /// 
        public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

        /// <summary>
        /// Request timeout value.
        /// </summary>
        /// 
        /// <remarks>The property sets timeout value in milliseconds for web requests.
        /// Default value is 10000 milliseconds.</remarks>
        /// 
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object.</remarks>
        /// 
        public bool IsRunning
		{
			get
			{
				if ( thread != null )
				{
                    // check thread status
					if ( thread.Join( 0 ) == false )
						return true;

					// the thread is not running, so free resources
					Free( );
				}
				return false;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MJPEGStream"/> class.
        /// </summary>
        /// 
        public MJPEGStream( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPEGStream"/> class.
        /// </summary>
        /// 
        /// <param name="source">URL, which provides MJPEG stream.</param>
        /// 
        public MJPEGStream( string source )
        {
            this.source = source;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Start video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start( )
		{
			if ( thread == null )
			{
                // check source
                if ( ( source == null ) || ( source == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified" );
                
                framesReceived = 0;
				bytesReceived = 0;

				// create events
				stopEvent	= new ManualResetEvent( false );
				reloadEvent	= new ManualResetEvent( false );
				
				// create and start new thread
				thread = new Thread( new ThreadStart( WorkerThread ) );
				thread.Name = source;
				thread.Start( );
			}
		}

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop( )
		{
			// stop thread
			if ( thread != null )
			{
				// signal to stop
				stopEvent.Set( );
			}
		}

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop( )
		{
			if ( thread != null )
			{
				// wait for thread stop
				thread.Join( );

				Free( );
			}
		}

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks>Stops video source aborting its thread.</remarks>
        /// 
        public void Stop( )
		{
			if ( this.IsRunning )
			{
				thread.Abort( );
				WaitForStop( );
			}
		}

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void Free( )
		{
			thread = null;

			// release events
			stopEvent.Close( );
			stopEvent = null;
			reloadEvent.Close( );
			reloadEvent = null;
		}

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
        public void WorkerThread( )
		{
            // buffer to read stream
            byte[] buffer = new byte[bufSize];

			while ( true )
			{
				// reset reload event
				reloadEvent.Reset( );

                // HTTP web request
				HttpWebRequest request = null;
                // web responce
				WebResponse responce = null;
                // stream for MJPEG downloading
                Stream stream = null;
                // new line delimiters
				byte[] delimiter = null;
				byte[] delimiter2 = null;
                // boundary betweeen images
				byte[]			boundary = null;
                // length of delimiters and boundary
				int boundaryLen, delimiterLen = 0, delimiter2Len = 0;
                // read amounts and positions
				int read, todo = 0, total = 0, pos = 0, align = 1;
				int start = 0, stop = 0;

				// align
				//  1 = searching for image start
				//  2 = searching for image end

				try
				{
					// create request
                    request = (HttpWebRequest) WebRequest.Create( source );
                    // set timeout value for the request
                    request.Timeout = requestTimeout;
                    // set login and password
					if ( ( login != null ) && ( password != null ) && ( login != string.Empty ) )
                        request.Credentials = new NetworkCredential( login, password );
					// set connection group name
					if ( useSeparateConnectionGroup )
                        request.ConnectionGroupName = GetHashCode( ).ToString( );
					// get response
                    responce = request.GetResponse( );

					// check content type
                    string contentType = responce.ContentType;
                    if ( contentType.IndexOf( "multipart/x-mixed-replace" ) == -1 )
                    {
                        // provide information to clients
                        if ( VideoSourceError != null )
                        {
                            VideoSourceError( this, new VideoSourceErrorEventArgs( "Invalid content type" ) );
                        }

                        request.Abort( );
                        request = null;
                        responce.Close( );
                        responce = null;

                        // need to stop ?
                        if ( stopEvent.WaitOne( 0, true ) )
                            break;
                        continue;
                    }

					// get boundary
					ASCIIEncoding encoding = new ASCIIEncoding( );
                    boundary = encoding.GetBytes( contentType.Substring( contentType.IndexOf( "boundary=", 0 ) + 9 ) );
					boundaryLen = boundary.Length;

					// get response stream
                    stream = responce.GetResponseStream( );

					// loop
					while ( ( !stopEvent.WaitOne( 0, true ) ) && ( !reloadEvent.WaitOne( 0, true ) ) )
					{
						// check total read
						if ( total > bufSize - readSize )
						{
							total = pos = todo = 0;
						}

						// read next portion from stream
						if ( ( read = stream.Read( buffer, total, readSize ) ) == 0 )
							throw new ApplicationException( );

						total += read;
						todo += read;

						// increment received bytes counter
						bytesReceived += read;
				
						// do we know the delimiter ?
						if ( delimiter == null )
						{
							// find boundary
							pos = ByteArrayUtils.Find( buffer, boundary, pos, todo );

							if ( pos == -1 )
							{
								// was not found
								todo = boundaryLen - 1;
								pos = total - todo;
								continue;
							}

                            // data to process further
							todo = total - pos;

							if ( todo < 2 )
								continue;

							// check new line delimiter type
							if ( buffer[pos + boundaryLen] == 10 )
							{
								delimiterLen = 2;
								delimiter = new byte[2] { 10, 10 };
								delimiter2Len = 1;
								delimiter2 = new byte[1] { 10 };
							}
							else
							{
								delimiterLen = 4;
								delimiter = new byte[4]  {13, 10, 13, 10 };
								delimiter2Len = 2;
								delimiter2 = new byte[2] { 13, 10 };
							}

							pos += boundaryLen + delimiter2Len;
							todo = total - pos;
						}

						// search for image start
						if ( align == 1 )
						{
							start = ByteArrayUtils.Find( buffer, delimiter, pos, todo );
							if ( start != -1 )
							{
								// found delimiter
								start	+= delimiterLen;
								pos		= start;
								todo	= total - pos;
								align	= 2;
							}
							else
							{
								// delimiter not found
								todo	= delimiterLen - 1;
								pos		= total - todo;
							}
						}

						// search for image end
						while ( ( align == 2 ) && ( todo >= boundaryLen ) )
						{
							stop = ByteArrayUtils.Find( buffer, boundary, pos, todo );
							if (stop != -1)
							{
								pos		= stop;
								todo	= total - pos;

								// increment frames counter
								framesReceived ++;

								// image at stop
								if ( NewFrame != null )
								{
									Bitmap bitmap = (Bitmap) Bitmap.FromStream ( new MemoryStream( buffer, start, stop - start ) );
									// notify client
                                    NewFrame( this, new NewFrameEventArgs( bitmap ) );
									// release the image
                                    bitmap.Dispose( );
                                    bitmap = null;
								}

								// shift array
								pos		= stop + boundaryLen;
								todo	= total - pos;
								Array.Copy( buffer, pos, buffer, 0, todo );

								total	= todo;
								pos		= 0;
								align	= 1;
							}
							else
							{
								// delimiter not found
								todo	= boundaryLen - 1;
								pos		= total - todo;
							}
						}
					}
				}
				catch ( WebException exception )
				{
                    // provide information to clients
                    if ( VideoSourceError != null )
                    {
                        VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );

                    }
                    // wait for a while before the next try
					Thread.Sleep( 250 );
				}
				catch ( ApplicationException )
				{
					// wait for a while before the next try
					Thread.Sleep( 250 );
				}
				catch ( Exception )
				{
				}
				finally
				{
					// abort request
					if ( request != null)
					{
                        request.Abort( );
                        request = null;
					}
					// close response stream
					if ( stream != null )
					{
						stream.Close( );
						stream = null;
					}
					// close response
					if ( responce != null )
					{
                        responce.Close( );
                        responce = null;
					}
				}

				// need to stop ?
				if ( stopEvent.WaitOne( 0, true ) )
					break;
			}
		}
	}
}
