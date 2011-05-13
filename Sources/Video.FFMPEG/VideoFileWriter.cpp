// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#include "StdAfx.h"
#include "VideoFileWriter.h"

namespace AForge { namespace Video { namespace FFMPEG
{

VideoFileWriter::VideoFileWriter( void )
{
	m_formatContext     = NULL;
	m_videoStream       = NULL;
	m_videoFrame        = NULL;
	m_convertContext	= NULL;
	m_videoOutputBuffer = NULL;

	m_width     = 0;
	m_height    = 0;
	m_frameRate = 25;
	m_codec     = VideoCodec::Default;

	libffmpeg::av_register_all( );
}

void VideoFileWriter::Open( String^ fileName, int width, int height )
{
	Open( fileName, width, height, m_frameRate );
}

void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate )
{
	Open( fileName, width, height, frameRate, m_codec );
}

void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec )
{
	// close previous file if any open
	Close( );

	bool success = false;

	// check width and height
	if ( ( ( width & 1 ) != 0 ) || ( ( height & 1 ) != 0 ) )
	{
		throw gcnew ArgumentException( "Video file resolution must be a multiple of two." );
	}

	// check video codec
	if ( ( (int) codec < -1 ) || ( (int) codec >= CODECS_COUNT ) )
	{
		throw gcnew ArgumentException( "Invalid video codec is specified." );
	}

	m_width  = width;
	m_height = height;
	m_codec  = codec;
	m_frameRate = frameRate;
	
	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi( fileName );
	char* nativeFileName = reinterpret_cast<char*>( static_cast<void*>( ptr ) );

	try
	{
		// gues about destination file format from its file name
		libffmpeg::AVOutputFormat* outputFormat = libffmpeg::av_guess_format( NULL, nativeFileName, NULL );

		if ( !outputFormat )
		{
			// gues about destination file format from its short name
			outputFormat = libffmpeg::av_guess_format( "mpeg", NULL, NULL );

			if ( !outputFormat )
			{
				throw gcnew VideoException( "Cannot find suitable output format." );
			}
		}

		Console::WriteLine( "got format" );

		// prepare format context
		m_formatContext = libffmpeg::avformat_alloc_context( );

		if ( !m_formatContext )
		{
			throw gcnew VideoException( "Cannot allocate format context." );
		}
		m_formatContext->oformat = outputFormat;

		Console::WriteLine( "format context allocated" );

		// add video stream using the specified video codec
		m_videoStream = add_video_stream( m_formatContext,
			( m_codec == VideoCodec::Default ) ? outputFormat->video_codec : video_codecs[(int) m_codec] );

		// set the output parameters (must be done even if no parameters)
		if ( libffmpeg::av_set_parameters( m_formatContext, NULL ) < 0 )
		{
			throw gcnew VideoException( "Failed configuring format context." );
		}

		Console::WriteLine( "video stream initialized" );

		open_video( m_formatContext, m_videoStream );

		// open output file
		if ( !( outputFormat->flags & AVFMT_NOFILE  ))
		{
			if ( libffmpeg::avio_open( &m_formatContext->pb, nativeFileName, AVIO_WRONLY ) < 0 )
			{
				throw gcnew System::IO::IOException( "Cannot open the video file." );
			}
		}

		libffmpeg::av_write_header( m_formatContext );

		Console::WriteLine( "header written" );
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

// Close video file
void VideoFileWriter::Close( )
{
	if ( m_formatContext )
	{
		if ( m_formatContext->pb != NULL )
		{
			libffmpeg::av_write_trailer( m_formatContext );
			Console::WriteLine( "trailler written" );
		}

		if ( m_videoStream )
		{
			libffmpeg::avcodec_close( m_videoStream->codec );
			m_videoStream = NULL;
		}

		if ( m_videoFrame )
		{
			libffmpeg::av_free( m_videoFrame->data[0] );
			libffmpeg::av_free( m_videoFrame );
			m_videoFrame = NULL;
		}

		if ( m_videoOutputBuffer )
		{
			libffmpeg::av_free(m_videoOutputBuffer);
			m_videoOutputBuffer = NULL;
		}

		for ( unsigned int i = 0; i < m_formatContext->nb_streams; i++ )
		{
			libffmpeg::av_freep( &m_formatContext->streams[i]->codec );
			libffmpeg::av_freep( &m_formatContext->streams[i] );
		}

		if ( m_formatContext->pb != NULL )
		{
			libffmpeg::avio_close( m_formatContext->pb );
		}
		
		libffmpeg::av_free( m_formatContext );
		m_formatContext = NULL;
	}

	if ( m_convertContext )
	{
		libffmpeg::sws_freeContext( m_convertContext );
		m_convertContext = NULL;
	}

	m_width  = 0;
	m_height = 0;
}

// Writes new video frame to the opened video file
void VideoFileWriter::WriteVideoFrame( Bitmap^ frame )
{
	if ( !m_formatContext )
	{
		throw gcnew VideoException( "A video file was not opened yet." );
	}

	if ( ( frame->PixelFormat != System::Drawing::Imaging::PixelFormat::Format24bppRgb ) &&
	     ( frame->PixelFormat != System::Drawing::Imaging::PixelFormat::Format32bppArgb ) &&
		 ( frame->PixelFormat != System::Drawing::Imaging::PixelFormat::Format32bppPArgb ) &&
	 	 ( frame->PixelFormat != System::Drawing::Imaging::PixelFormat::Format32bppRgb ) )
	{
		throw gcnew ArgumentException( "The provided bitmap must be 24 or 32 bpp color image." );
	}

	if ( ( frame->Width != m_width ) || ( frame->Height != m_height ) )
	{
		throw gcnew ArgumentException( "Bitmap size must of the same as video size, which was specified on opening video file." );
	}

	// lock the bitmap
	BitmapData^ bitmapData = frame->LockBits( System::Drawing::Rectangle( 0, 0, m_width, m_height ),
		ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb );

	libffmpeg::uint8_t* ptr = reinterpret_cast<libffmpeg::uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

	libffmpeg::uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// convert source image to the format of the video file
	libffmpeg::sws_scale( m_convertContext, srcData, srcLinesize, 0, m_height, m_videoFrame->data, m_videoFrame->linesize );

	frame->UnlockBits( bitmapData );

	// write the converted frame to the video file
	write_video_frame( );
}

#pragma region Private methods
// Writes video frame to opened video file
void VideoFileWriter::write_video_frame( )
{
	libffmpeg::AVCodecContext* codecContext = m_videoStream->codec;
	int out_size, ret = 0;

	if ( m_formatContext->oformat->flags & AVFMT_RAWPICTURE )
	{
		Console::WriteLine( "raw picture must be written" );
	}
	else
	{
		// encode the image
		out_size = libffmpeg::avcodec_encode_video( codecContext, m_videoOutputBuffer,
			m_videoOutputBufferSize, m_videoFrame );

		// if zero size, it means the image was buffered
		if ( out_size > 0 )
		{
			libffmpeg::AVPacket packet;
			libffmpeg::av_init_packet( &packet );

			if ( codecContext->coded_frame->pts != AV_NOPTS_VALUE )
			{
				packet.pts = libffmpeg::av_rescale_q( codecContext->coded_frame->pts, codecContext->time_base, m_videoStream->time_base );
			}

			if ( codecContext->coded_frame->key_frame )
			{
				packet.flags |= AV_PKT_FLAG_KEY;
			}

			packet.stream_index = m_videoStream->index;
			packet.data = m_videoOutputBuffer;
			packet.size = out_size;

			// write the compressed frame to the media file
			ret = libffmpeg::av_interleaved_write_frame( m_formatContext, &packet );
		}
		else
		{
			Console::WriteLine( "image was buffered" );
		}
	}

	if ( ret != 0 )
	{
		throw gcnew VideoException( "Error while writing video frame." );
	}
}

// Allocate picture of the specified format and size
static libffmpeg::AVFrame* alloc_picture( enum libffmpeg::PixelFormat pix_fmt, int width, int height )
{
	libffmpeg::AVFrame* picture;
	void* picture_buf;
	int size;

	picture = libffmpeg::avcodec_alloc_frame( );
	if ( !picture )
	{
		return NULL;
	}

	size = libffmpeg::avpicture_get_size( pix_fmt, width, height );
	picture_buf = libffmpeg::av_malloc( size );
	if ( !picture_buf )
	{
		libffmpeg::av_free( picture );
		return NULL;
	}

	libffmpeg::avpicture_fill( (libffmpeg::AVPicture *) picture, (libffmpeg::uint8_t *) picture_buf, pix_fmt, width, height );

	return picture;
}

// Create new video stream and configure it
libffmpeg::AVStream* VideoFileWriter::add_video_stream( libffmpeg::AVFormatContext* formatContext, enum libffmpeg::CodecID codec_id )
{
	libffmpeg::AVCodecContext* codecContex;
	libffmpeg::AVStream* stream;

	// create new stream
	stream = libffmpeg::av_new_stream( formatContext, 0 );
	if ( !stream )
	{
		throw gcnew VideoException( "Failed creating new video stream." );
	}

	codecContex = stream->codec;
	codecContex->codec_id   = codec_id;
	codecContex->codec_type = libffmpeg::AVMEDIA_TYPE_VIDEO;

	// put sample parameters
	codecContex->bit_rate = 400000;
	codecContex->width    = m_width;
	codecContex->height   = m_height;

	// time base: this is the fundamental unit of time (in seconds) in terms
	// of which frame timestamps are represented. for fixed-fps content,
	// timebase should be 1/framerate and timestamp increments should be
	// identically 1.
	codecContex->time_base.den = m_frameRate;
	codecContex->time_base.num = 1;

	codecContex->gop_size = 12; // emit one intra frame every twelve frames at most
	codecContex->pix_fmt  = libffmpeg::PIX_FMT_YUV420P;

	if ( codecContex->codec_id == libffmpeg::CODEC_ID_MPEG1VIDEO )
	{
		// Needed to avoid using macroblocks in which some coeffs overflow.
		// This does not happen with normal video, it just happens here as
		// the motion of the chroma plane does not match the luma plane.
		codecContex->mb_decision = 2;
	}

	// some formats want stream headers to be separate
	if( formatContext->oformat->flags & AVFMT_GLOBALHEADER )
	{
		codecContex->flags |= CODEC_FLAG_GLOBAL_HEADER;
	}

	return stream;
}

// Open video codec and prepare out buffer and picture
void VideoFileWriter::open_video( libffmpeg::AVFormatContext* formatContext, libffmpeg::AVStream* stream )
{
	libffmpeg::AVCodecContext* codecContext = stream->codec;
	libffmpeg::AVCodec* codec = avcodec_find_encoder( codecContext->codec_id );

	if ( !codec )
	{
		throw gcnew VideoException( "Cannot find video codec." );
	}

	// open the codec 
	if ( avcodec_open( codecContext, codec ) < 0 )
	{
		throw gcnew VideoException( "Cannot open video codec." );
	}

	m_videoOutputBuffer = NULL;
	if ( !( formatContext->oformat->flags & AVFMT_RAWPICTURE ) )
	{
         // allocate output buffer 
         m_videoOutputBufferSize = 6 * m_width * m_height; // more than enough even for raw video
		 m_videoOutputBuffer = (libffmpeg::uint8_t*) libffmpeg::av_malloc( m_videoOutputBufferSize );
	}

	// allocate the encoded raw picture
	m_videoFrame = alloc_picture( codecContext->pix_fmt, codecContext->width, codecContext->height );

	if ( !m_videoFrame )
	{
		throw gcnew VideoException( "Cannot allocate video picture." );
	}

	// prepare scaling context to convert RGB image to video format
	m_convertContext = libffmpeg::sws_getContext( codecContext->width, codecContext->height, libffmpeg::PIX_FMT_BGR24,
			codecContext->width, codecContext->height, codecContext->pix_fmt,
			SWS_BICUBIC, NULL, NULL, NULL );

	if ( m_convertContext == NULL )
	{
		throw gcnew VideoException( "Cannot initialize frames conversion context." );
	}
}
#pragma endregion
		
} } }

