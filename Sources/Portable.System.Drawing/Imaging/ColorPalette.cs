namespace System.Drawing.Imaging
{
    public class ColorPalette
    {
        private Color[] _entries;

        public Color[] Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }
    }
}