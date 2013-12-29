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
			foreach (var name in columnNames)
				table.Columns.Add(name, _table.Columns.Contains(name) ? _table.Columns[name].DataType : typeof (object));

			var viewRows = new List<DataRow>();
			foreach (DataRow row in _table.Rows)
			{
				var viewRow = new DataRow(table);
				foreach (var columnName in columnNames)
				{
					if (table.Columns.Contains(columnName))
						viewRow[columnName] = row[columnName];
				}
				viewRows.Add(viewRow);
			}
			foreach (var row in (distinct ? viewRows.Distinct(new DataRowComparer(_table)) : viewRows))
				table.Rows.Add(row);

			return table;
		}

		#endregion

		#region INNER CLASSES

		private class DataRowComparer : IEqualityComparer<DataRow>
		{
			#region FIELDS

			private readonly DataTable _table;
 
			#endregion

			#region CONSTRUCTORS

			internal DataRowComparer(DataTable table)
			{
				_table = table;
			}

			#endregion

			#region METHODS

			public bool Equals(DataRow x, DataRow y)
			{
				foreach (DataColumn column in _table.Columns)
				{
					var columnName = column.ColumnName;
					if (!x[columnName].Equals(y[columnName])) return false;
				}
				return true;
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