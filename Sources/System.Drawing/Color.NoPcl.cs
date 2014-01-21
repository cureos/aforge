// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

#if NETFX_CORE
using TrueColor = Windows.UI.Color;
#else
using TrueColor = System.Windows.Media.Color;
#endif

namespace System.Drawing
{
	public partial struct Color
	{

		public static implicit operator TrueColor(Color color)
		{
			return TrueColor.FromArgb(color._a, color._r, color._g, color._b);
		}

		public static implicit operator Color(TrueColor trueColor)
		{
			return FromArgb(trueColor.A, trueColor.R, trueColor.G, trueColor.B);
		}
	}
}