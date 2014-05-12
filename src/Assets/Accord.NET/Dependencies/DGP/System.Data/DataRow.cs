//
// System.Data.DataRow.cs
//
// Author:
//   Rodrigo Moya <rodrigo@ximian.com>
//   Daniel Morgan <danmorg@sc.rr.com>
//   Tim Coleman <tim@timcoleman.com>
//   Ville Palo <vi64pa@koti.soon.fi>
//   Alan Tam Siu Lung <Tam@SiuLung.com>
//   Sureshkumar T <tsureshkumar@novell.com>
//   Veerapuram Varadhan <vvaradhan@novell.com>
//
// (C) Ximian, Inc 2002
// (C) Daniel Morgan 2002, 2003
// Copyright (C) 2002 Tim Coleman
//

//
// Copyright (C) 2004-2009 Novell, Inc (http://www.novell.com)
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

using System.Globalization;

namespace System.Data{
	/// <summary>
	/// Represents a row of data in a DataTable.
	/// </summary>
	[Serializable]
	public class DataRow{
		#region public instance properties
		/// <summary>
		/// Gets or sets the data stored in the column specified by name.
		/// </summary>
		public object this [string columnName] {
			get { return this [columnName, DataRowVersion.Default]; }
			set {
				DataColumn column = _table.Columns [columnName];
				if (column == null)
					throw new ArgumentException (
							"The column '" + columnName + 
							"' does not belong to the table : " + _table.TableName
					);
				this [column.Ordinal] = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the data stored in specified DataColumn
		/// </summary>
		public object this [DataColumn column] {
			get { return this [column, DataRowVersion.Default]; }
			set {
				if (column == null)
					throw new ArgumentNullException ("column");
				int columnIndex = _table.Columns.IndexOf (column);
				if (columnIndex == -1)
					throw new ArgumentException (string.Format (
						CultureInfo.InvariantCulture,
						"The column '{0}' does not belong to the table : {1}.",
						column.ColumnName, 
						_table.TableName
					));
				this [columnIndex] = value;
			}
		}

		/// <summary>
		/// Gets or sets the data stored in column specified by index.
		/// </summary>
		public object this [int columnIndex] {
			get { return this [columnIndex, DataRowVersion.Default]; }
			set {
				if (columnIndex < 0 || columnIndex > _table.Columns.Count)
					throw new IndexOutOfRangeException ();
				if (RowState == DataRowState.Deleted)
					throw new DeletedRowInaccessibleException ();

				DataColumn column = _table.Columns [columnIndex];
				_table.ChangingDataColumn (this, column, value);
				
				if (value == null && column.DataType.IsValueType)
					throw new ArgumentException (
						"Canot set column '"
						+ column.ColumnName + "' to be null."
						+ " Please use DBNull instead."
					);

				_rowChanged = true;

				CheckValue (value, column);
				bool already_editing = Proposed >= 0;
				if (!already_editing)
					BeginEdit ();

				column [Proposed] = value;
				_table.ChangedDataColumn (this, column, value);

				if (!already_editing)
					EndEdit ();
			}
		}

		/// <summary>
		/// Gets the specified version of data stored in the named column.
		/// </summary>
		public object this [string columnName, DataRowVersion version] {
			get {
				DataColumn column = _table.Columns [columnName];
				if (column == null)
					throw new ArgumentException (
						"The column '" + columnName +
						"' does not belong to the table : " + _table.TableName
					);
				return this [column.Ordinal, version];
			}
		}

		/// <summary>
		/// Gets the specified version of data stored in the specified DataColumn.
		/// </summary>
		public object this [DataColumn column, DataRowVersion version] {
			get {
				if (column == null)
					throw new ArgumentNullException ("column");
				if (column.Table != Table)
					throw new ArgumentException (string.Format (
						CultureInfo.InvariantCulture,
						"The column '{0}' does not belong to the table : {1}.",
						column.ColumnName, 
						_table.TableName
					));
				return this [column.Ordinal, version];
			}
		}
					
		/// <summary>
		/// Gets the data stored in the column, specified by index and version of the data to
		/// retrieve.
		/// </summary>
		public object this [int columnIndex, DataRowVersion version] {
			get {
				if (columnIndex < 0 || columnIndex > _table.Columns.Count)
					throw new IndexOutOfRangeException ();

				DataColumn column = _table.Columns [columnIndex];
				int recordIndex = IndexFromVersion (version);
				
				/*
				if (column.Expression != String.Empty && _table.Rows.IndexOf (this) != -1) {
					// FIXME: how does this handle 'version'?
					// TODO: Can we avoid the Eval each time by using the cached value?
					object o = column.CompiledExpression.Eval (this);
					if (o != null && o != DBNull.Value)
							o = Convert.ChangeType (o, column.DataType);
					column [recordIndex] = o;
					return column [recordIndex];
				}
				*/
				return column [recordIndex];
			}
		}


		/// <summary>
		/// Gets the DataTable for which this row has a schema.
		/// </summary>
		public DataTable Table {
			get { return _table; }
			internal set { _table = value; }
		}
		#endregion

		#region private instance fields
		private bool _inChangingEvent;
		private int _rowId;
		private DataTable _table;
		#endregion

		#region internal instance fields
		internal int _current = -1;
		internal int _original = -1;
		internal int _proposed = -1;
		internal bool _rowChanged = false;
		
		internal int Current {
			get { return _current; }
			set {
				if (Table != null) {
					//Table.RecordCache[_current] = null;
					Table.RecordCache [value] = this;
				}
				_current = value;
			}
		}

		internal int Original {
			get { return _original; }
			set {
				if (Table != null) {
					//Table.RecordCache[_original] = null;
					Table.RecordCache [value] = this;
				}
				_original = value;
			}
		}

		internal int Proposed {
			get { return _proposed; }
			set {
				if (Table != null) {
					//Table.RecordCache[_proposed] = null;
					Table.RecordCache [value] = this;
				}
				_proposed = value;
			}
		}
		
		/// <summary>
		/// Gets and sets index of row.
		// </summary>
		internal int RowID {
			get { return _rowId; }
			set { _rowId = value; }
		}

		/// <summary>
		/// Gets the current state of the row in regards to its relationship to the
		/// DataRowCollection.
		/// </summary>
		public DataRowState RowState {
			get {
				//return rowState;
				if (Original == -1 && Current == -1)
					return DataRowState.Detached;
				if (Original == Current)
					return DataRowState.Unchanged;
				if (Original == -1)
					return DataRowState.Added;
				if (Current == -1)
					return DataRowState.Deleted;
				return DataRowState.Modified;
			}
			internal set {
				if (DataRowState.Detached == value) {
					Original = -1;
					Current = -1;
				}
				
				if (DataRowState.Unchanged == value)
					Original = Current;
				if (DataRowState.Added == value)
					Original = -1;
				if (DataRowState.Deleted == value)
					Current = -1;
			}
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// This member supports the .NET Framework infrastructure and is not intended to be
		/// used directly from your code.
		/// </summary>
		protected internal DataRow (DataRowBuilder builder)
		{
			_table = builder.Table;
			// Get the row id from the builder.
			_rowId = builder._rowId;
			/*
			rowError = String.Empty;
			*/
		}

		internal DataRow (DataTable table, int rowId)
		{
			_table = table;
			_rowId = rowId;
		}
		#endregion // Constructors

		#region public instance methods
		/// <summary>
		/// Commits all the changes made to this row since the last time AcceptChanges was
		/// called.
		/// </summary>
		public void AcceptChanges ()
		{
			EndEdit (); // in case it hasn't been called

			_table.ChangingDataRow (this, DataRowAction.Commit);
			CheckChildRows (DataRowAction.Commit);
			switch (RowState) {
			case DataRowState.Added:
			case DataRowState.Modified:
				if (Original >= 0)
					Table.RecordCache.DisposeRecord (Original);
				Original = Current;
				break;
			case DataRowState.Deleted:
				Detach ();
				break;
			case DataRowState.Detached:
				throw new RowNotInTableException("Cannot perform this operation on a row not in the table.");
			}

			_table.ChangedDataRow (this, DataRowAction.Commit);
		}

		/// <summary>
		/// Begins an edit operation on a DataRow object.
		/// </summary>
		public void BeginEdit ()
		{
			if (_inChangingEvent)
				throw new InRowChangingEventException ("Cannot call BeginEdit inside an OnRowChanging event.");
			if (RowState == DataRowState.Deleted)
				throw new DeletedRowInaccessibleException ();

			if (!HasVersion (DataRowVersion.Proposed)) {
				Proposed = Table.RecordCache.NewRecord ();
				int from = HasVersion (DataRowVersion.Current) ? Current : Table.DefaultValuesRowIndex;
				for (int i = 0; i < Table.Columns.Count; i++){
					DataColumn column = Table.Columns [i];
					column.DataContainer.CopyValue (from, Proposed);
				}
			}
		}

		/// <summary>
		/// Cancels the current edit on the row.
		/// </summary>
		public void CancelEdit ()
		{
			if (_inChangingEvent)
				throw new InRowChangingEventException ("Cannot call CancelEdit inside an OnRowChanging event.");

			if (HasVersion (DataRowVersion.Proposed)) {
				int oldRecord = Proposed;
				DataRowState oldState = RowState;
				Table.RecordCache.DisposeRecord(Proposed);
				Proposed = -1;
				
				/*
				foreach (Index index in Table.Indexes)
					index.Update (this, oldRecord, DataRowVersion.Proposed, oldState);
				*/
			}
		}

		/// <summary>
		/// Deletes the DataRow.
		/// </summary>
		public void Delete ()
		{
			_table.DeletingDataRow (this, DataRowAction.Delete);
			switch (RowState) {
			case DataRowState.Added:
				CheckChildRows (DataRowAction.Delete);
				Detach ();
				break;
			case DataRowState.Deleted:
			case DataRowState.Detached:
				break;
			default:
				// check what to do with child rows
				CheckChildRows (DataRowAction.Delete);
				break;
			}
			
			if (Current >= 0) {
				int current = Current;
				DataRowState oldState = RowState;
				if (Current != Original)
					_table.RecordCache.DisposeRecord (Current);
				
				Current = -1;
				/*
				foreach (Index index in Table.Indexes)
					index.Update (this, current, DataRowVersion.Current, oldState);
				*/
			}
			
			_table.DeletedDataRow (this, DataRowAction.Delete);
		}
		
		/// <summary>
		/// Ends the edit occurring on the row.
		/// </summary>
		public void EndEdit ()
		{
			if (_inChangingEvent)
				throw new InRowChangingEventException ("Cannot call EndEdit inside an OnRowChanging event.");

			if (RowState == DataRowState.Detached || !HasVersion (DataRowVersion.Proposed))
				return;

			CheckReadOnlyStatus ();

			_inChangingEvent = true;
			try {
				_table.ChangingDataRow (this, DataRowAction.Change);
			} finally {
				_inChangingEvent = false;
			}

			DataRowState oldState = RowState;

			int oldRecord = Current;
			Current = Proposed;
			Proposed = -1;
			
			/*
			//FIXME : ideally  indexes shouldnt be maintained during dataload.But this needs to
			//be implemented at multiple places.For now, just maintain the index.
			//if (!Table._duringDataLoad) {
			foreach (Index index in Table.Indexes)
				index.Update (this, oldRecord, DataRowVersion.Current, oldState);
			//}
			*/
			
			try {
				/*
				AssertConstraints ();
				*/

				// restore previous state to let the cascade update to find the rows 
				Proposed = Current;
				Current = oldRecord; 

				CheckChildRows (DataRowAction.Change);

				// apply new state
				Current = Proposed;
				Proposed = -1;
			} catch {
				int proposed = Proposed >= 0 ? Proposed : Current;
				Current = oldRecord;
				/*
				//if (!Table._duringDataLoad) {
				foreach (Index index in Table.Indexes)
					index.Update (this, proposed, DataRowVersion.Current, RowState);
				//}
				*/
				throw;
			}

			if (Original != oldRecord)
				Table.RecordCache.DisposeRecord (oldRecord);

			// Note : row state must not be changed before all the job on indexes finished,
			// since the indexes works with recods rather than with rows and the decision
			// which of row records to choose depends on row state.
			if (_rowChanged == true) {
				_table.ChangedDataRow (this, DataRowAction.Change);
				_rowChanged = false;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether a specified version exists.
		/// </summary>
		public bool HasVersion (DataRowVersion version)
		{
				switch (version) {
				case DataRowVersion.Default:
						return (Proposed >= 0 || Current >= 0);
				case DataRowVersion.Proposed:
						return Proposed >= 0;
				case DataRowVersion.Current:
						return Current >= 0;
				case DataRowVersion.Original:
						return Original >= 0;
				default:
						return IndexFromVersion (version) >= 0;
				}
		}
		
		/// <summary>
		/// Gets a value indicating whether the specified DataColumn contains a null value.
		/// </summary>
		public bool IsNull (DataColumn column)
		{
			return IsNull (column, DataRowVersion.Default);
		}

		/// <summary>
		/// Gets a value indicating whether the column at the specified index contains a null
		/// value.
		/// </summary>
		public bool IsNull (int columnIndex)
		{
			return IsNull (Table.Columns [columnIndex]);
		}

		/// <summary>
		/// Gets a value indicating whether the named column contains a null value.
		/// </summary>
		public bool IsNull (string columnName)
		{
			return IsNull (Table.Columns [columnName]);
		}

		/// <summary>
		/// Gets a value indicating whether the specified DataColumn and DataRowVersion
		/// contains a null value.
		/// </summary>
		public bool IsNull (DataColumn column, DataRowVersion version)
		{
			return column.DataContainer.IsNull (IndexFromVersion (version));
		}
		#endregion
		
		#region private instance methods
		private int AssertValidVersionIndex (DataRowVersion version, int index)
		{
			if (index >= 0)
				return index;

			throw new VersionNotFoundException (String.Format ("There is no {0} data to access.", version));
		}
		
		// check the child rows of this row before deleting the row.
		private void CheckChildRows (DataRowAction action)
		{
			/*
			DataSet ds = _table.DataSet;

			if (ds == null || !ds.EnforceConstraints)
					return;

			// if the table we're attached-to doesn't have an constraints, no foreign keys are pointing to us ...
			if (_table.Constraints.Count == 0)
					return;

			foreach (DataTable table in ds.Tables) {
				// loop on all ForeignKeyConstrain of the table.
				foreach (Constraint constraint in table.Constraints) {
					ForeignKeyConstraint fk = constraint as ForeignKeyConstraint;
					if (fk == null || fk.RelatedTable != _table)
							continue;

					switch (action) {
					case DataRowAction.Delete:
							CheckChildRows (fk, action, fk.DeleteRule);
							break;
					case DataRowAction.Commit:
					case DataRowAction.Rollback:
							if (fk.AcceptRejectRule != AcceptRejectRule.None)
									CheckChildRows (fk, action, Rule.Cascade);
							break;
					default:
							CheckChildRows (fk, action, fk.UpdateRule);
							break;
					}
				}
			}
			*/
		}
						
		private void CheckValue (object v, DataColumn col)
		{
			CheckValue (v, col, true);
		}

		private void CheckValue (object v, DataColumn col, bool doROCheck)
		{
			if (doROCheck && _rowId != -1 && col.ReadOnly)
				throw new ReadOnlyException ();

			if (v == null || v == DBNull.Value) {
				if (col.AllowDBNull || col.AutoIncrement || col.DefaultValue != DBNull.Value)
					return;
				/*
				//Constraint violations during data load is raise in DataTable EndLoad
				this._nullConstraintViolation = true;
				if (this.Table._duringDataLoad || (Table.DataSet != null && !Table.DataSet.EnforceConstraints))
						this.Table._nullConstraintViolationDuringDataLoad = true;
				_nullConstraintMessage = "Column '" + col.ColumnName + "' does not allow nulls.";
				*/
			}
		}
		
		void Detach ()
		{
			/*
			Table.DeleteRowFromIndexes (this);
			*/
			Table.Rows.RemoveInternal (this);

			if (Proposed >= 0 && Proposed != Current && Proposed != Original)
					_table.RecordCache.DisposeRecord (Proposed);
			Proposed = -1;

			if (Current >= 0 && Current != Original)
					_table.RecordCache.DisposeRecord (Current);
			Current = -1;

			if (Original >= 0)
					_table.RecordCache.DisposeRecord (Original);
			Original = -1;

			_rowId = -1;
		}
		#endregion

		#region internal instance methods
		// Called by DataRowCollection.Add/InsertAt
		internal void AttachAt (int row_id, DataRowAction action)
		{
			_rowId = row_id;
			if (Proposed != -1) {
				if (Current >= 0)
					Table.RecordCache.DisposeRecord (Current);
				Current = Proposed;
				Proposed = -1;
			}
			
			if ((action & (DataRowAction.ChangeCurrentAndOriginal | DataRowAction.ChangeOriginal)) != 0)
				Original = Current;
		}
		
		internal void CheckReadOnlyStatus()
		{
			int defaultIdx = IndexFromVersion (DataRowVersion.Default);
			foreach(DataColumn column in Table.Columns) {
				if ((column.DataContainer.CompareValues (defaultIdx,Proposed) != 0) && column.ReadOnly)
					throw new ReadOnlyException ();
			}
		}
		
		internal void ImportRecord (int record)
		{
			if (HasVersion (DataRowVersion.Proposed))
				Table.RecordCache.DisposeRecord (Proposed);

			Proposed = record;

			foreach (DataColumn column in Table.Columns.AutoIncrmentColumns)
				column.UpdateAutoIncrementValue (column.DataContainer.GetInt64 (Proposed));

			foreach (DataColumn col in Table.Columns)
				CheckValue (this [col], col, false);
		}

		internal int IndexFromVersion (DataRowVersion version)
		{
			switch (version) {
			case DataRowVersion.Default:
				if (Proposed >= 0)
					return Proposed;

				if (Current >= 0)
					return Current;

				if (Original < 0)
					throw new RowNotInTableException ("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");

				throw new DeletedRowInaccessibleException ("Deleted row information cannot be accessed through the row.");

			case DataRowVersion.Proposed:
					return AssertValidVersionIndex (version, Proposed);
			case DataRowVersion.Current:
					return AssertValidVersionIndex (version, Current);
			case DataRowVersion.Original:
					return AssertValidVersionIndex (version, Original);
			default:
					throw new DataException ("Version must be Original, Current, or Proposed.");
			}
		}
		
		internal void Validate ()
		{
				/*
				Table.AddRowToIndexes (this);
				AssertConstraints ();
				*/
		}
		#endregion
	}
}
