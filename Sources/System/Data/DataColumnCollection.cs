//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Data
{
	public sealed class DataColumnCollection : ICollection
	{
		#region FIELDS

		private readonly DataTable _table;
		private readonly List<DataColumn> _columns;
		private readonly object _syncRoot;

		#endregion

		#region CONSTRUCTORS

		internal DataColumnCollection(DataTable table)
		{
			_table = table;
			_columns = new List<DataColumn>();
			_syncRoot = new object();
		}

		#endregion

		#region INDEXERS

		public DataColumn this[string columnName]
		{
			get { return _columns.Single(col => col.ColumnName.Equals(columnName)); }
		}

		public DataColumn this[int index]
		{
			get { return _columns[index]; }
		}

		#endregion

		#region PROPERTIES

		public int Count
		{
			get { return _columns.Count; }
		}

		public bool IsSynchronized
		{
			get { return true; }
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion
		
		#region METHODS

		public DataColumn Add(string columnName, Type type)
		{
			var column = new DataColumn(_table, columnName, type);
			_columns.Add(column);
			return column;
		}

		public bool Contains(string columnName)
		{
			return _columns.Any(col => col.ColumnName.Equals(columnName));
		}

		public IEnumerator GetEnumerator()
		{
			return _columns.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}