//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
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
			foreach (DataColumn column in this.Columns) table.Columns.Add(column.ColumnName, column.DataType);
			return table;
		}

		public DataTable Copy()
		{
			var table = Clone();
			foreach (DataRow row in this.Rows) table.Rows.Add(row.ItemArray);
			return table;
		}

		public void ImportRow(DataRow row)
		{
			var newRow = new DataRow(this);
			foreach (DataColumn column in this.Columns)
			{
				var columnName = column.ColumnName;
				newRow[columnName] = row[columnName];
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