// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#pragma once

using namespace System;

namespace libffmpeg
{
	extern "C"
	{
		#include "libavcodec\avcodec.h"
	}
}

extern libffmpeg::CodecID video_codecs[];
extern int CODECS_COUNT;

namespace AForge { namespace Video { namespace FFMPEG
{
	public enum class VideoCodec
	{
		Default = -1,
		MPEG4 = 0,
		WMV1,
		WMV2,
		MSMPEG4v2,
		MSMPEG4v3,
		H263P,
		FLV1,
		THEORA
	};

} } }