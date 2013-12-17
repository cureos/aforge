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
    public sealed class DataRow : Dictionary<DataColumn, object>
    {
        #region CONSTRUCTORS

        internal DataRow(DataTable table, IEnumerable<object> cells)
            : base(table.Columns.Zip(cells, Tuple.Create).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2))
        {
            Table = table;
        }

        internal DataRow(DataTable table)
            : base(table.Columns.ToDictionary(col => col, col => (object)null))
        {
            Table = table;
        }

        #endregion

        #region INDEXERS

        public object this[int index]
        {
            get { return Values.ElementAt(index); }
        }

        public object this[string columnName]
        {
            get { return this[GetDataColumn(columnName)]; }
            set { this[GetDataColumn(columnName)] = value; }
        }

        #endregion

        #region PROPERTIES

        public DataTable Table { get; private set; }

        #endregion

        #region METHODS

        private DataColumn GetDataColumn(string columnName)
        {
            return this.Single(kv => kv.Key.ColumnName.Equals(columnName)).Key;
        }

        #endregion
    }
}