//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System
{
    public sealed class DBNull
    {
        #region FIELDS

        public static readonly DBNull Value;

        #endregion

        #region CONSTRUCTORS

        static DBNull()
        {
            Value = new DBNull();
        }

        private DBNull()
        {
        }

        #endregion
    }
}