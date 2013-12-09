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

        #endregion

        #region METHODS

        #endregion

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }
    }
}