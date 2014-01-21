//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.ComponentModel
{
    [AttributeUsageAttribute(AttributeTargets.All)]
    public sealed class DescriptionAttribute : Attribute
    {
        #region CONSTRUCTORS

        public DescriptionAttribute(string description)
        {
        }

        #endregion
    }
}