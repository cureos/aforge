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
	public struct PointF
	{
		#region FIELDS

		private float _x;
		private float _y;

		#endregion

		#region CONSTRUCTORS

		public PointF(float x, float y)
		{
			_x = x;
			_y = y;
		}

		#endregion

		#region PROPERTIES

		public float X
		{
			get { return _x; }
			set { _x = value; }
		}

		public float Y
		{
			get { return _y; }
			set { _y = value; }
		}

		#endregion
	}
}