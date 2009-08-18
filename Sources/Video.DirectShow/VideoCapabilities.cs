// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2009
// andrew.kirillov@gmail.com

// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2003-Jan-24		BL		- created
//
// Copyright (c) 2003 Brian Low
// ------------------------------------------------------------------
// Adapted for AForge, Yves Vander Haeghen, 2009
//
// Changed a lot from the original by Andrew Kirillov to fit AForge.NET framework, 2009
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Capabilities of video device such as frame size and frame rate.
    /// </summary>
    public class VideoCapabilities
    {
        /// <summary>
        /// Frame size supported by video device.
        /// </summary>
        public readonly Size FrameSize;

        /// <summary>
        /// Maximum frame rate supported by video device for corresponding <see cref="FrameSize">frame size</see>.
        /// </summary>
        public readonly int MaxFrameRate;

        internal VideoCapabilities( ) { }

        // Retrieve capabilities of a video device
        static internal VideoCapabilities[] FromStreamConfig( IAMStreamConfig videoStreamConfig )
        {
            if ( videoStreamConfig == null )
                throw new ArgumentNullException( "videoStreamConfig" );

            // ensure this device reports capabilities
            int count, size;
            int hr = videoStreamConfig.GetNumberOfCapabilities( out count, out size );

            if ( hr != 0 )
                Marshal.ThrowExceptionForHR( hr );

            if ( count <= 0 )
                throw new NotSupportedException( "This video device does not report capabilities." );

            if ( size > Marshal.SizeOf( typeof( VideoStreamConfigCaps ) ) )
                throw new NotSupportedException( "Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure." );

            Dictionary<uint, VideoCapabilities> videocapsList = new Dictionary<uint, VideoCapabilities>( );

            for ( int i = 0; i < count; i++ )
            {
                // vidcaps[i] = new VideoCapabilities( videoStreamConfig, i );
                VideoCapabilities vc = new VideoCapabilities( videoStreamConfig, i );

                uint key = ( ( (uint) vc.FrameSize.Height ) << 16 ) | (uint) vc.FrameSize.Width;

                if ( !videocapsList.ContainsKey( key ) )
                {
                    videocapsList.Add( key, vc );
                }
            }

            VideoCapabilities[] videocaps = new VideoCapabilities[videocapsList.Count];
            videocapsList.Values.CopyTo( videocaps, 0 );

            return videocaps;
        }

        // Retrieve capabilities of a video device
        internal VideoCapabilities( IAMStreamConfig videoStreamConfig, int index )
        {
            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = new VideoStreamConfigCaps( );

            try
            {
                // retrieve capabilities struct at the specified index
                int hr = videoStreamConfig.GetStreamCaps( index, out mediaType, caps );

                if ( hr != 0 )
                    Marshal.ThrowExceptionForHR( hr );

                // extract info
                FrameSize    = caps.InputSize;
                MaxFrameRate = (int) ( 10000000 / caps.MinFrameInterval );
            }
            finally
            {
                if ( mediaType != null )
                    mediaType.Dispose( );
            }
        }
    }
}
