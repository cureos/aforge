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