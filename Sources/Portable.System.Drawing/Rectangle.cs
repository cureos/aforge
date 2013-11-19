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
    public struct Rectangle
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public Rectangle(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Left
        {
            get { return _x; }
        }

        public int Top
        {
            get { return _y; }
        }

        public int Right
        {
            get { return _x + _width; }
        }

        public int Bottom
        {
            get { return _y + _height; }
        }

        public void Intersect(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        public bool Contains(int x, int y)
        {
            throw new NotImplementedException();
        }

        public static Rectangle Intersect(Rectangle rectangle, Rectangle rectangle1)
        {
            throw new NotImplementedException();
        }

        public void Inflate(int i, int i1)
        {
            throw new NotImplementedException();
        }
    }
}