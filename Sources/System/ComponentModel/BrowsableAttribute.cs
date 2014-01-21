//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
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