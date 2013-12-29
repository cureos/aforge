//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

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