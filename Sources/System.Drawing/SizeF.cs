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
    public struct SizeF
    {
        #region FIELDS

        private readonly float _width;
        private readonly float _height;
        
        #endregion

        #region CONSTRUCTORS

        public SizeF(float width, float height)
        {
            _width = width;
            _height = height;
        }
        
        #endregion

        #region PROPERTIES

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }
        
        #endregion
    }
}