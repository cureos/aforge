// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Portable Adaptations
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.Drawing
{
    public struct Point
    {
        #region FIELDS

        private readonly int _x;
        private readonly int _y;

        #endregion

        #region CONSTRUCTORS

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        #endregion

        #region PROPERTIES

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        #endregion

		#region METHODS

	    public static Point Round(PointF value)
	    {
		    int x, y;
		    checked
		    {
			    x = (int)Math.Round(value.X);
			    y = (int)Math.Round(value.Y);
		    }

		    return new Point(x, y);
	    }

	    #endregion
    }
}