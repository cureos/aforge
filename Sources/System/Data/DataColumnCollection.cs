//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
    public sealed class DataColumnCollection : List<DataColumn>
    {
        #region FIELDS

        private readonly DataTable _table;

        #endregion

        #region CONSTRUCTORS

        internal DataColumnCollection(DataTable table)
        {
            _table = table;
        }

        #endregion

        #region INDEXERS

        public DataColumn this[string columnName]
        {
            get { return this.Single(col => col.ColumnName.Equals(columnName)); }
        }

        #endregion

        #region METHODS

        public DataColumn Add(string columnName, Type type)
        {
            var column = new DataColumn(_table, columnName, type);
            Add(column);
            return column;
        }

        public bool Contains(string columnName)
        {
            return this.Any(col => col.ColumnName.Equals(columnName));
        }

        #endregion
    }
}