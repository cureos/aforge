// AForge Kinect Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

using System;
using System.Runtime.InteropServices;

namespace AForge.Video.Ximea.Internal
{
    internal enum XimeaImageFormat
    {
        Grayscale8 = 0,
        Grayscale16 = 1,
        RGB24 = 2,
        RGB32 = 3
    }

    [StructLayout( LayoutKind.Sequential )]
    internal struct XimeaImage
    {
        public int StructSize;
        public IntPtr BitmapData;
        public int Size;
        public XimeaImageFormat PixelFormat;
        public int Width;
        public int Height;
        public int FrameNumber;
    }
}
