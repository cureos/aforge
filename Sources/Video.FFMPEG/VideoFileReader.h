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
	public ref class VideoFileReader
	{
	public:
		VideoFileReader( void );

		void Open( String^ fileName );

		Bitmap^ ReadVideoFrame( );

		void Close( );

	private:

		libffmpeg::AVFormatContext*		m_formatContext;
		libffmpeg::AVStream*			m_stream;
		libffmpeg::AVCodecContext*		m_codecContext;
		libffmpeg::AVFrame*				m_videoFrame;
		struct libffmpeg::SwsContext*	m_convertContext;
	};

} } }
