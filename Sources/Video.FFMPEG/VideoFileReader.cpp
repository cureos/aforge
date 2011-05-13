// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#include "StdAfx.h"
#include "VideoFileReader.h"

namespace AForge { namespace Video { namespace FFMPEG
{

VideoFileReader::VideoFileReader( void )
{
	m_formatContext  = NULL;
	m_stream         = NULL;
	m_codecContext   = NULL;
	m_videoFrame     = NULL;
	m_convertContext = NULL;

	libffmpeg::av_register_all( );
}

#pragma managed(push, off)
static libffmpeg::AVFormatContext*	open_file( char* fileName )
{
	libffmpeg::AVFormatContext* formatContext;

	if ( libffmpeg::av_open_input_file( &formatContext, fileName, NULL, 0, NULL ) !=0 )
	{
		return NULL;
	}
	return formatContext;
}
#pragma managed(pop)

void VideoFileReader::Open( String^ fileName )
{
	// close previous file if any open
	Close( );

	bool success = false;

	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi( fileName );
	char* nativeFileName = reinterpret_cast<char*>( static_cast<void*>( ptr ) );

	try
	{
		// open the specified video file
		m_formatContext = open_file( nativeFileName );
		if ( m_formatContext == NULL )
		{
			throw gcnew System::IO::IOException( "Cannot open the video file." );
		}

		// retrieve stream information
		if ( libffmpeg::av_find_stream_info( m_formatContext ) < 0 )
		{
			throw gcnew VideoException( "Cannot find stream information." );
		}

		// search for the first video stream
		for ( unsigned int i = 0; i < m_formatContext->nb_streams; i++ )
		{
			if( m_formatContext->streams[i]->codec->codec_type == libffmpeg::CODEC_TYPE_VIDEO )
			{
				// get the pointer to the codec context for the video stream
				m_codecContext = m_formatContext->streams[i]->codec;
				m_stream = m_formatContext->streams[i];
				break;
			}
		}
		if ( m_stream == NULL )
		{
			throw gcnew VideoException( "Cannot find video stream in the specified file." );
		}

		// find decoder for the video stream
		libffmpeg::AVCodec *codec = libffmpeg::avcodec_find_decoder( m_codecContext->codec_id );
		if ( codec == NULL )
		{
			throw gcnew VideoException( "Cannot find codec to decode the video stream." );
		}

		// open the codec
		if ( libffmpeg::avcodec_open( m_codecContext, codec ) < 0 )
		{
			throw gcnew VideoException( "Cannot open video codec." );
		}

		// allocate video frame
		m_videoFrame = libffmpeg::avcodec_alloc_frame( );

		// prepare scaling context to convert RGB image to video format
		m_convertContext = libffmpeg::sws_getContext( m_codecContext->width, m_codecContext->height, m_codecContext->pix_fmt,
				m_codecContext->width, m_codecContext->height, libffmpeg::PIX_FMT_BGR24,
				SWS_BICUBIC, NULL, NULL, NULL );

		if ( m_convertContext == NULL )
		{
			throw gcnew VideoException( "Cannot initialize frames conversion context." );
		}

		Console::WriteLine( String::Format( "duration: {0}", m_stream->duration ) );
		Console::WriteLine( String::Format( "fps: {0}", m_stream->time_base.den / m_stream->time_base.num  ) );

		Console::WriteLine( String::Format( "frames: {0}", m_stream->nb_frames ) );
		
		// libffmpeg::av_seek_frame( m_formatContext, m_videoStreamIndex, 100, 0 );
		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal( ptr );

		if ( !success )
		{
			Close( );
		}
	}
}

void VideoFileReader::Close(  )
{
	if ( m_videoFrame != NULL )
	{
		libffmpeg::av_free( m_videoFrame );
		m_videoFrame = NULL;
	}

	if ( m_codecContext != NULL )
	{
		libffmpeg::avcodec_close( m_codecContext );
		m_codecContext = NULL;
	}

	if ( m_formatContext != NULL )
	{
		libffmpeg::av_close_input_file( m_formatContext );
		m_formatContext = NULL;
	}

	if ( m_convertContext != NULL )
	{
		libffmpeg::sws_freeContext( m_convertContext );
		m_convertContext = NULL;
	}

	m_stream = NULL;
}

Bitmap^ VideoFileReader::ReadVideoFrame(  )
{
	libffmpeg::AVPacket packet;
	int frameFinished;
	Bitmap^ bitmap = nullptr;

	while ( libffmpeg::av_read_frame( m_formatContext, &packet ) >= 0 )
	{
		if ( packet.stream_index == m_stream->index )
		{
			libffmpeg::avcodec_decode_video( m_codecContext, m_videoFrame, &frameFinished, packet.data, packet.size );

			if ( frameFinished != 0 )
			{
				bitmap = gcnew Bitmap( m_codecContext->width, m_codecContext->height, PixelFormat::Format24bppRgb );
				
				// lock the bitmap
				BitmapData^ bitmapData = bitmap->LockBits( System::Drawing::Rectangle( 0, 0, m_codecContext->width, m_codecContext->height ),
					ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb );

				libffmpeg::uint8_t* ptr = reinterpret_cast<libffmpeg::uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

				libffmpeg::uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
				int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

				// convert video frame to the RGB bitmap
				libffmpeg::sws_scale( m_convertContext, m_videoFrame->data, m_videoFrame->linesize, 0,
					m_codecContext->height, srcData, srcLinesize );

				bitmap->UnlockBits( bitmapData );
				break;
			}
		}
	}

	libffmpeg::av_free_packet( &packet );

	return bitmap;
}

} } }