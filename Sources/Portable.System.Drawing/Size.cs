namespace System.Drawing
{
    public struct Size
    {
        private int _width;
        private int _height;

        public Size(int width, int height)
        {
            _width = width;
            _height = height;
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
    }
}