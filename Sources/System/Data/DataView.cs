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
    public sealed class DataView
    {
        #region FIELDS

        private readonly DataTable _table;

        #endregion

        #region CONSTRUCTORS

        internal DataView(DataTable table)
        {
            _table = table;
        }

        #endregion

        #region METHODS

        public DataTable ToTable(bool distinct, params string[] columnNames)
        {
            var table = new DataTable { Locale = _table.Locale };
            table.Columns.AddRange(
                columnNames.Select(
                    name =>
                        new DataColumn(table, name,
                            _table.Columns.Contains(name) ? _table.Columns[name].DataType : typeof (object))));

            var viewRows = new List<DataRow>();
            foreach (var row in _table.Rows)
            {
                var viewRow = new DataRow(table);
                foreach (var kv in row)
                {
                    if (columnNames.Contains(kv.Key.ColumnName))
                        viewRow[kv.Key.ColumnName] = kv.Value;
                }
                viewRows.Add(viewRow);
            }
            table.Rows.AddRange(distinct ? viewRows.Distinct(DataRowComparer.Default) : viewRows);

            return table;
        }

        #endregion

        #region INNER CLASSES

        private class DataRowComparer : IEqualityComparer<DataRow>
        {
            #region FIELDS

            internal static readonly IEqualityComparer<DataRow> Default;
 
            #endregion

            #region CONSTRUCTORS

            static DataRowComparer()
            {
                Default = new DataRowComparer();
            }

            private DataRowComparer()
            {
            }

            #endregion

            #region METHODS

            public bool Equals(DataRow x, DataRow y)
            {
                var join = x.Join(y, kvx => kvx.Key.ColumnName, kvy => kvy.Key.ColumnName,
                    (kvx, kvy) => Tuple.Create(kvx.Value, kvy.Value)).ToArray();
                return join.Length == x.Count && join.All(tuple => tuple.Item1.Equals(tuple.Item2));
            }

            public int GetHashCode(DataRow obj)
            {
                return 1;
            }
            
            #endregion
        }

        #endregion
    }
}