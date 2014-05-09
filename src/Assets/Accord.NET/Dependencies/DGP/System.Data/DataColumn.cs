//
// System.Data.DataColumn.cs
//
// Author:
//   Franklin Wise (gracenote@earthlink.net)
//   Christopher Podurgiel (cpodurgiel@msn.com)
//   Rodrigo Moya (rodrigo@ximian.com)
//   Daniel Morgan (danmorg@sc.rr.com)
//   Tim Coleman (tim@timcoleman.com)
//
// (C) Copyright 2002, Franklin Wise
// (C) Chris Podurgiel
// (C) Ximian, Inc 2002
// Copyright (C) Tim Coleman, 2002
// Copyright (C) Daniel Morgan, 2002, 2003
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
// Copyright (C) 2011 Xamarin Inc (http://www.xamarin.com)
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
using System.ComponentModel;
using System.Globalization;
using System.Data.Common;

namespace System.Data{

	[DefaultProperty ("ColumnName")]
	public class DataColumn : MarshalByValueComponent{
		#region Events
		EventHandlerList _eventHandlers = new EventHandlerList ();

		//used for constraint validation
		//if an exception is fired during this event the change should be canceled
		//[MonoTODO]
		//internal event DelegateColumnValueChange ValidateColumnValueChange;

		//used for FK Constraint Cascading rules
		//[MonoTODO]
		//internal event DelegateColumnValueChange ColumnValueChanging;

		static readonly object _propertyChangedKey = new object ();
		internal event PropertyChangedEventHandler PropertyChanged {
			add { _eventHandlers.AddHandler (_propertyChangedKey, value); }
			remove { _eventHandlers.RemoveHandler (_propertyChangedKey, value); }
		}

		#endregion //Events
		
		#region internal instance properties
		internal static bool CanAutoIncrement (Type type)
		{
			switch (Type.GetTypeCode (type)) {
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
				return true;
			}

			return false;
		}
		
		internal DataContainer DataContainer {
				get { return _dataContainer; }
		}
		#endregion

		#region public instance properties
		internal object this [int index] {
			get { return DataContainer [index]; }
			set {
				if (!(value == null && AutoIncrement)) {
					try {
						DataContainer [index] = value;
					} catch(Exception e) {
						throw new ArgumentException (
							String.Format (
								"{0}. Couldn't store <{1}> in Column named '{2}'. Expected type is {3}.",
								e.Message, 
								value, 
								ColumnName, 
								DataType.Name
							)
							,
							e
						);
					}
				}

				if (AutoIncrement && !DataContainer.IsNull (index)) {
					long value64 = Convert.ToInt64 (value);
					UpdateAutoIncrementValue (value64);
				}
			}
		}

		[DataCategory ("Data")]
		[DefaultValue (true)]
		public bool AllowDBNull {
			get { return _allowDBNull; }
			set {
				if (!value && null != _table) {
					for (int r = 0; r < _table.Rows.Count; r++) {
						DataRow row = _table.Rows [r];
						DataRowVersion version = row.HasVersion (DataRowVersion.Default) ?
							DataRowVersion.Default : DataRowVersion.Original;
						if (row.IsNull (this, version))
							throw new DataException ("Column '" + ColumnName + "' has null values in it.");
						//TODO: do we also check different versions of the row??
					}
				}

				_allowDBNull = value;
			}
		}

		
		[DefaultValue (false)]
		[RefreshProperties (RefreshProperties.All)]
		public bool AutoIncrement {
			get { return _autoIncrement; }
			set {
				if (value) {
					//Can't be true if this is a computed column
					/*
					if (Expression != string.Empty)
						throw new ArgumentException ("Can not Auto Increment a computed column.");
					*/
					if (DefaultValue != DBNull.Value)
						throw new ArgumentException ("Can not set AutoIncrement while default value exists for this column.");

					if (!CanAutoIncrement (DataType))
						DataType = typeof (Int32);
				}

				if (_table != null)
					_table.Columns.UpdateAutoIncrement (this, value);
				_autoIncrement = value;
			}
		}
		
		[DataCategory ("Data")]
		[DefaultValue (1)]
		public long AutoIncrementStep {
			get { return _autoIncrementStep; }
			set { _autoIncrementStep = value; }
		}
		
		[DataCategory ("Data")]
		public string Caption {
			get { return _caption == null ? ColumnName : _caption; }
			set { _caption = value == null ? String.Empty : value; }
		}

		[DefaultValue (MappingType.Element)]
		public virtual MappingType ColumnMapping {
			get { return _columnMapping; }
			set { _columnMapping = value; }
		}

		[DataCategory ("Data")]
		[RefreshProperties (RefreshProperties.All)]
		[DefaultValue ("")]
		public string ColumnName {
			get { return _columnName; }
			set {
				if (value == null)
					value = String.Empty;

				CultureInfo info = Table != null ? Table.Locale : CultureInfo.CurrentCulture;
				if (String.Compare (value, _columnName, true, info) != 0) {
					if (Table != null) {
						if (value.Length == 0)
							throw new ArgumentException ("ColumnName is required when it is part of a DataTable.");

						Table.Columns.RegisterName (value, this);
						if (_columnName.Length > 0)
							Table.Columns.UnregisterName (_columnName);
					}

					RaisePropertyChanging ("ColumnName");
					_columnName = value;

					if (Table != null)
						Table.ResetPropertyDescriptorsCache ();
				} else if (String.Compare (value, _columnName, false, info) != 0) {
					RaisePropertyChanging ("ColumnName");
					_columnName = value;

					if (Table != null)
						Table.ResetPropertyDescriptorsCache ();
				}
			}
		}

		[DataCategory ("Data")]
		[DefaultValue (typeof (string))]
		[RefreshProperties (RefreshProperties.All)]
		[TypeConverterAttribute (typeof (ColumnTypeConverter))]
		public Type DataType {
			get { return DataContainer.Type; }
			set {
				if (value == null)
					return;

				if (_dataContainer != null) {
					if (value == _dataContainer.Type)
						return;

					// check if data already exists can we change the datatype
					if (_dataContainer.Capacity > 0)
						throw new ArgumentException ("The column already has data stored.");
				}

				/*
				if (null != GetParentRelation () || null != GetChildRelation ())
					throw new InvalidConstraintException ("Cannot change datatype when column is part of a relation");
				*/
				Type prevType = _dataContainer != null ? _dataContainer.Type : null; // current

				if (_dataContainer != null && _dataContainer.Type == typeof (DateTime))
					_datetimeMode = DataSetDateTime.UnspecifiedLocal;

				_dataContainer = DataContainer.Create (value, this);

				//Check AutoIncrement status, make compatible datatype
				if(AutoIncrement == true) {
					// we want to check that the datatype is supported?					
					if (!CanAutoIncrement (value))
						AutoIncrement = false;
				}

				if (DefaultValue != GetDefaultValueForType (prevType))
					SetDefaultValue (DefaultValue, true);
				else
					_defaultValue = GetDefaultValueForType (DataType);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>When AutoIncrement is set to true, there can be no default value.</remarks>
		/// <exception cref="System.InvalidCastException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		[DataCategory ("Data")]
		[TypeConverterAttribute (typeof (System.Data.DefaultValueTypeConverter))]
		public object DefaultValue {
			get { return _defaultValue; }

			set {
				if (AutoIncrement)
					throw new ArgumentException ("Can not set default value while AutoIncrement is true on this column.");
				SetDefaultValue (value, false);
			}
		}

		[DataCategory ("Data")]
		[DefaultValue (-1)] //Default == -1 no max length
		public int MaxLength {
			get { return _maxLength; }
			set {
				if (value >= 0 && _columnMapping == MappingType.SimpleContent)
					throw new ArgumentException (
						String.Format (
							"Cannot set MaxLength property on '{0}' column which is mapped to SimpleContent.",
							ColumnName));
				//only applies to string columns
				_maxLength = value;
			}
		}

		//Need a good way to set the Ordinal when the column is added to a columnCollection.
		[DataCategory ("Data")]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public int Ordinal {
			get { return _ordinal; }
			internal set { _ordinal = value; }
		}

		[DataCategory ("Data")]
		[DefaultValue (false)]
		public bool ReadOnly {
			get { return _readOnly; }
			set { _readOnly = value; }
		}

		[Browsable (false)]
		[DataCategory ("Data")]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public DataTable Table {
			get { return _table; }
			internal set { _table = value; }
		}
		#endregion

		#region private instance fields
		private bool _allowDBNull = true;
		private bool _autoIncrement;
		private long _autoIncrementSeed;
		private long _autoIncrementStep = 1;
		private DataSetDateTime _datetimeMode = DataSetDateTime.UnspecifiedLocal;
		private long _nextAutoIncrementValue;
		private string _caption;
		private string _columnName;
		private MappingType _columnMapping;
		private object _defaultValue;
		private int _maxLength;
		private int _ordinal;
		private bool _readOnly;
		private DataTable _table;
		private DataContainer _dataContainer;
		#endregion

		#region Constructors

		public DataColumn ()
			: this (String.Empty, typeof (string), String.Empty, MappingType.Element)
		{
		}

		//TODO: Ctor init vars directly
		public DataColumn (string columnName)
			: this (columnName, typeof (string), String.Empty, MappingType.Element)
		{
		}

		public DataColumn (string columnName, Type dataType)
			: this (columnName, dataType, String.Empty, MappingType.Element)
		{
		}

		public DataColumn (string columnName, Type dataType, string expr)
			: this (columnName, dataType, expr, MappingType.Element)
		{
		}

		public DataColumn (string columnName, Type dataType, string expr, MappingType type)
		{
			ColumnName = columnName == null ? String.Empty : columnName;

			if (dataType == null)
				throw new ArgumentNullException ("dataType");

			DataType = dataType;
			/*
			// FIXME: include expression support
			Expression = expr == null ? String.Empty : expr;
			*/
			ColumnMapping = type;
		}
		#endregion

		#region public instance methods
		public void SetOrdinal (int ordinal)
		{
			if (_ordinal == -1)
				throw new ArgumentException ("Column must belong to a table.");
			_table.Columns.MoveColumn (_ordinal, ordinal);
			_ordinal = ordinal;
		}
		#endregion

		#region protected instance methods
		protected internal virtual void
		OnPropertyChanging (PropertyChangedEventArgs pcevent)
		{
			PropertyChangedEventHandler eh = _eventHandlers [_propertyChangedKey] as PropertyChangedEventHandler;

			if (eh != null)
				eh (this, pcevent);
		}
		
		protected internal void RaisePropertyChanging (string name)
		{
			OnPropertyChanging (new PropertyChangedEventArgs (name));
		}
		
		void SetDefaultValue (object value, bool forcedTypeCheck)
		{
			if (forcedTypeCheck || !this._defaultValue.Equals (value)) {
				if (value == null || value == DBNull.Value)
					_defaultValue = GetDefaultValueForType (DataType);
				else if (DataType.IsInstanceOfType (value))
					_defaultValue = value;
				else
					try {
						_defaultValue = Convert.ChangeType (value, DataType);
					} catch (InvalidCastException) {
						string msg = String.Format ("Default Value of type '{0}' is not compatible with column type '{1}'", value.GetType (), DataType);
						throw new DataException (msg);
					}
			}

			// store default value in the table if already belongs to
			if (Table != null && Table.DefaultValuesRowIndex != -1)
				DataContainer [Table.DefaultValuesRowIndex] = _defaultValue;
		}
		
		/*
		/// <summary>
		///     Returns the data relation, which contains this column.
		///     This searches in current table's parent relations.
		/// <summary>
		/// <returns>
		///     DataRelation if found otherwise null.
		/// </returns>
		private DataRelation GetParentRelation ()
		{
			if (_table == null)
				return null;
			foreach (DataRelation rel in _table.ParentRelations)
				if (rel.Contains (this))
					return rel;
			return null;
		}
		*/
		#endregion

		#region internal class methods
		internal static object GetDefaultValueForType (Type type)
		{
			if (type == null)
				return DBNull.Value;
			/*
			if (type.Namespace == "System.Data.SqlTypes" && type.Assembly == typeof (DataColumn).Assembly) {
				// For SqlXxx types, set SqlXxx.Null instead of DBNull.Value.
				if (type == typeof (SqlBinary))
					return SqlBinary.Null;
				if (type == typeof (SqlBoolean))
					return SqlBoolean.Null;
				if (type == typeof (SqlByte))
					return SqlByte.Null;
				if (type == typeof (SqlBytes))
					return SqlBytes.Null;
				if (type == typeof (SqlChars))
					return SqlChars.Null;
				if (type == typeof (SqlDateTime))
					return SqlDateTime.Null;
				if (type == typeof (SqlDecimal))
					return SqlDecimal.Null;
				if (type == typeof (SqlDouble))
					return SqlDouble.Null;
				if (type == typeof (SqlGuid))
					return SqlGuid.Null;
				if (type == typeof (SqlInt16))
					return SqlInt16.Null;
				if (type == typeof (SqlInt32))
					return SqlInt32.Null;
				if (type == typeof (SqlInt64))
					return SqlInt64.Null;
				if (type == typeof (SqlMoney))
					return SqlMoney.Null;
				if (type == typeof (SqlSingle))
					return SqlSingle.Null;
				if (type == typeof (SqlString))
					return SqlString.Null;
				if (type == typeof (SqlXml))
					return SqlXml.Null;
			}
			*/
			return DBNull.Value;
		}
		
		internal void UpdateAutoIncrementValue (long value64)
		{
			if (_autoIncrementStep > 0) {
				if (value64 >= _nextAutoIncrementValue) {
					_nextAutoIncrementValue = value64;
					AutoIncrementValue ();
				}
			} else if (value64 <= _nextAutoIncrementValue) {
				_nextAutoIncrementValue = value64;
				AutoIncrementValue ();
			}
		}

		internal long AutoIncrementValue ()
		{
			long currentValue = _nextAutoIncrementValue;
			_nextAutoIncrementValue += AutoIncrementStep;
			return currentValue;
		}

		internal long GetAutoIncrementValue ()
		{
			return _nextAutoIncrementValue;
		}
		
		internal void SetDefaultValue (int index)
		{
			if (AutoIncrement)
				this [index] = _nextAutoIncrementValue;
			else
				DataContainer.CopyValue (Table.DefaultValuesRowIndex, index);
		}

		internal void SetTable (DataTable table)
		{
			if(_table != null)
				throw new ArgumentException ("The column already belongs to a different table");

			_table = table;

			/*
			// this will get called by DataTable
			// and DataColumnCollection
			if (_unique) {
				// if the DataColumn is marked as Unique and then
				// added to a DataTable , then a UniqueConstraint
				// should be created
				UniqueConstraint uc = new UniqueConstraint (this);
				_table.Constraints.Add (uc);
			}
			*/

			// allocate space in the column data container
			DataContainer.Capacity = _table.RecordCache.CurrentCapacity;

			int defaultValuesRowIndex = _table.DefaultValuesRowIndex;
			if (defaultValuesRowIndex != -1) {
				// store default value in the table
				DataContainer [defaultValuesRowIndex] = _defaultValue;
				// Set all the values in data container to default
				// it's cheaper that raise event on each row.
				DataContainer.FillValues (defaultValuesRowIndex);
			}
		}
		#endregion
	}
}
