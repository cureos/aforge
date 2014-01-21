// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Drawing.Imaging
{
    public class ColorPalette
    {
        #region FIELDS

        private readonly Color[] _entries;
        
        #endregion

        #region CONSTRUCTORS
        
        internal ColorPalette(Color[] entries)
        {
            _entries = entries;
        }

        #endregion

        #region PROPERTIES

        public Color[] Entries
        {
            get { return _entries; }
        }
        
        #endregion
    }
}