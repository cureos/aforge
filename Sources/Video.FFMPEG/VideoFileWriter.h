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

namespace libffmpeg
{
	extern "C"
	{
		#include "libavformat\avformat.h"
		#include "libavformat\avio.h"
		#include "libavcodec\avcodec.h"
		#include "libswscale\swscale.h"
	}
}

namespace AForge { namespace Video { namespace FFMPEG
{
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

	private:

		libffmpeg::AVStream* add_video_stream( libffmpeg::AVFormatContext* formatContext, enum libffmpeg::CodecID codec_id );
		void open_video( libffmpeg::AVFormatContext* formatContext, libffmpeg::AVStream* stream );
		void write_video_frame( );

	private:

		libffmpeg::AVFormatContext*		m_formatContext;
		libffmpeg::AVStream*			m_videoStream;
		libffmpeg::AVFrame*				m_videoFrame;
		struct libffmpeg::SwsContext*	m_convertContext;
		struct libffmpeg::SwsContext*	m_convertContextGrayscale;

		libffmpeg::uint8_t*	m_videoOutputBuffer;
		int	m_videoOutputBufferSize;
	};

} } }
