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
using System.Reflection;

namespace System.Data
{
	public sealed class DataRow
	{
		#region FIELDS

		private readonly Dictionary<DataColumn, object> _objects;

		#endregion
		
		#region CONSTRUCTORS

		internal DataRow(DataTable table, IEnumerable cells)
		{
			Table = table;
			_objects = table.Columns.Cast<DataColumn>()
				.Zip(cells.Cast<object>(), Tuple.Create)
				.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
		}

		internal DataRow(DataTable table)
		{
			Table = table;
			_objects = new Dictionary<DataColumn, object>();
		}

		#endregion

		#region INDEXERS

		public object this[int index]
		{
			get { return _objects.ElementAt(index); }
		}

		public object this[DataColumn column]
		{
			get
			{
				return _objects.ContainsKey(column) ? _objects[column] : DBNull.Value;
			}
			set
			{
				if (!column.DataType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
					throw new InvalidCastException(String.Format("Value {0} of type {1} is not assignable to column data type {2}.",
						value, value.GetType().Name, column.DataType.Name));
				_objects[column] = value;
			}
		}

		public object this[string columnName]
		{
			get
			{
				var column = GetDataColumn(columnName);
				return this[column];
			}
			set
			{
				var column = GetDataColumn(columnName);
				this[column] = value;
			}
		}

		#endregion

		#region PROPERTIES

		public DataTable Table { get; private set; }

		public object[] ItemArray
		{
			get { return _objects.Values.ToArray(); }
		}

		#endregion

		#region METHODS

		private DataColumn GetDataColumn(string columnName)
		{
			try
			{
				return Table.Columns.Cast<DataColumn>().Single(col => col.ColumnName.Equals(columnName));
			}
			catch (InvalidOperationException e)
			{
				throw new ArgumentException("Specified column name does not exist in data table", e);
			}
		}

		#endregion
	}
}