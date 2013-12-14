//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.ComponentModel
{
    [AttributeUsageAttribute(AttributeTargets.All)]
    public sealed class BrowsableAttribute : Attribute
    {
        #region CONSTRUCTORS

        public BrowsableAttribute(bool browsable)
        {
        }

        #endregion
    }
}