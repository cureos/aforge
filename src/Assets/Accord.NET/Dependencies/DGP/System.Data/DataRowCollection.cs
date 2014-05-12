//
// System.Data.DataRowCollection.cs
//
// Author:
//   Daniel Morgan <danmorg@sc.rr.com>
//   Tim Coleman <tim@timcoleman.com>
//
// (C) Ximian, Inc 2002
// (C) Copyright 2002 Tim Coleman
// (C) Copyright 2002 Daniel Morgan
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

using System.ComponentModel;

namespace System.Data{
	public class DataRowCollection : InternalDataCollectionBase{
		#region internal event definitions
		internal event ListChangedEventHandler ListChanged;
		#endregion
		
		#region public instance properties
		/// <summary>
		/// Gets the row at the specified index.
		/// </summary>
		public DataRow this [int index] {
			get {
				if (index < 0 || index >= Count)
					throw new IndexOutOfRangeException ("There is no row at position " + index + ".");

				return (DataRow) List [index];
			}
		}
		#endregion

		#region private instance fields
		private DataTable table;
		#endregion

		#region internal constructor
		/// <summary>
		/// Internal constructor used to build a DataRowCollection.
		/// </summary>
		internal DataRowCollection (DataTable table)
		{
			this.table = table;
		}
		#endregion

		#region public instance methods
		/// <summary>
		/// Adds the specified DataRow to the DataRowCollection object.
		/// </summary>
		public void Add (DataRow row)
		{
			//TODO: validation
			if (row == null)
				throw new ArgumentNullException ("row", "'row' argument cannot be null.");

			if (row.Table != this.table)
					throw new ArgumentException ("This row already belongs to another table.");
			
			// If row id is not -1, we know that it is in the collection.
			if (row.RowID != -1)
				throw new ArgumentException ("This row already belongs to this table.");
			
			row.BeginEdit ();

			row.Validate ();

			AddInternal (row);
		}
		
		/// <summary>
		/// Creates a row using specified values and adds it to the DataRowCollection.
		/// </summary>
		public DataRow Add (params object[] values)
		{
			if (values == null)
				throw new NullReferenceException ();
			DataRow row = table.NewNotInitializedRow ();
			int newRecord = table.CreateRecord (values);
			row.ImportRecord (newRecord);

			row.Validate ();
			AddInternal (row);
			return row;
		}

		/// <summary>
		/// Clears the collection of all rows.
		/// </summary>
		public void Clear ()
		{
			/*
			if (this.table.DataSet != null && this.table.DataSet.EnforceConstraints) {
				foreach (Constraint c in table.Constraints) {
					UniqueConstraint uc = c as UniqueConstraint;
					if (uc == null)
							continue;
					if (uc.ChildConstraint == null || uc.ChildConstraint.Table.Rows.Count == 0)
						continue;

					string err = String.Format (
						"Cannot clear table Parent because ForeignKeyConstraint {0} enforces Child.", 
						uc.ConstraintName
					);
					throw new InvalidConstraintException (err);
				}
			}
			*/
			
			table.DataTableClearing ();
			List.Clear ();

			// Remove from indexes
			/*
			table.ResetIndexes ();
			*/
			table.DataTableCleared ();
			OnListChanged (this, new ListChangedEventArgs (ListChangedType.Reset, -1, -1));
		}
		
		public int IndexOf (DataRow row)
		{
			if (row == null || row.Table != table)
				return -1;

			int id = row.RowID;
			return (id >= 0 && id < List.Count && row == List [id]) ? id : -1;
		}
		
		/// <summary>
		/// Removes the specified DataRow from the collection.
		/// </summary>
		public void Remove (DataRow row)
		{
			if (IndexOf (row) < 0)
				throw new IndexOutOfRangeException ("The given datarow is not in the current DataRowCollection.");

			DataRowState state = row.RowState;
			if (state != DataRowState.Deleted && state != DataRowState.Detached) {
				row.Delete ();
				// if the row was in added state it will be in Detached state after the
				// delete operation, so we have to check it.
				if (row.RowState != DataRowState.Detached)
					row.AcceptChanges ();
			}
		}

		/// <summary>
		/// Removes the row at the specified index from the collection.
		/// </summary>
		public void RemoveAt (int index)
		{
			Remove (this [index]);
		}
		#endregion

		#region internal instance methods
		internal void AddInternal (DataRow row)
		{
			AddInternal (row, DataRowAction.Add);
		}

		internal void AddInternal (DataRow row, DataRowAction action)
		{
			row.Table.ChangingDataRow (row, action);
			List.Add (row);
			row.AttachAt (List.Count - 1, action);
			row.Table.ChangedDataRow (row, action);
			if (row._rowChanged)
				row._rowChanged = false;
		}
		
		internal void OnListChanged (object sender, ListChangedEventArgs args)
		{
			if (ListChanged != null)
				ListChanged (sender, args);
		}
		
		/// <summary>
		/// Removes the specified DataRow from the internal list. Used by DataRow to commit the removing.
		/// </summary>
		internal void RemoveInternal (DataRow row)
		{
			if (row == null)
				throw new IndexOutOfRangeException ("The given datarow is not in the current DataRowCollection.");
			int index = this.IndexOf (row);
			if (index < 0)
				throw new IndexOutOfRangeException ("The given datarow is not in the current DataRowCollection.");
			List.RemoveAt (index);
			for (; index < List.Count; ++index)
				((DataRow) List [index]).RowID = index;
		}

		#endregion
	}
}