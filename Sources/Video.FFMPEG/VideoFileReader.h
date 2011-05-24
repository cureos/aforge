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
		property int Width
		{
			int get( ) 
			{
				CheckIfVideoFileIsOpen( );
				return m_codecContext->width;
			}
		}
		property int Height
		{
			int get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_codecContext->height;
			}
		}
		property int FrameRate
		{
			int get( )
			{
				CheckIfVideoFileIsOpen( );
				return m_stream->r_frame_rate.den / m_stream->r_frame_rate.num;
			}
		}

		property Int64 FrameCount
		{
			Int64 get( )
			{
				CheckIfVideoFileIsOpen( );
				return Int64( m_stream->nb_frames );
			}
		}

		property String^ CodecName
		{
			String^ get( )
			{
				CheckIfVideoFileIsOpen( );
				return gcnew String( m_codecContext->codec->name );
			}
		}

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

		libffmpeg::AVPacket* m_packet;
		libffmpeg::uint8_t* m_rawData;
		int m_bytesRemaining;

	private:
		Bitmap^ DecodeVideoFrame( );

	private:

		void CheckIfVideoFileIsOpen( )
		{
			if ( m_formatContext == NULL )
			{
				throw gcnew System::IO::IOException( "Video file is not open, so can not access its properties." );
			}
		}
	};

} } }
