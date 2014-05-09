//
// System.Data.DataColumnCollection.cs
//
// Author:
//   Christopher Podurgiel (cpodurgiel@msn.com)
//   Stuart Caborn	<stuart.caborn@virgin.net>
//   Tim Coleman (tim@timcoleman.com)
//
// (C) Chris Podurgiel
// Copyright (C) Tim Coleman, 2002
// Copyright (C) Daniel Morgan, 2003
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data{
	internal class Doublet
	{
		public Doublet (int count, string columnname)
		{
			this.count = count;
			this.columnNames.Add (columnname);
		}
		// Number of case insensitive column name
		public int count;
		// Array of exact column names
		public ArrayList columnNames = new ArrayList ();
	}

	[DefaultEvent ("CollectionChanged")]
	public class DataColumnCollection : InternalDataCollectionBase{
		#region event definitions
		/// <summary>
		/// Occurs when the columns collection changes, either by adding or removing a column.
		/// </summary>
		public event CollectionChangeEventHandler CollectionChanged;

		internal event CollectionChangeEventHandler CollectionMetaDataChanged;
		#endregion

		#region private instance fields
		//This hashtable maps between unique case insensetive column name to a doublet containing column ref and column count
		private Hashtable columnNameCount = new Hashtable (StringComparer.OrdinalIgnoreCase);

		//This hashtable maps between column name to DataColumn object.
		private Hashtable columnFromName = new Hashtable ();
		
		//This ArrayList contains the auto-increment columns names
		private ArrayList autoIncrement = new ArrayList ();
		
		//This holds the next index to use for default column name.
		private int defaultColumnIndex = 1;
		
		//table should be the DataTable this DataColumnCollection belongs to.
		private DataTable parentTable = null;
		
		// Keep reference to most recent columns passed to AddRange()
		// so that they can be added when EndInit() is called.
		DataColumn [] _mostRecentColumns = null;

		static readonly string ColumnPrefix = "Column";

		// Internal Constructor.  This Class can only be created from other classes in this assembly.
		internal DataColumnCollection (DataTable table)
		{
			parentTable = table;
		}
		
		/// <summary>
		/// Gets the DataColumn from the collection at the specified index.
		/// </summary>
		public DataColumn this [int index] {
			get {
				if (index < 0 || index >= base.List.Count)
					throw new IndexOutOfRangeException ("Cannot find column " + index + ".");
				return (DataColumn) base.List [index];
			}
		}

		/// <summary>
		/// Gets the DataColumn from the collection with the specified name.
		/// </summary>
		public DataColumn this [string name] {
			get {
				if (name == null)
					throw new ArgumentNullException ("name");
		
				DataColumn dc = columnFromName [name] as DataColumn;
				if (dc != null)
					return dc;

				int tmp = IndexOf (name, true);
					return tmp == -1 ? null : (DataColumn) base.List [tmp];
			}
		}

		/// <summary>
		/// Gets a list of the DataColumnCollection items.
		/// </summary>
		protected override ArrayList List {
				get { return base.List; }
		}

		internal ArrayList AutoIncrmentColumns {
				get { return autoIncrement; }
		}

		#endregion

		#region public instance methods
		/// <summary>
		/// Creates and adds a DataColumn object to the DataColumnCollection.
		/// </summary>
		/// <returns></returns>
		public DataColumn Add ()
		{
			DataColumn column = new DataColumn (null);
			Add (column);
			return column;
		}

		/// <summary>
		/// Creates and adds a DataColumn object with the specified name to the DataColumnCollection.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <returns>The newly created DataColumn.</returns>
		public DataColumn Add (string columnName)
		{
			DataColumn column = new DataColumn (columnName);
			Add (column);
			return column;
		}

		/// <summary>
		/// Creates and adds a DataColumn object with the specified name and type to the DataColumnCollection.
		/// </summary>
		/// <param name="columnName">The ColumnName to use when cretaing the column.</param>
		/// <param name="type">The DataType of the new column.</param>
		/// <returns>The newly created DataColumn.</returns>
		public DataColumn Add (string columnName, Type type)
		{
			if (columnName == null || columnName == "")
				columnName = GetNextDefaultColumnName ();

			DataColumn column = new DataColumn (columnName, type);
			Add (column);
			return column;
		}

		/// <summary>
		/// Creates and adds a DataColumn object with the specified name, type, and expression to the DataColumnCollection.
		/// </summary>
		/// <param name="columnName">The name to use when creating the column.</param>
		/// <param name="type">The DataType of the new column.</param>
		/// <param name="expression">The expression to assign to the Expression property.</param>
		/// <returns>The newly created DataColumn.</returns>
		public DataColumn Add (string columnName, Type type, string expression)
		{
			if (columnName == null || columnName == "")
				columnName = GetNextDefaultColumnName ();

			DataColumn column = new DataColumn (columnName, type, expression);
			Add (column);
			return column;
		}

		/// <summary>
		/// Copies the elements of the specified DataColumn array to the end of the collection.
		/// </summary>
		/// <param name="columns">The array of DataColumn objects to add to the collection.</param>
		public void AddRange (DataColumn [] columns)
		{
			if (parentTable.InitInProgress){
				_mostRecentColumns = columns;
				return;
			}

			if (columns == null)
				return;

			foreach (DataColumn column in columns){
				if (column == null)
					continue;
				Add(column);
			}
		}

		/// <summary>
		/// Creates and adds the specified DataColumn object to the DataColumnCollection.
		/// </summary>
		/// <param name="column">The DataColumn to add.</param>
		public void Add (DataColumn column)
		{
			if (column == null)
				throw new ArgumentNullException ("column", "'column' argument cannot be null.");

			if (column.ColumnName.Length == 0) {
				column.ColumnName = GetNextDefaultColumnName ();
			}

			//			if (Contains(column.ColumnName))
			//				throw new DuplicateNameException("A DataColumn named '" + column.ColumnName + "' already belongs to this DataTable.");

			if (column.Table != null)
				throw new ArgumentException ("Column '" + column.ColumnName + "' already belongs to this or another DataTable.");

			column.SetTable (parentTable);
			RegisterName (column.ColumnName, column);
			int ordinal = base.List.Add (column);
			column.Ordinal = ordinal;

			// Check if the Column Expression is ok
			/*
			if (column.CompiledExpression != null)
			if (parentTable.Rows.Count == 0)
				column.CompiledExpression.Eval (parentTable.NewRow());
			else
				column.CompiledExpression.Eval (parentTable.Rows[0]);
			*/
			
			// if table already has rows we need to allocate space
			// in the column data container
			if (parentTable.Rows.Count > 0)
				column.DataContainer.Capacity = parentTable.RecordCache.CurrentCapacity;

			if (column.AutoIncrement) {
				DataRowCollection rows = column.Table.Rows;
				for (int i = 0; i < rows.Count; i++)
					rows [i] [ordinal] = column.AutoIncrementValue ();
			}

			if (column.AutoIncrement)
				autoIncrement.Add (column);

			column.PropertyChanged += new PropertyChangedEventHandler (ColumnPropertyChanged);
			OnCollectionChanged (new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
		}
		
		public void CopyTo (DataColumn [] array, int index)
		{
			CopyTo ((Array) array, index);
		}
		
		/// <summary>
		/// Checks whether the collection contains a column with the specified name.
		/// </summary>
		/// <param name="name">The ColumnName of the column to check for.</param>
		/// <returns>true if a column exists with this name; otherwise, false.</returns>
		public bool Contains (string name)
		{
			if (columnFromName.Contains (name))
				return true;

			return (IndexOf (name, false) != -1);
		}

		/// <summary>
		/// Gets the index of a column specified by name.
		/// </summary>
		/// <param name="column">The name of the column to return.</param>
		/// <returns>The index of the column specified by column if it is found; otherwise, -1.</returns>
		public int IndexOf (DataColumn column)
		{
			if (column == null)
				return -1;
			return base.List.IndexOf (column);
		}

		/// <summary>
		/// Gets the index of the column with the given name (the name is not case sensitive).
		/// </summary>
		/// <param name="columnName">The name of the column to find.</param>
		/// <returns>The zero-based index of the column with the specified name, or -1 if the column doesn't exist in the collection.</returns>
		public int IndexOf (string columnName)
		{
			if (columnName == null)
				return -1;
			DataColumn dc = columnFromName [columnName] as DataColumn;
			
			if (dc != null)
				return IndexOf (dc);
			
			return IndexOf (columnName, false);
		}
		#endregion

		#region private instance methods
		private void ColumnPropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			OnCollectionMetaDataChanged (new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		
		private string GetNextDefaultColumnName ()
		{
			string defColumnName = MakeName (defaultColumnIndex);
			for (int index = defaultColumnIndex + 1; Contains (defColumnName); ++index) {
				defColumnName = MakeName (index);
				defaultColumnIndex++;
			}
			defaultColumnIndex++;
			return defColumnName;
		}

		private void OnCollectionMetaDataChanged (CollectionChangeEventArgs ccevent)
		{
			parentTable.ResetPropertyDescriptorsCache ();
			if (CollectionMetaDataChanged != null)
				CollectionMetaDataChanged (this, ccevent);
		}

		private int IndexOf (string name, bool error)
		{
			// exact case matching has already be done by the caller
			// Get existing doublet
			Doublet d = (Doublet) columnNameCount [name];
			if (d != null) {
				if (d.count == 1) {
					// There's only one
					// return index of the column from the only column name of the doublet
					return base.List.IndexOf (columnFromName [d.columnNames [0]]);
				} else if (d.count > 1 && error) {
					// there's more than one, exception!
					throw new ArgumentException ("There is no match for '" + name + "' in the same case and there are multiple matches in different case.");
				} else {
					return -1;
				}
			}
			return -1;
		}
		#endregion

		#region internal instance methods
		internal void MoveColumn (int oldOrdinal, int newOrdinal)
		{
			if (newOrdinal == -1 || newOrdinal > this.Count)
				throw new ArgumentOutOfRangeException ("ordinal", "Ordinal '" + newOrdinal + "' exceeds the maximum number.");
			if (oldOrdinal == newOrdinal)
				return;

			int start = newOrdinal > oldOrdinal ? oldOrdinal : newOrdinal;
			int end = newOrdinal > oldOrdinal ?  newOrdinal : oldOrdinal;
			int direction = newOrdinal > oldOrdinal ? 1 : (-1);

			// swap ordinals as per direction of column movement
			if (direction < 0) {
				start = start + end;
				end = start - end;
				start -= end;
			}

			DataColumn currColumn = this [start];
			for (int i = start; (direction>0 ? i<end : i>end); i += direction) {
				List [i] = List [i+direction];
				((DataColumn) List [i]).Ordinal = i;
			}
			List [end] = currColumn;
			currColumn.Ordinal = end;
		}


		/// <summary>
		/// Raises the OnCollectionChanged event.
		/// </summary>
		/// <param name="ccevent">A CollectionChangeEventArgs that contains the event data.</param>
		internal void OnCollectionChanged (CollectionChangeEventArgs ccevent)
		{
			parentTable.ResetPropertyDescriptorsCache ();
			if (CollectionChanged != null)
				CollectionChanged (this, ccevent);
		}

		/// <summary>
		/// Raises the OnCollectionChanging event.
		/// </summary>
		/// <param name="ccevent">A CollectionChangeEventArgs that contains the event data.</param>
		internal void OnCollectionChanging (CollectionChangeEventArgs ccevent)
		{
			if (CollectionChanged != null) {
				//FIXME: this is not right
				//CollectionChanged(this, ccevent);
				throw new NotImplementedException();
			}
		}
		
		internal void RegisterName (string name, DataColumn column)
		{
			try {
				columnFromName.Add (name, column);
			} catch (ArgumentException) {
				throw new DuplicateNameException ("A DataColumn named '" + name + "' already belongs to this DataTable.");
			}

			// Get existing doublet
			Doublet d = (Doublet) columnNameCount [name];
			if (d != null) {
				// Add reference count
				d.count++;
				// Add a new name
				d.columnNames.Add (name);
			} else {
				// no existing doublet
				// create one
				d = new Doublet (1, name);
				columnNameCount [name] = d;
			}

			if (name.Length <= ColumnPrefix.Length || !name.StartsWith (ColumnPrefix, StringComparison.Ordinal))
				return;

			if (name == MakeName (defaultColumnIndex + 1)) {
				do {
					defaultColumnIndex++;
				} while (Contains (MakeName (defaultColumnIndex + 1)));
			}
		}

		internal void UnregisterName (string name)
		{
			if (columnFromName.Contains (name))
				columnFromName.Remove (name);

			// Get the existing doublet
			Doublet d = (Doublet) columnNameCount [name];
			if (d != null) {
				// decrease reference count
				d.count--;
				d.columnNames.Remove (name);
				// remove doublet if no more references
				if (d.count == 0)
					columnNameCount.Remove (name);
			}

			if (name.StartsWith(ColumnPrefix) && name == MakeName(defaultColumnIndex - 1)) {
				do {
					defaultColumnIndex--;
				} while (!Contains (MakeName (defaultColumnIndex - 1)) && defaultColumnIndex > 1);
			}
		}

		internal void UpdateAutoIncrement (DataColumn col,bool isAutoIncrement)
		{
			if (isAutoIncrement) {
				if (!autoIncrement.Contains (col))
					autoIncrement.Add (col);
			} else {
				if (autoIncrement.Contains (col))
					autoIncrement.Remove (col);
			}
		}
		#endregion

		#region private class methods
		static readonly string[] TenColumns = { "Column0", "Column1", "Column2", "Column3", "Column4", "Column5", "Column6", "Column7", "Column8", "Column9" };
		static string MakeName (int index)
		{
			if (index < 10)
				return TenColumns [index];
			return String.Concat (ColumnPrefix, index.ToString());
		}
		#endregion
	}
}