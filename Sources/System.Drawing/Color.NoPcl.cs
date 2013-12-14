// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
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