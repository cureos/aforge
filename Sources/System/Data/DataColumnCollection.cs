//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Data
{
    public class DataColumnCollection : List<DataColumn>
    {
        #region INDEXERS

        public DataColumn this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region METHODS

        public void Add(string columnName, Type type)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}