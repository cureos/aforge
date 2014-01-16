//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System
{
    public sealed class ApplicationException : Exception
    {
        #region CONSTRUCTORS

        public ApplicationException(string message) : base(message)
        {
        }

        #endregion
    }
}