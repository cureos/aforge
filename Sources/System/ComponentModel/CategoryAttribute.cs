//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.ComponentModel
{
    [AttributeUsageAttribute(AttributeTargets.All)]
    public sealed class CategoryAttribute : Attribute
    {
        #region CONSTRUCTORS

        public CategoryAttribute(string category)
        {
        }

        #endregion
    }
}