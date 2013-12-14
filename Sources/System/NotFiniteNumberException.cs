//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System
{
    public class NotFiniteNumberException : Exception
    {
        #region CONSTRUCTORS

        public NotFiniteNumberException(string message, object value) : base(message)
        {
        }

        #endregion
    }
}