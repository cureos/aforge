// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Drawing
{
	public sealed class Pen : IDisposable
	{
		#region FIELDS

		private readonly Color _color;

		#endregion
		
		#region CONSTRUCTORS

		public Pen(Color color)
		{
			_color = color;
		}

		#endregion

		#region PROPERTIES

		public Color Color
		{
			get { return _color; }
		}
		
		#endregion		

		#region METHODS

		public void Dispose()
		{
		}
		
		#endregion
	}
}