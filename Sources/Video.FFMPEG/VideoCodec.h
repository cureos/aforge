// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#pragma once

using namespace System;

extern int video_codecs[];
extern int CODECS_COUNT;

namespace AForge { namespace Video { namespace FFMPEG
{
	/// <summary>
	/// Enumeration of some video codecs from FFmpeg library, which are available for writing video files.
	/// </summary>
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
		Theora,
		Raw,
	};

} } }