//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
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