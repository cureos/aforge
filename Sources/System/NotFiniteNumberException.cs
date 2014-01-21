//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
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