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
    /// Video source for video files.
    /// </summary>
    /// 
    /// <remarks><para>The video source provides access to video files. DirectShow is used to access video
    /// files.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source
    /// FileVideoSource videoSource = new FileVideoSource( fileName );
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
    public class FileVideoSource : IVideoSource
    {
        // video file name
        private string fileName;
        // user data associated with the video source
        private object userData = null;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private int bytesReceived;

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
        /// <remarks>Video source is represented by video file name.</remarks>
        /// 
        public virtual string Source
        {
            get { return fileName; }
            set { fileName = value; }
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
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        public FileVideoSource( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="fileName">Video file name.</param>
        /// 
        public FileVideoSource( string fileName )
        {
            this.fileName = fileName;
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
                if ( ( fileName == null ) || ( fileName == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified" );

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent( false );

                // create and start new thread
                thread = new Thread( new ThreadStart( WorkerThread ) );
                thread.Name = fileName; // mainly for debugging
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
            IGraphBuilder       graph = null;
            IBaseFilter         sourceBase = null;
            IBaseFilter         grabberBase = null;
            ISampleGrabber      sampleGrabber = null;
            IMediaControl       mediaControl = null;
            IFileSourceFilter   fileSource = null;

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
                type = Type.GetTypeFromCLSID( Clsid.AsyncReader );
                if ( type == null )
                    throw new ApplicationException( "Failed creating filter async reader" );

                sourceObject = Activator.CreateInstance( type );
                sourceBase = (IBaseFilter) sourceObject;
                fileSource = (IFileSourceFilter) sourceObject;

                fileSource.Load( fileName, null );

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

                // render pin
                graph.Render( Tools.GetOutPin( grabberBase, 0 ) );

                // configure sample grabber
                sampleGrabber.SetBufferSamples( false );
                sampleGrabber.SetOneShot( false );
                sampleGrabber.SetCallback( grabber, 1 );

                // configure video window
                IVideoWindow window = (IVideoWindow) graphObject;
                window.put_AutoShow( false );
                window = null;

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
                fileSource      = null;

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
        // Vodeo grabber
        //
        private class Grabber : ISampleGrabberCB
        {
            private FileVideoSource parent;
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
            public Grabber( FileVideoSource parent )
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
                parent.OnNewFrame( image );

                // release the image
                image.Dispose( );

                return 0;
            }
        }
    }
}
