//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System.Collections;
using System.Collections.Generic;

namespace System.Data
{
	public sealed class DataRowCollection : ICollection
	{
		#region FIELDS

		private readonly DataTable _table;
		private readonly List<DataRow> _rows;
		private readonly object _syncRoot;

		#endregion

		#region CONSTRUCTORS

		internal DataRowCollection(DataTable table)
		{
			_table = table;
			_rows = new List<DataRow>();
			_syncRoot = new object();
		}

		#endregion

		#region INDEXERS

		public DataRow this[int index]
		{
			get { return _rows[index]; }
		}

		#endregion
		
		#region PROPERTIES

		public int Count
		{
			get { return _rows.Count; }
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

		public void Add(DataRow row)
		{
			_rows.Add(row);
		}

		public DataRow Add(object[] values)
		{
			var row = new DataRow(_table, values);
			_rows.Add(row);
			return row;
		}

		public void Remove(DataRow row)
		{
			_rows.Remove(row);
		}

		public int IndexOf(DataRow row)
		{
			return _rows.IndexOf(row);
		}

		public IEnumerator GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}