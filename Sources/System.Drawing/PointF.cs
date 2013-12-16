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
    public struct PointF
    {
        #region FIELDS

        private readonly float _x;
        private readonly float _y;

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
        }

        public float Y
        {
            get { return _y; }
        }

        #endregion
    }
}