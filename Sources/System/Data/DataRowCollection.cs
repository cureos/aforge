/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

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