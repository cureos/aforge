//
// System.Data/DataSet.cs
//
// Author:
//   Christopher Podurgiel <cpodurgiel@msn.com>
//   Daniel Morgan <danmorg@sc.rr.com>
//   Rodrigo Moya <rodrigo@ximian.com>
//   Stuart Caborn <stuart.caborn@virgin.net>
//   Tim Coleman (tim@timcoleman.com)
//   Ville Palo <vi64pa@koti.soon.fi>
//   Atsushi Enomoto <atsushi@ximian.com>
//   Konstantin Triger <kostat@mainsoft.com>
//
// (C) Ximian, Inc. 2002
// Copyright (C) Tim Coleman, 2002, 2003
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
using System.Globalization;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Common;

namespace System.Data
{
	//[DefaultProperty ("DataSetName")]
	[Serializable]
	public partial class DataSet : /*MarshalByValueComponent, */ISupportInitialize /*, IListSource, ISerializable*/{
		private string dataSetName;
		private string _namespace = string.Empty;
		private string prefix;
		private bool caseSensitive;
		private bool enforceConstraints = true;
		private DataTableCollection tableCollection;
		private DataRelationCollection relationCollection;
		private PropertyCollection properties;
		//FIXME: private DataViewManager defaultView;
		private CultureInfo locale;
		//FIXME: internal TableAdapterSchemaInfo tableAdapterSchemaInfo;
		bool initInProgress;
		#region Constructors

		public DataSet ()
			: this ("NewDataSet")
		{
		}

		public DataSet (string dataSetName)
		{
			this.dataSetName = dataSetName;
			tableCollection = new DataTableCollection (this);
			relationCollection = new DataRelationCollection.DataSetRelationCollection (this);
			properties = new PropertyCollection ();
			prefix = String.Empty;
		}
		#endregion // Constructors

		#region Public Properties

		[DataCategory ("Data")]
		[DefaultValue (false)]
		public bool CaseSensitive {
			get { return caseSensitive; }
			set {
				caseSensitive = value;
				if (!caseSensitive) {
					foreach (DataTable table in Tables) {
						table.ResetCaseSensitiveIndexes ();
						foreach (Constraint c in table.Constraints)
							c.AssertConstraint ();
					}
				} else {
					foreach (DataTable table in Tables) {
						table.ResetCaseSensitiveIndexes ();
					}
				}
			}
		}

		[DataCategory ("Data")]
		[DefaultValue ("")]
		public string DataSetName {
			get { return dataSetName; }
			set { dataSetName = value; }
		}
		/*
		[Browsable (false)]
		public DataViewManager DefaultViewManager {
			get {
				if (defaultView == null)
					defaultView = new DataViewManager (this);
				return defaultView;
			}
		}
		*/
		[DefaultValue (true)]
		public bool EnforceConstraints {
			get { return enforceConstraints; }
			set { InternalEnforceConstraints (value, true); }
		}

		[Browsable (false)]
		[DataCategory ("Data")]
		public PropertyCollection ExtendedProperties {
			get { return properties; }
		}

		[Browsable (false)]
		public bool HasErrors {
			get {
				for (int i = 0; i < Tables.Count; i++) {
					if (Tables[i].HasErrors)
						return true;
				}
				return false;
			}
		}

		[DataCategory ("Data")]
		public CultureInfo Locale {
			get { return locale != null ? locale : Thread.CurrentThread.CurrentCulture; }
			set {
				if (locale == null || !locale.Equals (value)) {
					// TODO: check if the new locale is valid
					// TODO: update locale of all tables
					locale = value;
				}
			}
		}

		internal bool LocaleSpecified {
			get { return locale != null; }
		}

		/*
		//FIXME: internal TableAdapterSchemaInfo TableAdapterSchemaData {
			get { return tableAdapterSchemaInfo; }
		}
		*/
		
		internal void InternalEnforceConstraints (bool value,bool resetIndexes)
		{
			if (value == enforceConstraints)
				return;

			if (value) {
				if (resetIndexes) {
					// FIXME : is that correct?
					// By design the indexes should be updated at this point.
					// In Fill from BeginLoadData till EndLoadData indexes are not updated (reset in EndLoadData)
					// In DataRow.EndEdit indexes are always updated.
					foreach (DataTable table in Tables)
						table.ResetIndexes ();
				}

				// TODO : Need to take care of Error handling and settting of RowErrors
				bool constraintViolated = false;
				foreach (DataTable table in Tables) {
					foreach (Constraint constraint in table.Constraints)
						constraint.AssertConstraint();
					table.AssertNotNullConstraints ();
					if (!constraintViolated && table.HasErrors)
						constraintViolated = true;
				}

				if (constraintViolated)
					Constraint.ThrowConstraintException ();
			}
			enforceConstraints = value;
		}

		public void Merge (DataRow[] rows)
		{
			Merge (rows, false, MissingSchemaAction.Add);
		}

		public void Merge (DataSet dataSet)
		{
			Merge (dataSet, false, MissingSchemaAction.Add);
		}

		public void Merge (DataTable table)
		{
			Merge (table, false, MissingSchemaAction.Add);
		}

		public void Merge (DataSet dataSet, bool preserveChanges)
		{
			Merge (dataSet, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge (DataRow[] rows, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (rows == null)
				throw new ArgumentNullException ("rows");
			if (!IsLegalSchemaAction (missingSchemaAction))
				throw new ArgumentOutOfRangeException ("missingSchemaAction");

			MergeManager.Merge (this, rows, preserveChanges, missingSchemaAction);
		}

		public void Merge (DataSet dataSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (dataSet == null)
				throw new ArgumentNullException ("dataSet");
			if (!IsLegalSchemaAction (missingSchemaAction))
				throw new ArgumentOutOfRangeException ("missingSchemaAction");

			MergeManager.Merge (this, dataSet, preserveChanges, missingSchemaAction);
		}

		public void Merge (DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (table == null)
				throw new ArgumentNullException ("table");
			if (!IsLegalSchemaAction (missingSchemaAction))
				throw new ArgumentOutOfRangeException ("missingSchemaAction");

			MergeManager.Merge (this, table, preserveChanges, missingSchemaAction);
		}

		private static bool IsLegalSchemaAction (MissingSchemaAction missingSchemaAction)
		{
			if (missingSchemaAction == MissingSchemaAction.Add || missingSchemaAction == MissingSchemaAction.AddWithKey
				|| missingSchemaAction == MissingSchemaAction.Error || missingSchemaAction == MissingSchemaAction.Ignore)
				return true;
			return false;
		}

		[DataCategory ("Data")]
		[DefaultValue ("")]
		public string Namespace {
			get { return _namespace; }
			set {
				//TODO - trigger an event if this happens?
				if (value == null)
					value = String.Empty;
				 if (value != this._namespace)
					RaisePropertyChanging ("Namespace");
				_namespace = value;
			}
		}

		[DataCategory ("Data")]
		[DefaultValue ("")]
		public string Prefix {
			get { return prefix; }
			set {
				if (value == null)
					value = String.Empty;
				// Prefix cannot contain any special characters other than '_' and ':'
				for (int i = 0; i < value.Length; i++) {
					if (!(Char.IsLetterOrDigit (value [i])) && (value [i] != '_') && (value [i] != ':'))
						throw new DataException ("Prefix '" + value + "' is not valid, because it contains special characters.");
				}

				if (value != this.prefix)
					RaisePropertyChanging ("Prefix");
				prefix = value;
			}
		}

		[DataCategory ("Data")]
		//[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		public DataRelationCollection Relations {
			get { return relationCollection; }
		}
		/*
		[Browsable (false)]
		//[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get { return base.Site; }
			set { base.Site = value; }
		}
		*/
		[DataCategory ("Data")]
		//[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		public DataTableCollection Tables {
			get { return tableCollection; }
		}

		#endregion // Public Properties

		#region Public Methods

		public void AcceptChanges ()
		{
			foreach (DataTable tempTable in tableCollection)
				tempTable.AcceptChanges ();
		}

		/// <summary>
		/// Clears all the tables
		/// </summary>
		public void Clear ()
		{
			bool enforceConstraints = this.EnforceConstraints;
			this.EnforceConstraints = false;
			for (int t = 0; t < tableCollection.Count; t++)
				tableCollection[t].Clear ();
			this.EnforceConstraints = enforceConstraints;
		}

		public virtual DataSet Clone ()
		{
			// need to return the same type as this...
			DataSet Copy = (DataSet) Activator.CreateInstance (GetType (), true);

			CopyProperties (Copy);

			foreach (DataTable Table in Tables) {
				// tables are often added in no-args constructor, don't add them
				// twice.
				if (!Copy.Tables.Contains (Table.TableName))
					Copy.Tables.Add (Table.Clone ());
			}

			//Copy Relationships between tables after existance of tables
			//and setting properties correctly
			CopyRelations (Copy);

			return Copy;
		}

		// Copies both the structure and data for this DataSet.
		public DataSet Copy ()
		{
			// need to return the same type as this...
			DataSet Copy = (DataSet) Activator.CreateInstance (GetType (), true);

			CopyProperties (Copy);

			// Copy DatSet's tables
			foreach (DataTable Table in Tables) {
				if (! Copy.Tables.Contains (Table.TableName)) {
					Copy.Tables.Add (Table.Copy ());
					continue;
				}
				foreach (DataRow row in Table.Rows)
					Copy.Tables [Table.TableName].ImportRow (row);
			}

			//Copy Relationships between tables after existance of tables
			//and setting properties correctly
			CopyRelations (Copy);

			return Copy;
		}

		private void CopyProperties (DataSet Copy)
		{
			Copy.CaseSensitive = CaseSensitive;
			//Copy.Container = Container
			Copy.DataSetName = DataSetName;
			//Copy.DefaultViewManager
			//Copy.DesignMode
			Copy.EnforceConstraints = EnforceConstraints;
			if(ExtendedProperties.Count > 0) {
				// Cannot copy extended properties directly as the property does not have a set accessor
				Array tgtArray = Array.CreateInstance( typeof (object), ExtendedProperties.Count);
				ExtendedProperties.Keys.CopyTo (tgtArray, 0);
				for (int i = 0; i < ExtendedProperties.Count; i++)
					Copy.ExtendedProperties.Add (tgtArray.GetValue (i), ExtendedProperties[tgtArray.GetValue (i)]);
			}
			Copy.locale = locale;
			Copy.Namespace = Namespace;
			Copy.Prefix = Prefix;
			//Copy.Site = Site; // FIXME : Not sure of this.
		}


		private void CopyRelations (DataSet Copy)
		{

			//Creation of the relation contains some of the properties, and the constructor
			//demands these values. instead changing the DataRelation constructor and behaviour the
			//parameters are pre-configured and sent to the most general constructor

			foreach (DataRelation MyRelation in this.Relations) {

				// typed datasets create relations through ctor.
				if (Copy.Relations.Contains (MyRelation.RelationName))
					continue;

				string pTable = MyRelation.ParentTable.TableName;
				string cTable = MyRelation.ChildTable.TableName;
				DataColumn[] P_DC = new DataColumn[MyRelation.ParentColumns.Length];
				DataColumn[] C_DC = new DataColumn[MyRelation.ChildColumns.Length];
				int i = 0;

				foreach (DataColumn DC in MyRelation.ParentColumns) {
					P_DC[i]=Copy.Tables[pTable].Columns[DC.ColumnName];
					i++;
				}

				i = 0;

				foreach (DataColumn DC in MyRelation.ChildColumns) {
					C_DC[i]=Copy.Tables[cTable].Columns[DC.ColumnName];
					i++;
				}

				DataRelation cRel = new DataRelation (MyRelation.RelationName, P_DC, C_DC, false);
				Copy.Relations.Add (cRel);
			}

			// Foreign Key constraints are not cloned in DataTable.Clone
			// so, these constraints should be cloned when copying the relations.
			foreach (DataTable table in this.Tables) {
				foreach (Constraint c in table.Constraints) {
					if (!(c is ForeignKeyConstraint)
						|| Copy.Tables[table.TableName].Constraints.Contains (c.ConstraintName))
						continue;
					ForeignKeyConstraint fc = (ForeignKeyConstraint)c;
					DataTable parentTable = Copy.Tables [fc.RelatedTable.TableName];
					DataTable currTable = Copy.Tables [table.TableName];
					DataColumn[] parentCols = new DataColumn [fc.RelatedColumns.Length];
					DataColumn[] childCols = new DataColumn [fc.Columns.Length];
					for (int j=0; j < parentCols.Length; ++j)
						parentCols [j] = parentTable.Columns[fc.RelatedColumns[j].ColumnName];
					for (int j=0; j < childCols.Length; ++j)
						childCols [j] = currTable.Columns[fc.Columns[j].ColumnName];
					currTable.Constraints.Add (fc.ConstraintName, parentCols, childCols);
				}
			}
		}

		public DataSet GetChanges ()
		{
			return GetChanges (DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}


		public DataSet GetChanges (DataRowState rowStates)
		{
			if (!HasChanges (rowStates))
				return null;

			DataSet copySet = Clone ();
			bool prev = copySet.EnforceConstraints;
			copySet.EnforceConstraints = false;

			Hashtable addedRows = new Hashtable ();

			for (int i = 0; i < Tables.Count; i++) {
				DataTable origTable = Tables [i];
				DataTable copyTable = copySet.Tables[origTable.TableName];
				for (int j = 0; j < origTable.Rows.Count; j++) {
					DataRow row = origTable.Rows [j];
					if (!row.IsRowChanged (rowStates) || addedRows.Contains (row))
						continue;
					AddChangedRow (addedRows, copyTable, row);
				}
			}
			copySet.EnforceConstraints = prev;
			return copySet;
		}

		private void AddChangedRow (Hashtable addedRows, DataTable copyTable, DataRow row)
		{
			if (addedRows.ContainsKey (row))
				return;

			foreach (DataRelation relation in row.Table.ParentRelations) {
				DataRow parent = ( row.RowState != DataRowState.Deleted ?
						   row.GetParentRow (relation) :
						   row.GetParentRow (relation, DataRowVersion.Original)
						   );
				if (parent == null)
					continue;
				// add the parent row
				DataTable parentCopyTable = copyTable.DataSet.Tables [parent.Table.TableName];
				AddChangedRow (addedRows, parentCopyTable, parent);
			}

			// add the current row
			DataRow newRow = copyTable.NewNotInitializedRow ();
			copyTable.Rows.AddInternal (newRow);
			// Don't check for ReadOnly, when cloning data to new uninitialized row.
			row.CopyValuesToRow (newRow, false);
			newRow.XmlRowID = row.XmlRowID;
			addedRows.Add (row, row);
		}

		public bool HasChanges ()
		{
			return HasChanges (DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public bool HasChanges (DataRowState rowStates)
		{
			if (((int) rowStates & 0xffffffe0) != 0)
				throw new ArgumentOutOfRangeException ("rowStates");

			DataTableCollection tableCollection = Tables;
			DataTable table;
			DataRowCollection rowCollection;
			DataRow row;

			for (int i = 0; i < tableCollection.Count; i++) {
				table = tableCollection [i];
				rowCollection = table.Rows;
				for (int j = 0; j < rowCollection.Count; j++) {
					row = rowCollection [j];
					if ((row.RowState & rowStates) != 0)
						return true;
				}
			}

			return false;
		}

		public virtual void RejectChanges ()
		{
			int i;
			bool oldEnforceConstraints = this.EnforceConstraints;
			this.EnforceConstraints = false;

			for (i = 0; i < this.Tables.Count;i++)
				this.Tables[i].RejectChanges ();

			this.EnforceConstraints = oldEnforceConstraints;
		}

		public virtual void Reset ()
		{
			// first we remove all ForeignKeyConstraints (if we will not do that
			// we will get an exception when clearing the tables).
			for (int i = 0; i < Tables.Count; i++) {
				ConstraintCollection cc = Tables[i].Constraints;
				for (int j = 0; j < cc.Count; j++) {
					if (cc[j] is ForeignKeyConstraint)
						cc.Remove (cc[j]);
				}
			}

			Clear ();
			Relations.Clear ();
			Tables.Clear ();
		}
		#endregion // Public Methods

		#region Public Events

		[DataCategory ("Action")]
		public event MergeFailedEventHandler MergeFailed;

		#endregion // Public Events

		#region IListSource methods
		/*
		DataViewManager IList IListSource.GetList ()
		{
			return DefaultViewManager;
		}
		*/
		bool /*IListSource.*/ContainsListCollection {
			get {
				return true;
			}
		}
		#endregion IListSource methods

		#region ISupportInitialize methods

		internal bool InitInProgress {
			get { return initInProgress; }
			set { initInProgress = value; }
		}

		public void BeginInit ()
		{
			InitInProgress = true;
			dataSetInitialized = false;
		}

		public void EndInit ()
		{
			// Finsh the init'ing the tables only after adding all the
			// tables to the collection.
			Tables.PostAddRange ();
			for (int i=0; i < Tables.Count; ++i) {
				if (!Tables [i].InitInProgress)
					continue;
				Tables [i].FinishInit ();
			}

			Relations.PostAddRange ();
			InitInProgress = false;
			dataSetInitialized = true;
			DataSetInitialized ();
		}
		#endregion

		#region Protected Methods
		protected virtual bool ShouldSerializeRelations ()
		{
			return true;
		}

		protected virtual bool ShouldSerializeTables ()
		{
			return true;
		}

		[MonoTODO]
		protected internal virtual void OnPropertyChanging (PropertyChangedEventArgs pcevent)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		protected virtual void OnRemoveRelation (DataRelation relation)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO]
		protected virtual void OnRemoveTable (DataTable table)
		{
			throw new NotImplementedException ();
		}

		internal virtual void OnMergeFailed (MergeFailedEventArgs e)
		{
			if (MergeFailed != null)
				MergeFailed (this, e);
			else
				throw new DataException (e.Conflict);
		}

		[MonoTODO]
		protected internal void RaisePropertyChanging (string name)
		{
		}

		#endregion

		#region Private Methods
		///<summary>
		/// Helper function to split columns into attributes elements and simple
		/// content
		/// </summary>
		internal static void SplitColumns (DataTable table,
			out ArrayList atts,
			out ArrayList elements,
			out DataColumn simple)
		{
			//The columns can be attributes, hidden, elements, or simple content
			//There can be 0-1 simple content cols or 0-* elements
			atts = new System.Collections.ArrayList ();
			elements = new System.Collections.ArrayList ();
			simple = null;

			//Sort out the columns
			foreach (DataColumn col in table.Columns) {
				switch (col.ColumnMapping) {
					case MappingType.Attribute:
						atts.Add (col);
						break;
					case MappingType.Element:
						elements.Add (col);
						break;
					case MappingType.SimpleContent:
						if (simple != null) {
							throw new System.InvalidOperationException ("There may only be one simple content element");
						}
						simple = col;
						break;
					default:
						//ignore Hidden elements
						break;
				}
			}
		}

		private void SetRowsID ()
		{
			foreach (DataTable table in Tables)
				table.SetRowsID ();
		}

		#endregion //Private Xml Serialisation
	}

	partial class DataSet/* : ISupportInitializeNotification */{
		private bool dataSetInitialized = true;
		public event EventHandler Initialized;

		SerializationFormat remotingFormat = SerializationFormat.Xml;
		[DefaultValue (SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat {
			get { return remotingFormat; }
			set { remotingFormat = value; }
		}

		[Browsable (false)]
		public bool IsInitialized {
			get { return dataSetInitialized; }
		}

		//[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		public virtual SchemaSerializationMode SchemaSerializationMode {
			get { return SchemaSerializationMode.IncludeSchema; }
			set {
				if (value != SchemaSerializationMode.IncludeSchema)
					throw new InvalidOperationException (
							"Only IncludeSchema Mode can be set for Untyped DataSet");
			}
		}

		/*
		//FIXME: public DataTableReader CreateDataReader (params DataTable[] dataTables)
		{
			return new DataTableReader (dataTables);
		}

		public DataTableReader CreateDataReader ()
		{
			return new DataTableReader ((DataTable[])Tables.ToArray (typeof (DataTable)));
		}

		public void Load (IDataReader reader, LoadOption loadOption, params DataTable[] tables)
		{
			if (reader == null)
				throw new ArgumentNullException ("Value cannot be null. Parameter name: reader");

			foreach (DataTable dt in tables) {
				if (dt.DataSet == null || dt.DataSet != this)
					throw new ArgumentException ("Table " + dt.TableName + " does not belong to this DataSet.");
				dt.Load (reader, loadOption);
				reader.NextResult ();
			}
		}

		public void Load (IDataReader reader, LoadOption loadOption, params string[] tables)
		{
			if (reader == null)
				throw new ArgumentNullException ("Value cannot be null. Parameter name: reader");

			foreach (string tableName in tables) {
				DataTable dt = Tables [tableName];

				if (dt == null) {
					dt = new DataTable (tableName);
					Tables.Add (dt);
				}
				dt.Load (reader, loadOption);
				reader.NextResult ();
			}
		}

		public virtual void Load (IDataReader reader, LoadOption loadOption,
					  FillErrorEventHandler errorHandler, params DataTable[] tables)
		{
			if (reader == null)
				throw new ArgumentNullException ("Value cannot be null. Parameter name: reader");

			foreach (DataTable dt in tables) {
				if (dt.DataSet == null || dt.DataSet != this)
					throw new ArgumentException ("Table " + dt.TableName + " does not belong to this DataSet.");
				dt.Load (reader, loadOption, errorHandler);
				reader.NextResult ();
			}
		}

		void BinaryDeserialize (SerializationInfo info)
		{
			ArrayList arrayList = null;

			DataSetName = info.GetString ("DataSet.DataSetName");
			Namespace = info.GetString ("DataSet.Namespace");
			CaseSensitive = info.GetBoolean ("DataSet.CaseSensitive");
			Locale = new CultureInfo (info.GetInt32 ("DataSet.LocaleLCID"));
			EnforceConstraints = info.GetBoolean ("DataSet.EnforceConstraints");
			Prefix = info.GetString ("DataSet.Prefix");

			//FIXME: Private variable available in SerializationInfo
			//this.RemotingVersion = (System.Version) info.GetValue("DataSet.RemotingVersion",
			//typeof(System.Version));

			properties = (PropertyCollection) info.GetValue ("DataSet.ExtendedProperties",
									 typeof (PropertyCollection));
			int tableCount = info.GetInt32 ("DataSet.Tables.Count");

			Byte [] bytes;
			DataTable dt = null;
			for (int i = 0; i < tableCount; i++) {
				bytes = (Byte []) info.GetValue ("DataSet.Tables_" + i,
								 typeof (Byte[]));
				MemoryStream ms = new MemoryStream (bytes);
				BinaryFormatter bf = new BinaryFormatter ();
				dt = (DataTable) bf.Deserialize (ms);
				ms.Close ();
				for (int j = 0; j < dt.Columns.Count; j++) {
					dt.Columns[j].Expression = info.GetString ("DataTable_" + i +
										   ".DataColumn_" + j +
										   ".Expression");
				}

				//Not using
				//int rowsCount = info.GetInt32 ("DataTable_" + i + ".Rows.Count");
				//int recordsCount = info.GetInt32 ("DataTable_" + i + ".Records.Count");

				ArrayList nullBits = (ArrayList) info.GetValue ("DataTable_" + i + ".NullBits",
										typeof (ArrayList));
				arrayList = (ArrayList) info.GetValue ("DataTable_" + i + ".Records",
								       typeof (ArrayList));
				BitArray rowStateBitArray = (BitArray) info.GetValue ("DataTable_" + i + ".RowStates",
										      typeof (BitArray));
				dt.DeserializeRecords (arrayList, nullBits, rowStateBitArray);
				Tables.Add (dt);
			}
			for (int i = 0; i < tableCount; i++) {
				dt = Tables [i];
				dt.dataSet = this;
				arrayList = (ArrayList) info.GetValue ("DataTable_" + i + ".Constraints",
								       typeof (ArrayList));
				if (dt.Constraints == null)
					dt.Constraints = new ConstraintCollection (dt);
				dt.DeserializeConstraints (arrayList);
			}
			arrayList = (ArrayList) info.GetValue ("DataSet.Relations",
							       typeof (ArrayList));
			bool bParentColumn = true;
			for (int l = 0; l < arrayList.Count; l++) {
				ArrayList tmpArrayList = (ArrayList) arrayList[l];
				ArrayList childColumns = new ArrayList ();
				ArrayList parentColumns = new ArrayList ();
				for (int k = 0; k < tmpArrayList.Count; k++) {
					if (tmpArrayList[k] != null && typeof (int) == tmpArrayList[k].GetType().GetElementType()) {
						Array dataColumnArray = (Array)tmpArrayList[k];
						if (bParentColumn) {
							parentColumns.Add (Tables [(int) dataColumnArray.GetValue (0)].
									   Columns [(int) dataColumnArray.GetValue (1)]);
							bParentColumn = false;
						}
						else {
							childColumns.Add (Tables [(int) dataColumnArray.GetValue (0)].
									  Columns [(int) dataColumnArray.GetValue (1)]);
							bParentColumn = true;
						}
					}
				}
				Relations.Add ((string) tmpArrayList [0],
					       (DataColumn []) parentColumns.ToArray (typeof (DataColumn)),
					       (DataColumn []) childColumns.ToArray (typeof (DataColumn)),
					       false);
			}
		}
		*/
		private void OnDataSetInitialized (EventArgs e)
		{
			if (null != Initialized)
				Initialized (this, e);
		}

		private void DataSetInitialized ()
		{
			EventArgs e = new EventArgs ();
			OnDataSetInitialized (e);
		}

		protected virtual void InitializeDataSet ()
		{
		}
		/*
		protected SchemaSerializationMode DetermineSchemaSerializationMode (SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator e = info.GetEnumerator ();
			while (e.MoveNext ()) {
				if (e.Name == "SchemaSerializationMode.DataSet") {
					return (SchemaSerializationMode) e.Value;
				}
			}
			
			return SchemaSerializationMode.IncludeSchema;
		}

		protected bool IsBinarySerialized (SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator e = info.GetEnumerator ();
			while (e.MoveNext ()) {
				if (e.ObjectType == typeof (System.Data.SerializationFormat))
					return true;
			}
			return false;
		}
		*/
	}
}
