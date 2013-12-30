// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.IO;
using MonoTouch.CoreGraphics;

namespace System.Drawing
{
	// TODO Implement methods and operators
	public sealed partial class Bitmap
	{
		#region METHODS

		public static Bitmap FromStream(Stream stream)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region OPERATORS

		public static implicit operator CGImage(Bitmap bitmap)
		{
			throw new NotImplementedException();
		}

		public static implicit operator Bitmap(CGImage cgImage)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
