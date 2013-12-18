//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System
{
    public sealed class NotFiniteNumberException : Exception
    {
        #region CONSTRUCTORS

        public NotFiniteNumberException(string message, double offendingNumber)
            : base(message)
        {
        }

        #endregion
    }
}