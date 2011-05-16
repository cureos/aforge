// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace AForge::Video;

#include "VideoCodec.h"

namespace AForge { namespace Video { namespace FFMPEG
{
	ref struct PrivateData;

	/// <summary>
	/// Class for writing video files utilizing FFmpeg library.
	/// </summary>
	public ref class VideoFileWriter
	{
	public:

		property int Width
		{
			int get( ) { return m_width; }
		}
		property int Height
		{
			int get( ) { return m_height; }
		}
		property int FrameRate
		{
			int get( ) { return m_frameRate; }
		}

		/// <summary>
		/// Codec to use for the video file. <see cref="VideoCodec"/> 
		/// </summary>
		property VideoCodec  Codec
		{
			VideoCodec get( ) { return m_codec; }
		}

	public:

		VideoFileWriter( void );

		void Open( String^ fileName, int width, int height );
		void Open( String^ fileName, int width, int height, int frameRate );
		void Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec );

		void WriteVideoFrame( Bitmap^ frame );

		void Close( );

	private:

		int m_width;
		int m_height;
		int	m_frameRate;
		VideoCodec m_codec;

		PrivateData^ data;
	};

} } }
