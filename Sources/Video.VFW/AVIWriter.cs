// AForge Video for Windows Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//
namespace AForge.Video.VFW
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
    using AForge;

	/// <summary>
	/// AVI files writing using Video for Windows.
	/// </summary>
    /// 
    /// <remarks><para>The class allows to write AVI files using Video for Windows API.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate AVI writer, use WMV3 codec
    /// AVIWriter writer = new AVIWriter( "wmv3" );
    /// // create new AVI file and open it
    /// writer.Open( "test.avi", 320, 240 );
    /// // create frame image
    /// Bitmap image = new Bitmap( 320, 240 );
    /// 
    /// for ( int i = 0; i &lt; 240; i++ )
    /// {
    ///     // update image
    ///     image.SetPixel( i, i, Color.Red );
    ///     // add the image as a new frame of video file
    ///     writer.AddFrame( image );
    /// }
    /// writer.Close( );
    /// </code>
    /// </remarks>
    /// 
	public class AVIWriter : IDisposable
	{
        // AVI file
        private IntPtr file;
        // video stream
        private IntPtr stream;
        // compressed stream
		private IntPtr streamCompressed;
        // buffer
		private IntPtr buffer = IntPtr.Zero;

        // width of video frames
        private int width;
        // height of vide frames
        private int height;
        // length of one line
		private int stride;
        // quality
		private int quality = -1;
        // frame rate
		private int rate = 25;
        // current position
		private int position;
        // codec used for video compression
        private string codec = "DIB ";

        /// <summary>
        /// Width of video frames.
        /// </summary>
        /// 
        public int Width
        {
            get { return ( buffer != IntPtr.Zero ) ? width : 0; }
        }

        /// <summary>
        /// Height of video frames.
        /// </summary>
        /// 
        public int Height
        {
            get { return ( buffer != IntPtr.Zero ) ? height : 0; }
        }

        /// <summary>
        /// Current position in video stream.
        /// </summary>
        /// 
        public int Position
		{
			get { return position; }
		}

        /// <summary>
        /// Desired playing frame rate.
        /// </summary>
        /// 
        /// <remarks>The property should be set befor opening new file to take effect.
        /// Default frame rate is set to 25.</remarks>
        /// 
        public int FrameRate
        {
            get { return rate; }
            set { rate = value; }
        }

        /// <summary>
        /// Codec used for video compression.
        /// </summary>
        /// 
        /// <remarks>The property should be set befor opening new file to take effect.
        /// Default video codec is set "DIB ".</remarks>
        /// 
        public string Codec
		{
			get { return codec; }
			set { codec = value; }
		}

        /// <summary>
        /// Compression video quality.
        /// </summary>
        /// 
        /// <remarks>The property should be set befor opening new file to take effect.
        /// Default value is -1 - default compression quality of the codec.</remarks>
        /// 
		public int Quality
		{
			get { return quality; }
			set { quality = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        /// 
        /// <remarks>Initializes Video for Windows library.</remarks>
        /// 
        public AVIWriter( )
		{
			Win32.AVIFileInit();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        /// 
        /// <param name="codec">Codec to use for compression.</param>
        /// 
        /// <remarks>Initializes Video for Windows library.</remarks>
        /// 
        public AVIWriter( string codec )
            : this( )
		{
			this.codec = codec;
		}

        /// <summary>
        /// Destroys the instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        /// 
        ~AVIWriter( )
		{
			Dispose(false);
		}

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <remarks>Frees unmanaged resources used by the object. The object becomes unusable
        /// after that.</remarks>
        /// 
        public void Dispose( )
		{
			Dispose( true );
			// remove me from the Finalization queue 
			GC.SuppressFinalize( this );
		}

        /// <summary>
        /// Dispose the object
        /// </summary>
        /// 
        /// <param name="disposing">Indicates if disposing was initiated manually</param>
        /// 
        protected virtual void Dispose( bool disposing )
		{
			if ( disposing )
			{
				// dispose managed resources
			}
            // close current AVI file if any opened and uninitialize AVI library
            Close( );
			Win32.AVIFileExit( );
		}

        /// <summary>
        /// Create new AVI file and open it for writing.
        /// </summary>
        /// 
        /// <param name="fileName">AVI file name to create.</param>
        /// <param name="width">Video width.</param>
        /// <param name="height">Video height.</param>
        /// 
        /// <remarks>This method throws <see cref="System.ApplicationException"/> in the case
        /// of failure.</remarks>
        /// 
        public void Open( string fileName, int width, int height )
		{
			// close previous file
			Close( );

			// calculate stride
			stride = width * 3;
			if ( ( stride % 4 ) != 0 )
				stride += (4 - stride % 4 );

			// create new file
			if ( Win32.AVIFileOpen( out file, fileName, Win32.OpenFileMode.Create | Win32.OpenFileMode.Write, IntPtr.Zero ) != 0 )
				throw new ApplicationException( "Failed opening file" );

			this.width = width;
			this.height = height;

			// describe new stream
			Win32.AVISTREAMINFO info = new Win32.AVISTREAMINFO( );

			info.type                   = Win32.mmioFOURCC( "vids" );
			info.handler                = Win32.mmioFOURCC( codec );
			info.scale                  = 1;
			info.rate                   = rate;
			info.suggestedBufferSize    = stride * height;

			// create stream
			if ( Win32.AVIFileCreateStream( file, out stream, ref info ) != 0)
				throw new ApplicationException( "Failed creating stream" );

			// describe compression options
			Win32.AVICOMPRESSOPTIONS options = new Win32.AVICOMPRESSOPTIONS( );

            options.handler = Win32.mmioFOURCC( codec );
            options.quality = quality;

			//
			// Win32.AVISaveOptions(stream, ref opts, IntPtr.Zero);
			
			// create compressed stream
			if ( Win32.AVIMakeCompressedStream( out streamCompressed, stream, ref options, IntPtr.Zero ) != 0 )
				throw new ApplicationException( "Failed creating compressed stream" );

			// describe frame format
			Win32.BITMAPINFOHEADER bitmapInfoHeader = new Win32.BITMAPINFOHEADER( );

            bitmapInfoHeader.size           = Marshal.SizeOf( bitmapInfoHeader.GetType( ) );
            bitmapInfoHeader.width          = width;
            bitmapInfoHeader.height         = height;
            bitmapInfoHeader.planes         = 1;
            bitmapInfoHeader.bitCount       = 24;
            bitmapInfoHeader.sizeImage      = 0;
            bitmapInfoHeader.compression    = 0; // BI_RGB

			// set frame format
            if ( Win32.AVIStreamSetFormat( streamCompressed, 0, ref bitmapInfoHeader, Marshal.SizeOf( bitmapInfoHeader.GetType( ) ) ) != 0 )
				throw new ApplicationException( "Failed creating compressed stream" );

			// alloc unmanaged memory for frame
			buffer = Marshal.AllocHGlobal( stride * height );

			position = 0;
		}

        /// <summary>
        /// Close video file
        /// </summary>
        /// 
        public void Close( )
		{
			// free unmanaged memory
            if ( buffer != IntPtr.Zero )
			{
                Marshal.FreeHGlobal( buffer );
                buffer = IntPtr.Zero;
			}

			// release compressed stream
			if ( streamCompressed != IntPtr.Zero )
			{
				Win32.AVIStreamRelease( streamCompressed );
				streamCompressed = IntPtr.Zero;
			}

			// release stream
			if ( stream != IntPtr.Zero )
			{
				Win32.AVIStreamRelease( stream );
				stream = IntPtr.Zero;
			}

			// release file
			if ( file != IntPtr.Zero )
			{
				Win32.AVIFileRelease( file );
				file = IntPtr.Zero;
			}
		}

        /// <summary>
        /// Add new frame to the AVI file 
        /// </summary>
        /// 
        /// <param name="frameImage">New frame image</param>
        /// 
        /// <remarks>This method throws <see cref="System.ApplicationException"/> in the case
        /// of failure.</remarks>
        /// 
        public void AddFrame( Bitmap frameImage )
		{
			// check image dimension
            if ( ( frameImage.Width != width ) || ( frameImage.Height != height ) )
				throw new ApplicationException( "Invalid image dimension" );

			// lock bitmap data
            BitmapData imageData = frameImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// copy image data
			int srcStride = imageData.Stride;
			int dstStride = stride;

            int src = imageData.Scan0.ToInt32( ) + srcStride * ( height - 1 );
			int dst = buffer.ToInt32( );

			for ( int y = 0; y < height; y++ )
			{
				AForge.Win32.memcpy( dst, src, dstStride );
				dst += dstStride;
				src -= srcStride;
			}

			// unlock bitmap data
            frameImage.UnlockBits( imageData );

			// write to stream
			if ( Win32.AVIStreamWrite( streamCompressed, position, 1, buffer,
				stride * height, 0, IntPtr.Zero, IntPtr.Zero ) != 0)
				throw new ApplicationException( "Failed adding frame" );

			position++;
		}
	}
}
