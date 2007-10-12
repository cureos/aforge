// AForge.NET Framework
// Motion Detection sample application
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace MotionDetector
{
	using System;
	using System.Drawing;
	using System.Threading;

    using AForge.Video;
    using AForge.Vision.Motion;

	/// <summary>
	/// Camera class
	/// </summary>
    /// 
	public class Camera
	{
		private IVideoSource	videoSource = null;
		private IMotionDetector	motionDetecotor = null;
		private Bitmap			lastFrame = null;
        private string          lastVideoSourceError = null;

		// image dimension
		private int width = -1;
        private int height = -1;

		// alarm level (0.5%)
		private double	alarmLevel = 0.005;

		// public events
		public event EventHandler NewFrame;
		public event EventHandler Alarm;
        public event EventHandler VideoSourceError;

		// Last video frame
		public Bitmap LastFrame
		{
			get { return lastFrame; }
		}

        // Last video source error
        public string LastVideoSourceError
        {
            get { return lastVideoSourceError; }
        }

		// Video frame width
		public int Width
		{
			get { return width; }
		}

		// Vodeo frame height
		public int Height
		{
			get { return height; }
		}

		// Frames received from the last access to the property
		public int FramesReceived
		{
			get { return ( videoSource == null ) ? 0 : videoSource.FramesReceived; }
		}

        // Bytes received from the last access to the property
		public int BytesReceived
		{
			get { return ( videoSource == null ) ? 0 : videoSource.BytesReceived; }
		}

		// Running property
		public bool IsRunning
		{
			get { return ( videoSource == null ) ? false : videoSource.IsRunning; }
		}

		// Motion detector used for motion detection
		public IMotionDetector MotionDetector
		{
			get { return motionDetecotor; }
			set { motionDetecotor = value; }
		}

		// Constructor
		public Camera( IVideoSource source ) : this( source, null ) { }

        public Camera( IVideoSource source, IMotionDetector detector )
		{
			this.videoSource = source;
			this.motionDetecotor = detector;
			videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
            videoSource.VideoSourceError += new VideoSourceErrorEventHandler( video_VideoSourceError );
		}

		// Start video source
		public void Start( )
		{
			if ( videoSource != null )
			{
				videoSource.Start( );
			}
		}

		// Siganl video source to stop
		public void SignalToStop( )
		{
			if ( videoSource != null )
			{
				videoSource.SignalToStop( );
			}
		}

		// Wait video source for stop
		public void WaitForStop( )
		{
			// lock
			Monitor.Enter( this );

			if ( videoSource != null )
			{
				videoSource.WaitForStop( );
			}
			// unlock
			Monitor.Exit( this );
		}

		// Abort camera
		public void Stop( )
		{
			// lock
			Monitor.Enter( this );

			if ( videoSource != null )
			{
				videoSource.Stop( );
			}
			// unlock
			Monitor.Exit( this );
		}

		// Lock it
		public void Lock( )
		{
			Monitor.Enter( this );
		}

		// Unlock it
		public void Unlock( )
		{
			Monitor.Exit( this );
		}

		// On new frame
		private void video_NewFrame( object sender, NewFrameEventArgs e )
		{
			try
			{
				// lock
				Monitor.Enter( this );

				// dispose old frame
				if ( lastFrame != null )
				{
					lastFrame.Dispose( );
				}

                // reset error
                lastVideoSourceError = null;
                // get new frame
				lastFrame = (Bitmap) e.Frame.Clone( );

				// apply motion detector
				if ( motionDetecotor != null )
				{
					motionDetecotor.ProcessFrame( lastFrame );

					// check motion level
					if (
						( motionDetecotor.MotionLevel >= alarmLevel ) &&
						( Alarm != null )
						)
					{
						Alarm( this, new EventArgs( ) );
					}
				}

				// image dimension
				width = lastFrame.Width;
				height = lastFrame.Height;
			}
			catch ( Exception )
			{
			}
			finally
			{
				// unlock
				Monitor.Exit( this );
			}

			// notify client
			if ( NewFrame != null )
				NewFrame( this, new EventArgs( ) );
		}

        // On video source error
        private void video_VideoSourceError( object sender, VideoSourceErrorEventArgs e )
        {
            // reset motion detector
            if ( motionDetecotor != null )
            {
                motionDetecotor.Reset( );
            }

            // save video source error's description
            lastVideoSourceError = e.Description;

            // notify clients about the error
            if ( VideoSourceError != null )
            {
                VideoSourceError( this, new EventArgs( ) );
            }
        }
	}
}
