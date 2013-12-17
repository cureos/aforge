//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Data
{
    public sealed class DataRowCollection : List<DataRow>
    {
        #region FIELDS

        private readonly DataTable _table;

        #endregion

        #region CONSTRUCTORS

        internal DataRowCollection(DataTable table)
        {
            _table = table;
        }

        #endregion

        #region METHODS

        public void Add(IEnumerable<object> cells)
        {
            Add(new DataRow(_table, cells));
        }
        
        #endregion
    }
}