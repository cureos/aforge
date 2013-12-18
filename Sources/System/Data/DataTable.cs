//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Globalization;
using System.Linq;

namespace System.Data
{
    public sealed class DataTable
    {
        #region CONSTRUCTORS

        public DataTable()
        {
            Columns = new DataColumnCollection(this);
            Rows = new DataRowCollection(this);
            Locale = CultureInfo.CurrentCulture;
            DefaultView = new DataView(this);
        }

        #endregion

        #region PROPERTIES

        public DataColumnCollection Columns { get; private set; }

        public DataRowCollection Rows { get; private set; }

        public CultureInfo Locale { get; set; }

        public DataView DefaultView { get; private set; }

        #endregion

        #region METHODS

        public DataTable Clone()
        {
            var table = new DataTable { Locale = this.Locale };
            table.Columns.AddRange(this.Columns.Select(col => new DataColumn(table, col.ColumnName, col.DataType)));
            return table;
        }

        public DataTable Copy()
        {
            var table = Clone();
            table.Rows.AddRange(this.Rows.Select(row => new DataRow(table, row.Values)));
            return table;
        }

        public void ImportRow(DataRow row)
        {
            var newRow = new DataRow(this);
            foreach (var kv in row)
            {
                if (Columns.Contains(kv.Key.ColumnName))
                    newRow[kv.Key.ColumnName] = kv.Value;
            }
            Rows.Add(newRow);
        }

        public DataRow NewRow()
        {
            return new DataRow(this);
        }

        public DataRow[] Select(string filterExpression, string sort)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}