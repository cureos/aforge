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