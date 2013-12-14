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
    public struct Size
    {
        #region FIELDS

        private readonly int _width;
        private readonly int _height;
        
        #endregion

        #region CONSTRUCTORS

        public Size(int width, int height)
        {
            _width = width;
            _height = height;
        }
        
        #endregion

        #region PROPERTIES

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }
        
        #endregion
    }
}