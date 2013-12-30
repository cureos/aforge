// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using MonoTouch.CoreGraphics;

namespace System.Drawing
{
	public partial struct Color
	{
		#region OPERATORS

		public static implicit operator CGColor(Color color)
		{
			return new CGColor(color._r / 255.0f, color._g / 255.0f, color._b / 255.0f, color._a / 255.0f);
		}

		public static implicit operator Color(CGColor cgColor)
		{
			// TODO Check validity of RGB selection
			return FromArgb((int)(255.0f * cgColor.Alpha), (int)(255.0f * cgColor.Components[0]),
				(int)(255.0f * cgColor.Components[1]), (int)(255.0f * cgColor.Components[2]));
		}

		#endregion
	}
}