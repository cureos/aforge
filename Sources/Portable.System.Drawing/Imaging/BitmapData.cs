namespace System.Drawing.Imaging
{
    public class BitmapData
    {
        private int _stride;
        private IntPtr _scan0;
        private int _width;
        private int _height;
        private PixelFormat _pixelFormat;

        public int Stride
        {
            get { return _stride; }
            set { _stride = value; }
        }

        public IntPtr Scan0
        {
            get { return _scan0; }
            set { _scan0 = value; }
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

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set { _pixelFormat = value; }
        }
    }
}