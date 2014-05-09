//
// System.Data.DataTable.cs
//
// Author:
//   Franklin Wise <gracenote@earthlink.net>
//   Christopher Podurgiel (cpodurgiel@msn.com)
//   Daniel Morgan <danmorg@sc.rr.com>
//   Rodrigo Moya <rodrigo@ximian.com>
//   Tim Coleman (tim@timcoleman.com)
//   Ville Palo <vi64pa@koti.soon.fi>
//   Sureshkumar T <tsureshkumar@novell.com>
//   Konstantin Triger <kostat@mainsoft.com>
//
// (C) Chris Podurgiel
// (C) Ximian, Inc 2002
// Copyright (C) Tim Coleman, 2002-2003
// Copyright (C) Daniel Morgan, 2002-2003
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

using System.Globalization;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data{
	public class DataTable{
		#region public instance events
		/// <summary>
		/// Occurs when after a value has been changed for
		/// the specified DataColumn in a DataRow.
		/// </summary>
		[DataCategory ("Data")]
		public event DataColumnChangeEventHandler ColumnChanged;
		
		/// <summary>
		/// Occurs when a value is being changed for the specified
		/// DataColumn in a DataRow.
		/// </summary>
		[DataCategory ("Data")]
		public event DataColumnChangeEventHandler ColumnChanging;
		
		/// <summary>
		/// Gets or sets the namespace for the XML represenation
		/// of the data stored in the DataTable.
		/// </summary>
		[DataCategory ("Data")]
		public string Namespace {
			get {
				if (_nameSpace != null)
					return _nameSpace;
				if (DataSet != null)
					return DataSet.Namespace;
				return String.Empty;
			}
			set { _nameSpace = value; }
		}

		/// <summary>
		/// Occurs after a DataRow has been changed successfully.
		/// </summary>
		[DataCategory ("Data")]
		public event DataRowChangeEventHandler RowChanged;
		
		/// <summary>
		/// Occurs when a DataRow is changing.
		/// </summary>
		[DataCategory ("Data")]
		public event DataRowChangeEventHandler RowChanging;

		/// <summary>
		/// Occurs after the Clear method is called on the datatable.
		/// </summary>
		[DataCategory ("Data")]
		public event DataTableClearEventHandler TableCleared;

		[DataCategory ("Data")]
		public event DataTableClearEventHandler TableClearing;

		public event DataTableNewRowEventHandler TableNewRow;
		#endregion
		
		#region public instance properties
		/// <summary>
		/// Indicates whether string comparisons within the table are case-sensitive.
		/// </summary>
		public bool CaseSensitive {
			get {
				if (_virginCaseSensitive && dataSet != null)
					return dataSet.CaseSensitive;
				else
					return _caseSensitive;
			}
			set {
				/*
				if (_childRelations.Count > 0 || _parentRelations.Count > 0) {
					throw new ArgumentException ("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				*/
				_virginCaseSensitive = false;
				_caseSensitive = value;
				/*
				ResetCaseSensitiveIndexes();
				*/
			}
		}

		public DataColumnCollection Columns {
			get { return _columnCollection; }
		}

		/// <summary>
		/// Gets the DataSet that this table belongs to.
		/// </summary>
		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public DataSet DataSet {
			get { return dataSet; }
		}

		/// <summary>
		/// Gets or sets the locale information used to
		/// compare strings within the table.
		/// </summary>
		public CultureInfo Locale {
			get {
				// if the locale is null, we check for the DataSet locale
				// and if the DataSet is null we return the current culture.
				// this way if DataSet locale is changed, only if there is no locale for
				// the DataTable it influece the Locale get;
				if (_locale != null)
					return _locale;
				if (DataSet != null)
					return DataSet.Locale;
				return CultureInfo.CurrentCulture;
			}
			set {
				/*
				if (_childRelations.Count > 0 || _parentRelations.Count > 0) {
					throw new ArgumentException ("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				*/
				if (_locale == null || !_locale.Equals(value))
					_locale = value;
			}
		}

		public DataRowCollection Rows {
			get { return _rows; }
		}

		/// <summary>
		/// Gets or sets the name of the the DataTable.
		/// </summary>
		[DataCategory ("Data")]
		[DefaultValue ("")]
		[RefreshProperties (RefreshProperties.All)]
		public string TableName {
			get { return _tableName == null ? "" : _tableName; }
			set { _tableName = value; }
		}
		#endregion

		#region internal instance properties
		internal DataSet dataSet;
		
		internal int DefaultValuesRowIndex {
			get { return _defaultValuesRowIndex; }
		}
		
		internal bool InitInProgress {
			get { return fInitInProgress; }
			set { fInitInProgress = value; }
		}
		
		internal RecordCache RecordCache {
				get { return _recordCache; }
		}
		#endregion

		#region private instance fields
		private bool _caseSensitive;
		private DataColumnCollection _columnCollection;
		private int _defaultValuesRowIndex;
		protected internal bool fInitInProgress;
		private CultureInfo _locale;
		private int _minimumCapacity;
		private string _nameSpace;
		private PropertyDescriptorCollection _propertyDescriptorsCache;
		private RecordCache _recordCache;
		private DataRowBuilder _rowBuilder;
		private DataRowCollection _rows;
		private string _tableName;

		// If CaseSensitive property is changed once it does not anymore follow owner DataSet's
		// CaseSensitive property. So when you lost you virginity it's gone for ever
		private bool _virginCaseSensitive = true;

		private DataRowBuilder RowBuilder {
			get {
				// initiate only one row builder.
				if (_rowBuilder == null)
					_rowBuilder = new DataRowBuilder (this, -1, 0);
				else
					// new row get id -1.
					_rowBuilder._rowId = -1;
				
				return _rowBuilder;
			}
		}
		#endregion
		

		#region public instance constructors
		/// <summary>
		/// Initializes a new instance of the DataTable class with no arguments.
		/// </summary>
		public DataTable ()
		{
			dataSet = null;
			_columnCollection = new DataColumnCollection(this);
			/*
			_constraintCollection = new ConstraintCollection(this);
			_extendedProperties = new PropertyCollection();
			*/
			_tableName = "";
			_nameSpace = null;
			_caseSensitive = false;	 	//default value
			/*
			_displayExpression = null;
			_primaryKeyConstraint = null;
			_site = null;
			*/
			_rows = new DataRowCollection (this);
			/*
			_indexes = new ArrayList();
			*/
			_recordCache = new RecordCache(this);

			//LAMESPEC: spec says 25 impl does 50
			_minimumCapacity = 50;
			
			/*
			_childRelations = new DataRelationCollection.DataTableRelationCollection (this);
			_parentRelations = new DataRelationCollection.DataTableRelationCollection (this);
			*/
		}

		/// <summary>
		/// Intitalizes a new instance of the DataTable class with the specified table name.
		/// </summary>
		public DataTable (string tableName) : this ()
		{
			_tableName = tableName;
		}
		#endregion
		
		#region protected instance methods
		/// <summary>
		/// This member is only meant to support Mono's infrastructure
		/// </summary>
		protected virtual Type GetRowType ()
		{
			return typeof (DataRow);
		}
		
		/// <summary>
		/// Creates a new row from an existing row.
		/// </summary>
		protected virtual DataRow NewRowFromBuilder (DataRowBuilder builder)
		{
			return new DataRow (builder);
		}

		/// <summary>
		/// Raises the ColumnChanged event.
		/// </summary>
		protected virtual void OnColumnChanged (DataColumnChangeEventArgs e)
		{
			if (null != ColumnChanged)
				ColumnChanged (this, e);
		}

		/// <summary>
		/// Raises the ColumnChanging event.
		/// </summary>
		protected virtual void OnColumnChanging (DataColumnChangeEventArgs e)
		{
			if (null != ColumnChanging)
				ColumnChanging (this, e);
		}
		
		/// <summary>
		/// Raises the RowChanged event.
		/// </summary>
		protected virtual void OnRowChanged (DataRowChangeEventArgs e)
		{
			if (null != RowChanged)
				RowChanged (this, e);
		}
		
		/// <summary>
		/// Raises the RowChanging event.
		/// </summary>
		protected virtual void OnRowChanging (DataRowChangeEventArgs e)
		{
			if (null != RowChanging)
				RowChanging (this, e);
		}
		
		/// <summary>
		/// Raises TableCleared Event and delegates to subscribers
		/// </summary>
		protected virtual void OnTableCleared (DataTableClearEventArgs e)
		{
			if (TableCleared != null)
				TableCleared (this, e);
		}
		
		protected virtual void OnTableClearing (DataTableClearEventArgs e)
		{
			if (TableClearing != null)
				TableClearing (this, e);
		}

		internal void RaiseOnColumnChanged (DataColumnChangeEventArgs e)
		{
			OnColumnChanged (e);
		}
		#endregion
		
		#region internal instance methods
		internal void ResetPropertyDescriptorsCache ()
		{
			_propertyDescriptorsCache = null;
		}
		
		internal void ChangedDataColumn (DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs (dr, dc, pv);
			OnColumnChanged (e);
		}
		
		internal void ChangedDataRow (DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs (dr, action);
			OnRowChanged (e);
		}
		
		internal void ChangingDataColumn (DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs (dr, dc, pv);
			OnColumnChanging (e);
		}

		internal void ChangingDataRow (DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs (dr, action);
			OnRowChanging (e);
		}
		
		internal int CreateRecord (object [] values)
		{
			int valCount = values != null ? values.Length : 0;
			if (valCount > Columns.Count)
				throw new ArgumentException ("Input array is longer than the number of columns in this table.");
			
			int index = RecordCache.NewRecord ();
			
			try {
				for (int i = 0; i < valCount; i++) {
					object value = values[i];
					if (value == null)
						Columns [i].SetDefaultValue (index);
					else
						Columns [i][index] = values [i];
				}
				
				for(int i = valCount; i < Columns.Count; i++)
					Columns [i].SetDefaultValue (index);
				
				return index;
			} catch {
				RecordCache.DisposeRecord (index);
				throw;
			}
		}
		
		internal void DataTableCleared ()
		{
			OnTableCleared (new DataTableClearEventArgs (this));
		}
		
		internal void DataTableClearing ()
		{
			OnTableClearing (new DataTableClearEventArgs (this));
		}
		
		internal DataRow NewNotInitializedRow ()
		{
			/*
			EnsureDefaultValueRowIndex ();
			*/
			return NewRowFromBuilder (RowBuilder);
		}

		/// <summary>
		/// This member supports the .NET Framework infrastructure
		///  and is not intended to be used directly from your code.
		/// </summary>
		DataRow [] empty_rows;
		protected internal DataRow [] NewRowArray (int size)
		{
			if (size == 0 && empty_rows != null)
				return empty_rows;
			Type t = GetRowType ();
			/* Avoid reflection if possible */
			DataRow [] rows = t == typeof (DataRow) ? new DataRow [size] : (DataRow []) Array.CreateInstance (t, size);
			if (size == 0)
				empty_rows = rows;
			return rows;
		}
		#endregion
	}
}
