// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Runtime.InteropServices;

    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Video source for local video capture device (for example USB webcam).
    /// </summary>
    /// 
    /// <remarks><para>The video source captures video data from local video capture device.
    /// DirectShow is used for capturing.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // enumerate video devices
    /// videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
    /// // create video source
    /// VideoCaptureDevice videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// // ...
    /// // signal to stop
    /// videoSource.SignalToStop( );
    /// // ...
    /// 
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class VideoCaptureDevice : IVideoSource
    {
        // moniker string of video capture device
        private string deviceMoniker;
        // user data associated with the video source
        private object userData = null;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private int bytesReceived;
        // prevent freezing
        private bool preventFreezing = true;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

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
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>Video source is represented by moniker string of video capture device.</remarks>
        /// 
        public virtual string Source
        {
            get { return deviceMoniker; }
            set { deviceMoniker = value; }
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

                    // the thread is not running, free resources
                    Free( );
                }
                return false;
            }
        }

        /// <summary>
        /// Prevent video freezing after screen saver and workstation lock or not.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The value specifies if the class should prevent video freezing during and
        /// after screen saver or workstation lock. To prevent freezing the <i>DirectShow</i> graph
        /// should not contain  <i>Renderer</i> filter, which is added by <i>Render()</i> method
        /// of graph. However, in some cases it may be required to call <i>Render()</i> method of graph, since
        /// it may add some more filters, which may be required for playing video. So, the property is
        /// a trade off - it is possible to prevent video freezing skipping adding renderer filter or
        /// it is possible to keep renderer filter, but video may freeze during screen saver.</para>
        /// <para>Default value of this property is set to <b>true</b> for capture device.</para>
        /// <para><note>The property may become obsolete in the future if approach to disable freezing
        /// and adding all required filters is found.</note></para>
        /// <para><note>The property should be set before calling <see cref="Start"/> method
        /// of the class.</note></para>
        /// </remarks>
        /// 
        public bool PreventFreezing
        {
            get { return preventFreezing; }
            set { preventFreezing = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        /// 
        public VideoCaptureDevice( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        /// 
        /// <param name="deviceMoniker">Moniker string of video capture device.</param>
        /// 
        public VideoCaptureDevice( string deviceMoniker )
        {
            this.deviceMoniker = deviceMoniker;
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
                if ( ( deviceMoniker == null ) || ( deviceMoniker == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified" );

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent( false );

                // create and start new thread
                thread = new Thread( new ThreadStart( WorkerThread ) );
                thread.Name = deviceMoniker; // mainly for debugging
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
        }

        /// <summary>
        /// Display property window for the video capture device providing its configuration
        /// capabilities.
        /// </summary>
        /// 
        /// <param name="parentWindow">Handle of parent window.</param>
        /// 
        public void DisplayPropertyPage( IntPtr parentWindow )
        {
            // check source
            if ( ( deviceMoniker == null ) || ( deviceMoniker == string.Empty ) )
                throw new ArgumentException( "Video source is not specified" );

            // create source device's object
            object sourceObject = FilterInfo.CreateFilter( deviceMoniker );
            if ( sourceObject == null )
                throw new ApplicationException( "Failed creating device object for moniker" );

            // retrieve ISpecifyPropertyPages interface of the device
            ISpecifyPropertyPages pPropPages = (ISpecifyPropertyPages) sourceObject;

            // get property pages from the property bag
            CAUUID caGUID;
            pPropPages.GetPages( out caGUID );

            // get filter info
            FilterInfo filterInfo = new FilterInfo( deviceMoniker );

            //Create and display the OlePropertyFrame
            // object device = sourceObject;
            Win32.OleCreatePropertyFrame( parentWindow, 0, 0, filterInfo.Name, 1, ref sourceObject, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero );

            // release COM objects
            Marshal.FreeCoTaskMem( caGUID.pElems );
            Marshal.ReleaseComObject( sourceObject );
        }

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
        private void WorkerThread( )
        {
            // grabber
            Grabber grabber = new Grabber( this );

            // objects
            object graphObject = null;
            object sourceObject = null;
            object grabberObject = null;

            // interfaces
            IGraphBuilder   graph = null;
            IBaseFilter     sourceBase = null;
            IBaseFilter     grabberBase = null;
            ISampleGrabber  sampleGrabber = null;
            IMediaControl   mediaControl = null;

            try
            {
                // get type for filter graph
                Type type = Type.GetTypeFromCLSID( Clsid.FilterGraph );
                if ( type == null )
                    throw new ApplicationException( "Failed creating filter graph" );

                // create filter graph
                graphObject = Activator.CreateInstance( type );
                graph = (IGraphBuilder) graphObject;

                // create source device's object
                sourceObject = FilterInfo.CreateFilter( deviceMoniker );
                if ( sourceObject == null )
                    throw new ApplicationException( "Failed creating device object for moniker" );

                // get base filter interface of source device
                sourceBase = (IBaseFilter) sourceObject;

                // get type for sample grabber
                type = Type.GetTypeFromCLSID( Clsid.SampleGrabber );
                if ( type == null )
                    throw new ApplicationException( "Failed creating sample grabber" );

                // create sample grabber
                grabberObject = Activator.CreateInstance( type );
                sampleGrabber = (ISampleGrabber) grabberObject;
                grabberBase = (IBaseFilter) grabberObject;

                // add source and grabber filters to graph
                graph.AddFilter( sourceBase, "source" );
                graph.AddFilter( grabberBase, "grabber" );

                // set media type
                AMMediaType mediaType = new AMMediaType( );
                mediaType.MajorType = MediaType.Video;
                mediaType.SubType = MediaSubType.RGB24;
                sampleGrabber.SetMediaType( mediaType );

                // connect pins
                if ( graph.Connect( Tools.GetOutPin( sourceBase, 0 ), Tools.GetInPin( grabberBase, 0 ) ) < 0 )
                    throw new ApplicationException( "Failed connecting filters" );

				// get media type
                if ( sampleGrabber.GetConnectedMediaType( mediaType ) == 0 )
                {
                    VideoInfoHeader vih = (VideoInfoHeader) Marshal.PtrToStructure( mediaType.FormatPtr, typeof( VideoInfoHeader ) );

                    grabber.Width = vih.BmiHeader.Width;
                    grabber.Height = vih.BmiHeader.Height;
                    mediaType.Dispose( );
                }

                // let's do rendering, if we don't need to prevent freezing
                if ( !preventFreezing )
                {
                    // render pin
                    graph.Render( Tools.GetOutPin( grabberBase, 0 ) );

                    // configure video window
                    IVideoWindow window = (IVideoWindow) graphObject;
                    window.put_AutoShow( false );
                    window = null;
                }

                // configure sample grabber
                sampleGrabber.SetBufferSamples( false );
                sampleGrabber.SetOneShot( false );
                sampleGrabber.SetCallback( grabber, 1 );

                // get media control
                mediaControl = (IMediaControl) graphObject;

                // run
                mediaControl.Run( );

                while ( !stopEvent.WaitOne( 0, true ) )
                {
                    Thread.Sleep( 100 );
                }
                mediaControl.StopWhenReady( );
            }
            catch ( Exception exception )
            {
                // provide information to clients
                if ( VideoSourceError != null )
                {
                    VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                }
            }
            finally
            {
                // release all objects
                graph           = null;
                sourceBase      = null;
                grabberBase     = null;
                sampleGrabber   = null;
                mediaControl    = null;

                if ( graphObject != null )
                {
                    Marshal.ReleaseComObject( graphObject );
                    graphObject = null;
                }
                if ( sourceObject != null )
                {
                    Marshal.ReleaseComObject( sourceObject );
                    sourceObject = null;
                }
                if ( grabberObject != null )
                {
                    Marshal.ReleaseComObject( grabberObject );
                    grabberObject = null;
                }
            }
        }

        /// <summary>
        /// Notifies client about new frame.
        /// </summary>
        /// 
        /// <param name="image">New frame's image.</param>
        /// 
        protected void OnNewFrame( Bitmap image )
        {
            framesReceived++;
            if ( ( !stopEvent.WaitOne( 0, true ) ) && ( NewFrame != null ) )
                NewFrame( this, new NewFrameEventArgs( image ) );
        }

        //
        // Video grabber
        //
        private class Grabber : ISampleGrabberCB
        {
            private VideoCaptureDevice parent;
            private int width, height;

            // Width property
            public int Width
            {
                get { return width; }
                set { width = value; }
            }
            // Height property
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            // Constructor
            public Grabber( VideoCaptureDevice parent )
            {
                this.parent = parent;
            }

            // Callback to receive samples
            public int SampleCB( double sampleTime, IntPtr sample )
            {
                return 0;
            }

			// Callback method that receives a pointer to the sample buffer
            public int BufferCB( double sampleTime, IntPtr buffer, int bufferLen )
            {
                // create new image
                System.Drawing.Bitmap image = new Bitmap( width, height, PixelFormat.Format24bppRgb );

                // lock bitmap data
                BitmapData imageData = image.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb );

                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = imageData.Stride;

                int dst = imageData.Scan0.ToInt32( ) + dstStride * ( height - 1 );
                int src = buffer.ToInt32( );

                for ( int y = 0; y < height; y++ )
                {
                    Win32.memcpy( dst, src, srcStride );
                    dst -= dstStride;
                    src += srcStride;
                }

                // unlock bitmap data
                image.UnlockBits( imageData );

                // notify parent
                parent.OnNewFrame( image);

                // release the image
                image.Dispose( );

                return 0;
            }
        }
    }
}
