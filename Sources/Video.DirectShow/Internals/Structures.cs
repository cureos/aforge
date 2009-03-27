// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    // PIN_DIRECTION

    /// <summary>
    /// This enumeration indicates a pin's direction.
    /// </summary>
    /// 
    [ComVisible( false )]
    internal enum PinDirection
    {
        /// <summary>
        /// Input pin.
        /// </summary>
        Input,

        /// <summary>
        /// Output pin.
        /// </summary>
        Output
    }

    // AM_MEDIA_TYPE

    /// <summary>
    /// The structure describes the format of a media sample.
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential )]
    internal class AMMediaType : IDisposable
    {
        /// <summary>
        /// Globally unique identifier (GUID) that specifies the major type of the media sample.
        /// </summary>
        public Guid MajorType;

        /// <summary>
        /// GUID that specifies the subtype of the media sample.
        /// </summary>
        public Guid SubType;

        /// <summary>
        /// If <b>true</b>, samples are of a fixed size.
        /// </summary>
        [MarshalAs( UnmanagedType.Bool )]
        public bool FixedSizeSamples = true;

        /// <summary>
        /// If <b>true</b>, samples are compressed using temporal (interframe) compression.
        /// </summary>
        [MarshalAs( UnmanagedType.Bool )]
        public bool TemporalCompression;

        /// <summary>
        /// Size of the sample in bytes. For compressed data, the value can be zero.
        /// </summary>
        public int SampleSize = 1;

        /// <summary>
        /// GUID that specifies the structure used for the format block.
        /// </summary>
        public Guid FormatType;

        /// <summary>
        /// Not used.
        /// </summary>
        public IntPtr unkPtr;

        /// <summary>
        /// Size of the format block, in bytes.
        /// </summary>
        public int FormatSize;

        /// <summary>
        /// Pointer to the format block.
        /// </summary>
        public IntPtr FormatPtr;

        /// <summary>
        /// Destroys the instance of the <see cref="AMMediaType"/> class.
        /// </summary>
        /// 
        ~AMMediaType( )
        {
            Dispose( false );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
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
        /// <param name="disposing">Indicates if disposing was initiated manually.</param>
        /// 
        protected virtual void Dispose( bool disposing )
        {
            if ( FormatSize != 0 )
                Marshal.FreeCoTaskMem( FormatPtr );
            if ( unkPtr != IntPtr.Zero )
                Marshal.Release( unkPtr );
        }
    }


    // PIN_INFO

    /// <summary>
    /// The structure contains information about a pin.
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode )]
    internal struct PinInfo
    {
        /// <summary>
        /// Owning filter.
        /// </summary>
        public IBaseFilter Filter;

        /// <summary>
        /// Direction of the pin.
        /// </summary>
        public PinDirection Direction;

        /// <summary>
        /// Name of the pin.
        /// </summary>
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
        public string Name;
    }

    // FILTER_INFO
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode )]
    internal struct FilterInfo
    {
        /// <summary>
        /// Filter's name.
        /// </summary>
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
        public string Name;

        /// <summary>
        /// Owning graph.
        /// </summary>
        public IFilterGraph FilterGraph;
    }

    // VIDEOINFOHEADE

    /// <summary>
    /// The structure describes the bitmap and color information for a video image.
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential )]
    internal struct VideoInfoHeader
    {
        /// <summary>
        /// <see cref="RECT"/> structure that specifies the source video window.
        /// </summary>
        public RECT SrcRect;

        /// <summary>
        /// <see cref="RECT"/> structure that specifies the destination video window.
        /// </summary>
        public RECT TargetRect;

        /// <summary>
        /// Approximate data rate of the video stream, in bits per second.
        /// </summary>
        public int BitRate;

        /// <summary>
        /// Data error rate, in bit errors per second.
        /// </summary>
        public int BitErrorRate;

        /// <summary>
        /// The desired average display time of the video frames, in 100-nanosecond units.
        /// </summary>
        public long AverageTimePerFrame;

        /// <summary>
        /// <see cref="BitmapInfoHeader"/> structure that contains color and dimension information for the video image bitmap.
        /// </summary>
        public BitmapInfoHeader BmiHeader;
    }

    // BITMAPINFOHEADER

    /// <summary>
    /// The structure contains information about the dimensions and color format of a device-independent bitmap (DIB).
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential, Pack = 2 )]
    internal struct BitmapInfoHeader
    {
        /// <summary>
        /// Specifies the number of bytes required by the structure.
        /// </summary>
        public int Size;

        /// <summary>
        /// Specifies the width of the bitmap.
        /// </summary>
        public int Width;

        /// <summary>
        /// Specifies the height of the bitmap, in pixels.
        /// </summary>
        public int Height;

        /// <summary>
        /// Specifies the number of planes for the target device. This value must be set to 1.
        /// </summary>
        public short Planes;

        /// <summary>
        /// Specifies the number of bits per pixel.
        /// </summary>
        public short BitCount;

        /// <summary>
        /// If the bitmap is compressed, this member is a <b>FOURCC</b> the specifies the compression.
        /// </summary>
        public int Compression;

        /// <summary>
        /// Specifies the size, in bytes, of the image.
        /// </summary>
        public int ImageSize;

        /// <summary>
        /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
        /// </summary>
        public int XPelsPerMeter;

        /// <summary>
        /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap.
        /// </summary>
        public int YPelsPerMeter;

        /// <summary>
        /// Specifies the number of color indices in the color table that are actually used by the bitmap.
        /// </summary>
        public int ColorsUsed;

        /// <summary>
        /// Specifies the number of color indices that are considered important for displaying the bitmap.
        /// </summary>
        public int ColorsImportant;
    }

    // RECT

    /// <summary>
    /// The structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential )]
    internal struct RECT
    {
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Left;

        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle. 
        /// </summary>
        public int Top;

        /// <summary>
        /// Specifies the x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Right;

        /// <summary>
        /// Specifies the y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Bottom;
    }

    // CAUUID

    /// <summary>
    /// The CAUUID structure is a Counted Array of UUID or GUID types.
    /// </summary>
    /// 
    [ComVisible( false ),
    StructLayout( LayoutKind.Sequential )]
    internal struct CAUUID
    {
        /// <summary>
        /// Size of the array pointed to by <b>pElems</b>.
        /// </summary>
        public int cElems;

        /// <summary>
        /// Pointer to an array of UUID values, each of which specifies UUID.
        /// </summary>
        public IntPtr pElems;

        /// <summary>
        /// Performs manual marshaling of <b>pElems</b> to retrieve an array of Guid objects.
        /// </summary>
        /// 
        /// <returns>A managed representation of <b>pElems</b>.</returns>
        /// 
        public Guid[] ToGuidArray( )
        {
            Guid[] retval = new Guid[cElems];

            for ( int i = 0; i < cElems; i++ )
            {
                IntPtr ptr = new IntPtr( pElems.ToInt64( ) + i * Marshal.SizeOf( typeof( Guid ) ) );
                retval[i] = (Guid) Marshal.PtrToStructure( ptr, typeof( Guid ) );
            }

            return retval;
        }
    }
}
