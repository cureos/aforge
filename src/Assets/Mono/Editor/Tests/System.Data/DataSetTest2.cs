// Authors:
//   Rafael Mizrahi   <rafim@mainsoft.com>
//   Erez Lotan       <erezl@mainsoft.com>
//   Oren Gurfinkel   <oreng@mainsoft.com>
//   Ofer Borstein
//   Veerapuram Varadhan  <vvaradhan@novell.com>
//
// Copyright (c) 2004 Mainsoft Co.
// Copyright (c) 2009 Novell Inc.
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

using NUnit.Framework;
using System;
using System.Text;
using System.IO;
using System.Data;
using MonoTests.System.Data.Utils;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace MonoTests.System.Data
{
	[TestFixture]
	public class DataSetTest2
	{
		private DataSet m_ds = null;
		private bool EventRaised = false;

		[Test] public void AcceptChanges()
		{
			DataSet ds = new DataSet();
			DataTable dtP = DataProvider.CreateParentDataTable();
			DataTable dtC = DataProvider.CreateChildDataTable();
			ds.Tables.Add(dtP);
			ds.Tables.Add(dtC);
			ds.Relations.Add(new DataRelation("myRelation",dtP.Columns[0],dtC.Columns[0]));

			//create changes
			dtP.Rows[0][0] = "70";
			dtP.Rows[1].Delete();
			dtP.Rows.Add(new object[] {9,"string1","string2"});

			// AcceptChanges
			ds.AcceptChanges();
			Assert.AreEqual(null, dtP.GetChanges(), "DS1");

			//read only exception
			dtP.Columns[0].ReadOnly = true;
			// check ReadOnlyException
			try
			{
				dtP.Rows[0][0] = 99;
				Assert.Fail("DS2: Indexer Failed to throw ReadOnlyException");
			}
			catch (ReadOnlyException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DS3: Indexer. Wrong exception type. Got:" + exc);
			}

			// check invoke AcceptChanges
			ds.AcceptChanges();
		}

		[Test] public void CaseSensitive()
		{
			DataSet ds = new DataSet();
			DataTable dt = new DataTable();

			// CaseSensitive - default value (false)
			Assert.AreEqual(false , ds.CaseSensitive  , "DS4");

			ds.CaseSensitive = true;

			// CaseSensitive - get
			Assert.AreEqual(true , ds.CaseSensitive  , "DS5");

			//add a datatable to a dataset
			ds.Tables.Add(dt);
			// DataTable CaseSensitive from DataSet - true
			Assert.AreEqual(true , dt.CaseSensitive  , "DS6");

			ds.Tables.Clear();
			ds.CaseSensitive = false;
			dt = new DataTable();
			ds.Tables.Add(dt);

			// DataTable CaseSensitive from DataSet - false
			Assert.AreEqual(false, dt.CaseSensitive , "DS7");

			//change DataSet CaseSensitive and check DataTables in it
			ds.Tables.Clear();
			ds.CaseSensitive = false;
			dt = new DataTable();
			ds.Tables.Add(dt);

			// Change DataSet CaseSensitive - check Table - true
			ds.CaseSensitive = true;
			Assert.AreEqual(true, dt.CaseSensitive , "DS8");

			// Change DataSet CaseSensitive - check Table - false
			ds.CaseSensitive = false;
			Assert.AreEqual(false, dt.CaseSensitive , "DS9");

			//Add new table to DataSet with CaseSensitive,check the table case after adding it to DataSet
			ds.Tables.Clear();
			ds.CaseSensitive = true;
			dt = new DataTable();
			dt.CaseSensitive = false;
			ds.Tables.Add(dt);

			// DataTable get case sensitive from DataSet - false
			Assert.AreEqual(false, dt.CaseSensitive , "DS10");

			ds.Tables.Clear();
			ds.CaseSensitive = false;
			dt = new DataTable();
			dt.CaseSensitive = true;
			ds.Tables.Add(dt);

			// DataTable get case sensitive from DataSet - true
			Assert.AreEqual(true, dt.CaseSensitive , "DS11");

			//Add new table to DataSet and change the DataTable CaseSensitive
			ds.Tables.Clear();
			ds.CaseSensitive = true;
			dt = new DataTable();
			ds.Tables.Add(dt);

			// Add new table to DataSet and change the DataTable CaseSensitive - false
			dt.CaseSensitive = false;
			Assert.AreEqual(false, dt.CaseSensitive , "DS12");

			ds.Tables.Clear();
			ds.CaseSensitive = false;
			dt = new DataTable();
			ds.Tables.Add(dt);

			// Add new table to DataSet and change the DataTable CaseSensitive - true
			dt.CaseSensitive = true;
			Assert.AreEqual(true, dt.CaseSensitive , "DS13");

			//Add DataTable to Dataset, Change DataSet CaseSensitive, check DataTable
			ds.Tables.Clear();
			ds.CaseSensitive = true;
			dt = new DataTable();
			dt.CaseSensitive = true;
			ds.Tables.Add(dt);

			// Add DataTable to Dataset, Change DataSet CaseSensitive, check DataTable - true
			ds.CaseSensitive = false;
			Assert.AreEqual(true, dt.CaseSensitive , "DS14");
		}

		[Test] public void Clear()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(DataProvider.CreateParentDataTable());
			ds.Tables[0].Rows.Add(new object[] {9,"",""});

			// Clear
			ds.Clear();
			Assert.AreEqual(0, ds.Tables[0].Rows.Count , "DS15");
		}

		[Test] public void Clear_WithNoDataWithConstraint()
		{
			// Test dataset with no data and with constraint
			DataSet ds = new DataSet();
			ds.Tables.Add(DataProvider.CreateParentDataTable());
			ds.Tables.Add(DataProvider.CreateChildDataTable());
			ds.Tables[0].Rows.Clear();
			ds.Tables[1].Rows.Clear();

			ds.Tables[0].Constraints.Add("test",ds.Tables[1].Columns[0],ds.Tables[0].Columns[0]);
			ds.Clear();
		}

		[Test] public void Clone()
		{
			DataSet ds = new DataSet(), dsTarget = null;
			ds.Tables.Add(DataProvider.CreateParentDataTable());
			ds.Tables.Add(DataProvider.CreateChildDataTable());
			ds.Relations.Add(new DataRelation("myRelation",ds.Tables[0].Columns[0],ds.Tables[1].Columns[0]));
			ds.Tables[0].Rows.Add(new object[] {9,"",""});
			ds.Tables[1].Columns[2].ReadOnly = true;
			ds.Tables[0].PrimaryKey = new DataColumn[] {ds.Tables[0].Columns[0],ds.Tables[0].Columns[1]};

			//copy schema only, no data

			// Clone 1
			dsTarget = ds.Clone();
			//Assert.AreEqual(ds.GetXmlSchema(), dsTarget.GetXmlSchema() , "DS16");
			//use my function because GetXmlSchema not implemented in java
			Assert.AreEqual(DataProvider.GetDSSchema(ds), DataProvider.GetDSSchema(dsTarget), "DS17");
		}

		[Test] public void Copy()
		{
			DataSet ds = new DataSet(), dsTarget = null;
			ds.Tables.Add(DataProvider.CreateParentDataTable());
			ds.Tables.Add(DataProvider.CreateChildDataTable());
			ds.Relations.Add(new DataRelation("myRelation",ds.Tables[0].Columns[0],ds.Tables[1].Columns[0]));
			ds.Tables[0].Rows.Add(new object[] {9,"",""});
			ds.Tables[1].Columns[2].ReadOnly = true;
			ds.Tables[0].PrimaryKey = new DataColumn[] {ds.Tables[0].Columns[0],ds.Tables[0].Columns[1]};

			//copy data and schema

			// Copy 1
			dsTarget = ds.Copy();
			//Assert.AreEqual(ds.GetXmlSchema(), dsTarget.GetXmlSchema() , "DS19");
			//using my function because GetXmlSchema in not implemented in java
			Assert.AreEqual(DataProvider.GetDSSchema(ds), DataProvider.GetDSSchema (dsTarget) , "DS20");
		}

		[Test] public void DataSetName()
		{
			DataSet ds = new DataSet();

			// DataSetName - default value
			Assert.AreEqual("NewDataSet" , ds.DataSetName   , "DS22");

			ds.DataSetName = "NewName";

			// DataSetName - get
			Assert.AreEqual("NewName" , ds.DataSetName   , "DS23");
		}

		[Test] public void EnforceConstraints()
		{
			DataSet ds = new DataSet();

			// EnforceConstraints - default value (true)
			Assert.AreEqual(true, ds.EnforceConstraints , "DS24");

			ds.EnforceConstraints = false;

			// EnforceConstraints - get
			Assert.AreEqual(false, ds.EnforceConstraints  , "DS25");
		}

		[Test]
		public void EnforceConstraints_CheckPrimaryConstraint ()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add ("table");
			ds.Tables [0].Columns.Add ("col");
			ds.Tables [0].PrimaryKey = new DataColumn[] {ds.Tables [0].Columns [0]};
			ds.EnforceConstraints = false;
			ds.Tables [0].Rows.Add (new object[] {null});
			try {
				ds.EnforceConstraints = true;
				Assert.Fail ("#1");
			} catch (ConstraintException e) {
				// Never premise English.
				//Assert.AreEqual ("Failed to enable constraints. One or more rows contain values " + 
				//		"violating non-null, unique, or foreign-key constraints.", e.Message, "#2");
			}
		}

		[Test]
		public void EnforceConstraints_NonNullCols ()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add ("table");
			ds.Tables [0].Columns.Add ("col");
			ds.Tables [0].Columns [0].AllowDBNull = false;

			ds.EnforceConstraints = false;
			ds.Tables [0].Rows.Add (new object[] {null});
			try {
				ds.EnforceConstraints = true;
				Assert.Fail ("#1");
			} catch (ConstraintException e) {
				// Never premise English.
				//Assert.AreEqual ("Failed to enable constraints. One or more rows contain values " + 
				//		"violating non-null, unique, or foreign-key constraints.", e.Message, "#2");
			}
		}

		[Test] public void GetChanges()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(DataProvider.CreateParentDataTable());

			// GetChanges 1
			Assert.AreEqual(null , ds.GetChanges(), "DS26");

			DataRow dr = ds.Tables[0].NewRow();
			dr[0] = 9;
			ds.Tables[0].Rows.Add(dr);

			// GetChanges 2
			Assert.AreEqual(true , ds.GetChanges()!=null, "DS27");

			// GetChanges 3
			Assert.AreEqual(dr.ItemArray, ds.GetChanges().Tables[0].Rows[0].ItemArray  , "DS28");
		}

		[Test] public void GetChanges_ByDataRowState()
		{
			DataSet ds = new DataSet();
			object[] arrAdded,arrDeleted,arrModified,arrUnchanged;
			//object[] arrDetached;

			DataRow dr;
			ds.Tables.Add(DataProvider.CreateParentDataTable());

			// GetChanges 1
			Assert.AreEqual(null , ds.GetChanges(), "DS29");

			//make some changes

			// can't check detached
			//		dr = ds.Tables[0].Rows[0];
			//		arrDetached = dr.ItemArray;
			//		dr.Delete();
			//		ds.Tables[0].AcceptChanges();

			dr= ds.Tables[0].Rows[1];
			arrDeleted  = dr.ItemArray;
			dr.Delete();

			dr = ds.Tables[0].Rows[2];
			dr[1] = "NewValue";
			arrModified = dr.ItemArray;

			dr = ds.Tables[0].Select("","",DataViewRowState.Unchanged)[0];
			arrUnchanged = dr.ItemArray;

			dr = ds.Tables[0].NewRow();
			dr[0] = 1;
			ds.Tables[0].Rows.Add(dr);
			arrAdded = dr.ItemArray;

			// GetChanges Added
			Assert.AreEqual(arrAdded, ds.GetChanges(DataRowState.Added).Tables[0].Rows[0].ItemArray , "DS30");

			// GetChanges Deleted
			dr = ds.GetChanges(DataRowState.Deleted).Tables[0].Rows[0];
			object[] tmp = new object[] {dr[0,DataRowVersion.Original],dr[1,DataRowVersion.Original],dr[2,DataRowVersion.Original],dr[3,DataRowVersion.Original],dr[4,DataRowVersion.Original],dr[5,DataRowVersion.Original]};
			Assert.AreEqual(arrDeleted, tmp, "DS31");

			//	can't check it	
			//		// GetChanges Detached
			//		dr = ds.GetChanges(DataRowState.Detached).Tables[0].Rows[0];
			//		object[] tmp = new object[] {dr[0,DataRowVersion.Original],dr[1,DataRowVersion.Original],dr[2,DataRowVersion.Original]};
			//		Assert.AreEqual(arrDetached, tmp, "DS32");

			// GetChanges Modified
			Assert.AreEqual(arrModified, ds.GetChanges(DataRowState.Modified).Tables[0].Rows[0].ItemArray , "DS33");

			// GetChanges Unchanged
			Assert.AreEqual(arrUnchanged, ds.GetChanges(DataRowState.Unchanged).Tables[0].Rows[0].ItemArray , "DS34");
		}

		[Test] public void BeginInitTest ()
		{
			DataSet ds = new DataSet ();

			DataTable table1 = new DataTable ("table1");
			DataTable table2 = new DataTable ("table2");

			DataColumn col1 = new DataColumn ("col1", typeof (int));
			DataColumn col2 = new DataColumn ("col2", typeof (int));
			table1.Columns.Add (col1);
			table2.Columns.Add (col2);
			
			UniqueConstraint pkey = new UniqueConstraint ("pk", new string[] {"col1"}, true);
			ForeignKeyConstraint fkey = new ForeignKeyConstraint ("fk", "table1", new String[] {"col1"}, 
								new String[] {"col2"}, AcceptRejectRule.Cascade,
								Rule.Cascade, Rule.Cascade);
			DataRelation relation = new DataRelation ("rel", "table1", "table2", new String[] {"col1"},
								 new String[] {"col2"}, false);
			ds.BeginInit ();
			table1.BeginInit ();
			table2.BeginInit ();

			ds.Tables.AddRange (new DataTable[] {table1, table2});
			ds.Relations.AddRange (new DataRelation[] {relation});
			
			table1.Constraints.AddRange (new Constraint[] {pkey});
			table2.Constraints.AddRange (new Constraint[] {fkey});

			// The tables/relations shud not get added to the DataSet yet
			Assert.AreEqual (0, ds.Tables.Count, "#1");
			Assert.AreEqual (0, ds.Relations.Count, "#2");
			Assert.AreEqual (0, table1.Constraints.Count, "#3");
			Assert.AreEqual (0, table2.Constraints.Count, "#4");
			ds.EndInit ();

			Assert.AreEqual (2, ds.Tables.Count, "#5");
			Assert.AreEqual (1, ds.Relations.Count, "#6");
			Assert.AreEqual (1, ds.Tables [0].Constraints.Count, "#7");
			Assert.AreEqual (1, ds.Tables [1].Constraints.Count, "#8");

			// Table shud still be in BeginInit .. 
			DataColumn col3 = new DataColumn ("col2");
			UniqueConstraint uc = new UniqueConstraint ("uc", new string[] {"col2"}, false);

			table1.Columns.AddRange (new DataColumn[] {col3});
			table1.Constraints.AddRange (new Constraint[] {uc});

			Assert.AreEqual (1, table1.Columns.Count, "#9");
			Assert.AreEqual (1, table1.Constraints.Count, "#10");

			table1.EndInit ();
			Assert.AreEqual (2, table1.Columns.Count, "#11");
			Assert.AreEqual (2, table1.Columns.Count, "#12");
		}
		
		[Test] public void HasChanges()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(DataProvider.CreateParentDataTable());

			// HasChanges 1
			Assert.AreEqual(false , ds.HasChanges(), "DS36");

			DataRow dr = ds.Tables[0].NewRow();
			dr[0] = 9;
			ds.Tables[0].Rows.Add(dr);

			// HasChanges 2
			Assert.AreEqual(true , ds.HasChanges(), "DS37");
		}

		[Test] public void HasChanges_ByDataRowState()
		{
			DataSet ds = new DataSet();

			DataRow dr;
			ds.Tables.Add(DataProvider.CreateParentDataTable());

			// HasChanges 1
			Assert.AreEqual(false , ds.HasChanges(), "DS38");

			//make some changes

			dr= ds.Tables[0].Rows[1];
			dr.Delete();

			dr = ds.Tables[0].Rows[2];
			dr[1] = "NewValue";

			dr = ds.Tables[0].Select("","",DataViewRowState.Unchanged)[0];

			dr = ds.Tables[0].NewRow();
			dr[0] = 1;
			ds.Tables[0].Rows.Add(dr);

			// HasChanges Added
			Assert.AreEqual(true , ds.HasChanges(DataRowState.Added), "DS39");

			// HasChanges Deleted
			Assert.AreEqual(true  , ds.HasChanges(DataRowState.Deleted) , "DS40");

			// HasChanges Modified
			Assert.AreEqual(true, ds.HasChanges(DataRowState.Modified), "DS41");

			// HasChanges Unchanged
			Assert.AreEqual(true, ds.HasChanges(DataRowState.Unchanged), "DS42");
		}

		[Test] public void HasErrors()
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(DataProvider.CreateParentDataTable());

			// HasErrors - default
			Assert.AreEqual(false , ds.HasErrors  , "DS43");

			ds.Tables[0].Rows[0].RowError = "ErrDesc";

			// HasErrors
			Assert.AreEqual(true , ds.HasErrors , "DS44");
		}
		
		[Test] public void Locale()
		{
			DataSet ds = new DataSet("MyDataSet");
			global::System.Globalization.CultureInfo culInfo = global::System.Globalization.CultureInfo.CurrentCulture ;

			// Checking Locale default from system
			Assert.AreEqual(culInfo, ds.Locale  , "DS156");

			// Checking Locale get/set
			culInfo = new global::System.Globalization.CultureInfo("fr"); // = french
			ds.Locale = culInfo ;
			Assert.AreEqual(culInfo , ds.Locale , "DS157");
		}

		[Test]
		[SetCulture ("cs-CZ")]
		public void DataSetSpecificCulture ()
		{			
			var ds = new DataSet() ;
			ds.Locale = CultureInfo.GetCultureInfo (1033);
			var dt = ds.Tables.Add ("machine");
			dt.Locale = ds.Locale;
			Assert.AreSame (dt, ds.Tables["MACHINE"]);
		}		

		[Test] public void MergeFailed()
		{
			EventRaised = false;
			DataSet ds1,ds2;
			ds1 = new DataSet();
			ds1.Tables.Add(DataProvider.CreateParentDataTable());
			//add primary key to the FIRST column
			ds1.Tables[0].PrimaryKey = new DataColumn[] {ds1.Tables[0].Columns[0]};

			//create target dataset which is a copy of the source
			ds2 = ds1.Copy();
			//clear the data
			ds2.Clear();
			//add primary key to the SECOND columnn
			ds2.Tables[0].PrimaryKey = new DataColumn[] {ds2.Tables[0].Columns[1]};
			//add a new row that already exists in the source dataset
			//ds2.Tables[0].Rows.Add(ds1.Tables[0].Rows[0].ItemArray);

			//enforce constraints
			ds2.EnforceConstraints = true;
			ds1.EnforceConstraints = true;

			// Add MergeFailed event handler for the table.
			ds2.MergeFailed += new MergeFailedEventHandler( Merge_Failed );

			ds2.Merge(ds1); //will raise MergeFailed event

			// MergeFailed event
			Assert.AreEqual(true , EventRaised , "DS158");
		}
		private void Merge_Failed( object sender, MergeFailedEventArgs e )
		{
			EventRaised = true;
		}

		[Test] public void Merge_ByDataRowsNoPreserveIgnoreMissingSchema()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};

			//create target dataset (copy of source dataset)
			DataSet dsTarget = new DataSet();
			dsTarget.Tables.Add(dt.Copy());

			DataRow[] drArr = new DataRow[3];
			//Update row
			string OldValue = dt.Select("ParentId=1")[0][1].ToString();
			drArr[0] = dt.Select("ParentId=1")[0];
			drArr[0][1]	= "NewValue";
			//delete rows
			drArr[1] = dt.Select("ParentId=2")[0];
			drArr[1].Delete();
			//add row
			drArr[2] = dt.NewRow();
			drArr[2].ItemArray = new object[] {99 ,"NewRowValue1", "NewRowValue2"};
			dt.Rows.Add(drArr[2]);

			dsTarget.Merge(drArr,false,MissingSchemaAction.Ignore );

			// Merge update row
			Assert.AreEqual("NewValue", dsTarget.Tables["Table1"].Select("ParentId=1")[0][1] , "DS159");

			// Merge added row
			Assert.AreEqual(1, dsTarget.Tables["Table1"].Select("ParentId=99").Length , "DS160");

			// Merge deleted row
			Assert.AreEqual(0, dsTarget.Tables["Table1"].Select("ParentId=2").Length , "DS161");
		}

		[Test] public void Merge_ByDataRowsPreserveMissingSchema()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};

			//create target dataset (copy of source dataset)
			DataSet dsTarget = new DataSet();
			dsTarget.Tables.Add(dt.Copy());

			//add new column (for checking MissingSchemaAction)
			DataColumn dc = new DataColumn("NewColumn",typeof(float));
			dt.Columns.Add(dc);

			DataRow[] drArr = new DataRow[3];

			//Update row
			string OldValue = dt.Select("ParentId=1")[0][1].ToString();
			drArr[0] = dt.Select("ParentId=1")[0];
			drArr[0][1]	= "NewValue";
			//delete rows
			drArr[1] = dt.Select("ParentId=2")[0];
			drArr[1].Delete();
			//add row
			drArr[2] = dt.NewRow();
			drArr[2].ItemArray = new object[] {99 ,"NewRowValue1", "NewRowValue2",null};
			dt.Rows.Add(drArr[2]);

			DataSet dsTarget1 = null;

			#region "Merge(drArr,true,MissingSchemaAction.Ignore )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(drArr,true,MissingSchemaAction.Ignore );
			// Merge true,Ignore - Column
			Assert.AreEqual(false, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS162");

			// Merge true,Ignore - changed values
			Assert.AreEqual(OldValue, dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS163");

			// Merge true,Ignore - added values
			Assert.AreEqual(1 , dsTarget1.Tables["Table1"].Select("ParentId=99").Length  , "DS164");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Select("ParentId=2").Length > 0, "DS165");
			#endregion

			#region "Merge(drArr,false,MissingSchemaAction.Ignore )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(drArr,false,MissingSchemaAction.Ignore );
			// Merge true,Ignore - Column
			Assert.AreEqual(false, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS166");

			// Merge true,Ignore - changed values
			Assert.AreEqual("NewValue", dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS167");

			// Merge true,Ignore - added values
			Assert.AreEqual(1, dsTarget1.Tables["Table1"].Select("ParentId=99").Length , "DS168");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(0, dsTarget1.Tables["Table1"].Select("ParentId=2").Length , "DS169");
			#endregion

			#region "Merge(drArr,true,MissingSchemaAction.Add )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(drArr,true,MissingSchemaAction.Add  );
			// Merge true,Ignore - Column
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS170");

			// Merge true,Ignore - changed values
			Assert.AreEqual(OldValue, dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS171");

			// Merge true,Ignore - added values
			Assert.AreEqual(1, dsTarget1.Tables["Table1"].Select("ParentId=99").Length , "DS172");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Select("ParentId=2").Length >0, "DS173");
			#endregion

			#region "Merge(drArr,false,MissingSchemaAction.Add )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(drArr,false,MissingSchemaAction.Add  );
			// Merge true,Ignore - Column
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS174");

			// Merge true,Ignore - changed values
			Assert.AreEqual("NewValue", dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS175");

			// Merge true,Ignore - added values
			Assert.AreEqual(1, dsTarget1.Tables["Table1"].Select("ParentId=99").Length , "DS176");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(0, dsTarget1.Tables["Table1"].Select("ParentId=2").Length , "DS177");
			#endregion

			#region "Merge(drArr,false/true,MissingSchemaAction.Error  )"
			//		dsTarget1 = dsTarget.Copy();
			//		// Merge true,Error - Column
			//		try {
			//			dsTarget1.Merge(drArr,true,MissingSchemaAction.Error );
			//			Assert.Fail("DS178: Merge Failed to throw InvalidOperationException");
			//		}
			//		catch (InvalidOperationException) {}
			//		catch (AssertionException exc) {throw  exc;}
			//		catch (Exception exc)
			//		{
			//			Assert.Fail("DS179: Merge. Wrong exception type. Got:" + exc);
			//		}
			//		
			//		// Merge false,Error - Column
			//		try {
			//			dsTarget1.Merge(drArr,false,MissingSchemaAction.Error );
			//			Assert.Fail("DS180: Merge Failed to throw InvalidOperationException");
			//		}
			//		catch (InvalidOperationException) {}
			//		catch (AssertionException exc) {throw  exc;}
			//		catch (Exception exc)
			//		{
			//			Assert.Fail("DS181: Merge. Wrong exception type. Got:" + exc);
			//		}
			#endregion
		}

		[Test] public void Merge_ByDataSet()
		{
			//create source dataset
			DataSet ds = new DataSet();
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			ds.Tables.Add(dt.Copy());
			dt.TableName = "Table2";
			//add primary key
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};
			ds.Tables.Add(dt.Copy());

			//create target dataset (copy of source dataset)
			DataSet dsTarget = ds.Copy();
			int iTable1RowsCount = dsTarget.Tables["Table1"].Rows.Count;

			//Source - add another table, don't exists on the target dataset
			ds.Tables.Add(new DataTable("SomeTable"));
			ds.Tables["SomeTable"].Columns.Add("Id");
			ds.Tables["SomeTable"].Rows.Add(new object[] {777});

			//Target - add another table, don't exists on the source dataset
			dsTarget.Tables.Add(new DataTable("SmallTable"));
			dsTarget.Tables["SmallTable"].Columns.Add("Id");
			dsTarget.Tables["SmallTable"].Rows.Add(new object[] {777});

			//update existing row
			ds.Tables["Table2"].Select("ParentId=1")[0][1] = "OldValue1";
			//add new row
			object[] arrAddedRow = new object[] {99,"NewValue1","NewValue2",new DateTime(0),0.5,true};
			ds.Tables["Table2"].Rows.Add(arrAddedRow);
			//delete existing rows
			foreach (DataRow dr in ds.Tables["Table2"].Select("ParentId=2"))
			{
				dr.Delete();
			}

			// Merge - changed values
			dsTarget.Merge(ds);
			Assert.AreEqual("OldValue1", dsTarget.Tables["Table2"].Select("ParentId=1")[0][1] , "DS182");

			// Merge - added values
			Assert.AreEqual(arrAddedRow, dsTarget.Tables["Table2"].Select("ParentId=99")[0].ItemArray  , "DS183");

			// Merge - deleted row
			Assert.AreEqual(0, dsTarget.Tables["Table2"].Select("ParentId=2").Length , "DS184");

			//Table1 rows count should be double (no primary key)
			// Merge - Unchanged table 1
			Assert.AreEqual(iTable1RowsCount * 2, dsTarget.Tables["Table1"].Rows.Count , "DS185");

			//SmallTable rows count should be the same
			// Merge - Unchanged table 2
			Assert.AreEqual(1, dsTarget.Tables["SmallTable"].Rows.Count , "DS186");

			//SomeTable - new table
			// Merge - new table
			Assert.AreEqual(true, dsTarget.Tables["SomeTable"] != null, "DS187");
		}

		[Test] public void Merge_ByDataSetPreserve()
		{
			//create source dataset
			DataSet ds = new DataSet();
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			ds.Tables.Add(dt.Copy());
			dt.TableName = "Table2";
			//add primary key
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};
			ds.Tables.Add(dt.Copy());

			//create target dataset (copy of source dataset)
			DataSet dsTarget1 = ds.Copy();
			DataSet dsTarget2 = ds.Copy();
			int iTable1RowsCount = dsTarget1.Tables["Table1"].Rows.Count;

			//update existing row
			string oldValue = ds.Tables["Table2"].Select("ParentId=1")[0][1].ToString();
			ds.Tables["Table2"].Select("ParentId=1")[0][1] = "NewValue";
			//add new row
			object[] arrAddedRow = new object[] {99,"NewValue1","NewValue2",new DateTime(0),0.5,true};
			ds.Tables["Table2"].Rows.Add(arrAddedRow);
			//delete existing rows
			int iDeleteLength = dsTarget1.Tables["Table2"].Select("ParentId=2").Length;
			foreach (DataRow dr in ds.Tables["Table2"].Select("ParentId=2"))
			{
				dr.Delete();
			}

			#region "Merge(ds,true)"
			//only new added rows are merged (preserveChanges = true)
			dsTarget1.Merge(ds,true);
			// Merge - changed values
			Assert.AreEqual(oldValue, dsTarget1.Tables["Table2"].Select("ParentId=1")[0][1] , "DS188");

			// Merge - added values
			Assert.AreEqual(arrAddedRow, dsTarget1.Tables["Table2"].Select("ParentId=99")[0].ItemArray  , "DS189");

			// Merge - deleted row
			Assert.AreEqual(iDeleteLength, dsTarget1.Tables["Table2"].Select("ParentId=2").Length , "DS190");
			#endregion

			#region "Merge(ds,false)"
			//all changes are merged (preserveChanges = false)
			dsTarget2.Merge(ds,false);
			// Merge - changed values
			Assert.AreEqual("NewValue", dsTarget2.Tables["Table2"].Select("ParentId=1")[0][1] , "DS191");

			// Merge - added values
			Assert.AreEqual(arrAddedRow, dsTarget2.Tables["Table2"].Select("ParentId=99")[0].ItemArray  , "DS192");

			// Merge - deleted row
			Assert.AreEqual(0, dsTarget2.Tables["Table2"].Select("ParentId=2").Length , "DS193");
			#endregion
		}

		[Test] public void Merge_ByDataSetPreserveMissingSchemaAction()
		{
			//create source dataset
			DataSet ds = new DataSet();

			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";

			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};

			//add table to dataset
			ds.Tables.Add(dt.Copy());

			dt = ds.Tables[0];

			//create target dataset (copy of source dataset)
			DataSet dsTarget = ds.Copy();

			//add new column (for checking MissingSchemaAction)
			DataColumn dc = new DataColumn("NewColumn",typeof(float));
			//make the column to be primary key
			dt.Columns.Add(dc);

			//add new table (for checking MissingSchemaAction)
			ds.Tables.Add(new DataTable("NewTable"));
			ds.Tables["NewTable"].Columns.Add("NewColumn1",typeof(int));
			ds.Tables["NewTable"].Columns.Add("NewColumn2",typeof(long));
			ds.Tables["NewTable"].Rows.Add(new object[] {1,2});
			ds.Tables["NewTable"].Rows.Add(new object[] {3,4});
			ds.Tables["NewTable"].PrimaryKey = new DataColumn[] {ds.Tables["NewTable"].Columns["NewColumn1"]};

			#region "ds,false,MissingSchemaAction.Add)"
			DataSet dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(ds,false,MissingSchemaAction.Add);
			// Merge MissingSchemaAction.Add - Column
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS194");

			// Merge MissingSchemaAction.Add - Table
			Assert.AreEqual(true, dsTarget1.Tables.Contains("NewTable"), "DS195");

			//failed, should be success by MSDN Library documentation
			//		// Merge MissingSchemaAction.Add - PrimaryKey
			//		Assert.AreEqual(0, dsTarget1.Tables["NewTable"].PrimaryKey.Length, "DS196");
			#endregion

			#region "ds,false,MissingSchemaAction.AddWithKey)"
			//MissingSchemaAction.Add,MissingSchemaAction.AddWithKey - behave the same, checked only Add

			//		DataSet dsTarget2 = dsTarget.Copy();
			//		dsTarget2.Merge(ds,false,MissingSchemaAction.AddWithKey);
			//		// Merge MissingSchemaAction.AddWithKey - Column
			//		Assert.AreEqual(true, dsTarget2.Tables["Table1"].Columns.Contains("NewColumn"), "DS197");
			//
			//		// Merge MissingSchemaAction.AddWithKey - Table
			//		Assert.AreEqual(true, dsTarget2.Tables.Contains("NewTable"), "DS198");
			//
			//		// Merge MissingSchemaAction.AddWithKey - PrimaryKey
			//		Assert.AreEqual(dsTarget2.Tables["NewTable"].Columns["NewColumn1"], dsTarget2.Tables["NewTable"].PrimaryKey[0], "DS199");
			#endregion

			#region "ds,false,MissingSchemaAction.Error)"
			//Error - throw System.Data.DataException, should throw InvalidOperationException
			//		DataSet dsTarget3 ;
			//		// Merge MissingSchemaAction.Error
			//		dsTarget3 = dsTarget.Copy();
			//		try {
			//			dsTarget3.Merge(ds,false,MissingSchemaAction.Error);
			//			Assert.Fail("DS200: Merge Failed to throw InvalidOperationException");
			//		}
			//		catch (InvalidOperationException) {}
			//		catch (AssertionException exc) {throw  exc;}
			//		catch (Exception exc)
			//		{
			//			Assert.Fail("DS201: Merge. Wrong exception type. Got:" + exc);
			//		}
			#endregion

			#region "ds,false,MissingSchemaAction.Ignore )"
			DataSet dsTarget4 = dsTarget.Copy();
			dsTarget4.Merge(ds,false,MissingSchemaAction.Ignore );
			// Merge MissingSchemaAction.Ignore - Column
			Assert.AreEqual(false, dsTarget4.Tables["Table1"].Columns.Contains("NewColumn"), "DS202");

			// Merge MissingSchemaAction.Ignore - Table
			Assert.AreEqual(false, dsTarget4.Tables.Contains("NewTable"), "DS203");
			#endregion
		}

		[Test] public void Merge_ByComplexDataSet()
		{
			//create source dataset
			DataSet ds = new DataSet();

			ds.Tables.Add(DataProvider.CreateParentDataTable());
			ds.Tables.Add(DataProvider.CreateChildDataTable());
			ds.Tables["Child"].TableName = "Child2";
			ds.Tables.Add(DataProvider.CreateChildDataTable());

			// Console.WriteLine(ds.Tables[0].TableName + ds.Tables[1].TableName + ds.Tables[2].TableName);
			// Console.WriteLine(ds.Tables[2].Rows.Count.ToString());

			//craete a target dataset to the merge operation
			DataSet dsTarget = ds.Copy();

			//craete a second target dataset to the merge operation
			DataSet dsTarget1 = ds.Copy();

			//------------------ make some changes in the second target dataset schema --------------------
			//add primary key
			dsTarget1.Tables["Parent"].PrimaryKey = new DataColumn[] {dsTarget1.Tables["Parent"].Columns["ParentId"]};
			dsTarget1.Tables["Child"].PrimaryKey = new DataColumn[] {dsTarget1.Tables["Child"].Columns["ParentId"],dsTarget1.Tables["Child"].Columns["ChildId"]};

			//add Foreign Key (different name)
			dsTarget1.Tables["Child2"].Constraints.Add("Child2_FK_2",dsTarget1.Tables["Parent"].Columns["ParentId"],dsTarget1.Tables["Child2"].Columns["ParentId"]);

			//add relation (different name)
			//dsTarget1.Relations.Add("Parent_Child_1",dsTarget1.Tables["Parent"].Columns["ParentId"],dsTarget1.Tables["Child"].Columns["ParentId"]);

			//------------------ make some changes in the source dataset schema --------------------
			//add primary key
			ds.Tables["Parent"].PrimaryKey = new DataColumn[] {ds.Tables["Parent"].Columns["ParentId"]};
			ds.Tables["Child"].PrimaryKey = new DataColumn[] {ds.Tables["Child"].Columns["ParentId"],ds.Tables["Child"].Columns["ChildId"]};

			//unique column
			ds.Tables["Parent"].Columns["String2"].Unique = true; //will not be merged

			//add Foreign Key
			ds.Tables["Child2"].Constraints.Add("Child2_FK",ds.Tables["Parent"].Columns["ParentId"],ds.Tables["Child2"].Columns["ParentId"]);

			//add relation
			ds.Relations.Add("Parent_Child",ds.Tables["Parent"].Columns["ParentId"],ds.Tables["Child"].Columns["ParentId"]);

			//add allow null constraint
			ds.Tables["Parent"].Columns["ParentBool"].AllowDBNull = false; //will not be merged

			//add Indentity column
			ds.Tables["Parent"].Columns.Add("Indentity",typeof(int));
			ds.Tables["Parent"].Columns["Indentity"].AutoIncrement = true;
			ds.Tables["Parent"].Columns["Indentity"].AutoIncrementStep = 2;

			//modify default value
			ds.Tables["Child"].Columns["String1"].DefaultValue = "Default"; //will not be merged

			//remove column
			ds.Tables["Child"].Columns.Remove("String2"); //will not be merged

			//-------------------- begin to check ----------------------------------------------
			// merge 1 - make sure the merge method invoked without exceptions
			dsTarget.Merge(ds);
			Assert.AreEqual("Success", "Success", "DS204");

			CompareResults_1("merge 1",ds,dsTarget);

			//merge again,
			// merge 2 - make sure the merge method invoked without exceptions
			dsTarget.Merge(ds);
			Assert.AreEqual("Success", "Success", "DS205");

			CompareResults_1("merge 2",ds,dsTarget);

			// merge second dataset - make sure the merge method invoked without exceptions
			dsTarget1.Merge(ds);
			Assert.AreEqual("Success", "Success", "DS206");

			CompareResults_2("merge 3",ds,dsTarget1);
		}

		[Test]
		public void Merge_RelationWithoutConstraints ()
		{
			DataSet ds = new DataSet ();

			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");

			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn ccol = table2.Columns.Add ("col1", typeof (int));

			DataSet ds1 = ds.Copy ();
			DataRelation rel = ds1.Relations.Add ("rel1", ds1.Tables[0].Columns[0], 
								ds1.Tables [1].Columns [0], false);

			ds.Merge (ds1);
			Assert.AreEqual (1, ds.Relations.Count , "#1");
			Assert.AreEqual (0, ds.Tables [0].Constraints.Count , "#2");
			Assert.AreEqual (0, ds.Tables [1].Constraints.Count , "#3");
		}

		[Test]
		public void Merge_DuplicateConstraints ()
		{
			DataSet ds = new DataSet ();

			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");

			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn ccol = table2.Columns.Add ("col1", typeof (int));

			DataSet ds1 = ds.Copy ();

			DataRelation rel = ds.Relations.Add ("rel1", pcol, ccol);

			ds1.Tables [1].Constraints.Add ("fk", ds1.Tables [0].Columns [0], ds1.Tables [1].Columns [0]);

			// No Exceptions shud be thrown
			ds.Merge (ds1);
			Assert.AreEqual (1, table2.Constraints.Count, "#1 Constraints shudnt be duplicated");
		}

		[Test]
		public void Merge_DuplicateConstraints_1 ()
		{
			DataSet ds = new DataSet ();

			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");

			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn ccol = table2.Columns.Add ("col1", typeof (int));
			DataColumn pcol1 = table1.Columns.Add ("col2", typeof (int));
			DataColumn ccol1 = table2.Columns.Add ("col2", typeof (int));

			DataSet ds1 = ds.Copy ();

			table2.Constraints.Add ("fk", pcol, ccol);
			ds1.Tables [1].Constraints.Add ("fk", ds1.Tables [0].Columns ["col2"], ds1.Tables [1].Columns ["col2"]);

			// No Exceptions shud be thrown
			ds.Merge (ds1);
			Assert.AreEqual (2, table2.Constraints.Count, "#1 fk constraint shud be merged");
			Assert.AreEqual ("Constraint1", table2.Constraints [1].ConstraintName, "#2 constraint name shud be changed");
		}

		[Test]
		public void CopyClone_RelationWithoutConstraints ()
		{
			DataSet ds = new DataSet ();

			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");

			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn ccol = table2.Columns.Add ("col1", typeof (int));

			DataRelation rel = ds.Relations.Add ("rel1", pcol, ccol, false);

			DataSet ds1 = ds.Copy ();
			DataSet ds2 = ds.Clone ();
			
			Assert.AreEqual (1, ds1.Relations.Count, "#1");
			Assert.AreEqual (1, ds2.Relations.Count, "#2");

			Assert.AreEqual (0, ds1.Tables [0].Constraints.Count, "#3");
			Assert.AreEqual (0, ds1.Tables [1].Constraints.Count, "#4");

			Assert.AreEqual (0, ds2.Tables [0].Constraints.Count, "#5");
			Assert.AreEqual (0, ds2.Tables [1].Constraints.Count, "#6");
		}

		[Test]
		[ExpectedException (typeof (DataException))]
		public void Merge_MissingEventHandler ()
		{
			DataSet ds = new DataSet ();
			DataTable table1 = ds.Tables.Add ("table1");

			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn pcol1 = table1.Columns.Add ("col2", typeof (int));
			
			DataSet ds1 = ds.Copy ();
			table1.PrimaryKey = new DataColumn[] {pcol};
			ds1.Tables [0].PrimaryKey = new DataColumn[] {ds1.Tables [0].Columns [1]};

			// Exception shud be raised when handler is not set for MergeFailed Event
			ds1.Merge (ds);
		}

		[Test]
		[ExpectedException (typeof(DataException))]
		public void Merge_MissingColumn  ()
		{
			DataSet ds = new DataSet ();
			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");

			table1.Columns.Add ("col1", typeof (int));
			table2.Columns.Add ("col1", typeof (int));

			DataSet ds1 = ds.Copy ();

			ds1.Tables [0].Columns.Add ("col2");

			ds.Merge (ds1, true, MissingSchemaAction.Error);
		}

		[Test]
		public void Merge_MissingConstraint ()
		{
			DataSet ds = new DataSet ();
			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");
			table1.Columns.Add ("col1", typeof (int));
			table2.Columns.Add ("col1", typeof (int));

			try {
				DataSet ds1 = ds.Copy ();
				DataSet ds2 = ds.Copy ();
				ds2.Tables [0].Constraints.Add ("uc", ds2.Tables [0].Columns [0], false);
				ds1.Merge (ds2, true, MissingSchemaAction.Error);
				Assert.Fail ("#1 If uniqueconstraint is missing, exception shud be thrown");
			}catch (DataException e) {
			}

			try {
				DataSet ds1 = ds.Copy ();
				DataSet ds2 = ds.Copy ();
				ds2.Tables [0].Constraints.Add ("fk", ds2.Tables [0].Columns [0], ds2.Tables[1].Columns [0]);
				ds1.Tables [0].Constraints.Add ("uc", ds1.Tables [0].Columns [0],false);
				ds1.Merge (ds2, true, MissingSchemaAction.Error);
				Assert.Fail ("#2 If foreignkeyconstraint is missing, exception shud be thrown");
			}catch (DataException e) {
			}

			try {
				DataSet ds1 = ds.Copy ();
				DataSet ds2 = ds.Copy ();
				ds2.Relations.Add ("rel", ds2.Tables [0].Columns [0], ds2.Tables[1].Columns [0], false);
				ds1.Merge (ds2, true, MissingSchemaAction.Error);
				Assert.Fail ("#2 If datarelation is missing, exception shud be thrown");
			}catch (ArgumentException e) {
			}
		}

		[Test]
		[ExpectedException (typeof (DataException))]
		public void Merge_PrimaryKeys_IncorrectOrder ()
		{
			DataSet ds = new DataSet ();
			DataTable table1 = ds.Tables.Add ("table1");
			DataTable table2 = ds.Tables.Add ("table2");
			DataColumn pcol = table1.Columns.Add ("col1", typeof (int));
			DataColumn pcol1 = table1.Columns.Add ("col2", typeof (int));
			DataColumn ccol = table2.Columns.Add ("col1", typeof (int));

			DataSet ds1 = ds.Copy ();
			table1.PrimaryKey = new DataColumn[] {pcol,pcol1};
			ds1.Tables [0].PrimaryKey = new DataColumn [] {ds1.Tables[0].Columns [1], ds1.Tables [0].Columns [0]};

			// Though the key columns are the same, if the order is incorrect
			// Exception must be raised
			ds1.Merge (ds);
		}

		void CompareResults_1(string Msg,DataSet ds, DataSet dsTarget)
		{
			// check Parent Primary key length
			Assert.AreEqual(dsTarget.Tables["Parent"].PrimaryKey.Length , ds.Tables["Parent"].PrimaryKey.Length , "DS207");

			// check Child Primary key length
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey.Length , ds.Tables["Child"].PrimaryKey.Length , "DS208");

			// check Parent Primary key columns
			Assert.AreEqual(dsTarget.Tables["Parent"].PrimaryKey[0].ColumnName, ds.Tables["Parent"].PrimaryKey[0].ColumnName , "DS209");

			// check Child Primary key columns[0]
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey[0].ColumnName, ds.Tables["Child"].PrimaryKey[0].ColumnName , "DS210");

			// check Child Primary key columns[1]
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey[1].ColumnName, ds.Tables["Child"].PrimaryKey[1].ColumnName , "DS211");

			// check Parent Unique columns
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["String2"].Unique, ds.Tables["Parent"].Columns["String2"].Unique , "DS212");

			// check Child2 Foreign Key name
			Assert.AreEqual(dsTarget.Tables["Child2"].Constraints[0].ConstraintName , ds.Tables["Child2"].Constraints[0].ConstraintName , "DS213");

			// check dataset relation count
			Assert.AreEqual(dsTarget.Relations.Count , ds.Relations.Count , "DS214");

			// check dataset relation - Parent column
			Assert.AreEqual(dsTarget.Relations[0].ParentColumns[0].ColumnName , ds.Relations[0].ParentColumns[0].ColumnName , "DS215");

			// check dataset relation - Child column 
			Assert.AreEqual(dsTarget.Relations[0].ChildColumns[0].ColumnName , ds.Relations[0].ChildColumns[0].ColumnName , "DS216");

			// check allow null constraint
			Assert.AreEqual(true, dsTarget.Tables["Parent"].Columns["ParentBool"].AllowDBNull, "DS217");

			// check Indentity column
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns.Contains("Indentity"), ds.Tables["Parent"].Columns.Contains("Indentity"), "DS218");

			// check Indentity column - AutoIncrementStep
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["Indentity"].AutoIncrementStep, ds.Tables["Parent"].Columns["Indentity"].AutoIncrementStep, "DS219");

			// check Indentity column - AutoIncrement
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["Indentity"].AutoIncrement, ds.Tables["Parent"].Columns["Indentity"].AutoIncrement, "DS220");

			// check Indentity column - DefaultValue
			Assert.AreEqual(true, dsTarget.Tables["Child"].Columns["String1"].DefaultValue == DBNull.Value , "DS221");

			// check remove colum
			Assert.AreEqual(true, dsTarget.Tables["Child"].Columns.Contains("String2"), "DS222");
		}

		void CompareResults_2(string Msg,DataSet ds, DataSet dsTarget)
		{
			// check Parent Primary key length
			Assert.AreEqual(dsTarget.Tables["Parent"].PrimaryKey.Length , ds.Tables["Parent"].PrimaryKey.Length , "DS223");

			// check Child Primary key length
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey.Length , ds.Tables["Child"].PrimaryKey.Length , "DS224");

			// check Parent Primary key columns
			Assert.AreEqual(dsTarget.Tables["Parent"].PrimaryKey[0].ColumnName, ds.Tables["Parent"].PrimaryKey[0].ColumnName , "DS225");

			// check Child Primary key columns[0]
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey[0].ColumnName, ds.Tables["Child"].PrimaryKey[0].ColumnName , "DS226");

			// check Child Primary key columns[1]
			Assert.AreEqual(dsTarget.Tables["Child"].PrimaryKey[1].ColumnName, ds.Tables["Child"].PrimaryKey[1].ColumnName , "DS227");

			// check Parent Unique columns
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["String2"].Unique, ds.Tables["Parent"].Columns["String2"].Unique , "DS228");

			// check Child2 Foreign Key name
			Assert.AreEqual("Child2_FK_2" , dsTarget.Tables["Child2"].Constraints[0].ConstraintName, "DS229");

			// check dataset relation count
			Assert.AreEqual(dsTarget.Relations.Count , ds.Relations.Count , "DS230");

			// check dataset relation - Parent column
			Assert.AreEqual(dsTarget.Relations[0].ParentColumns[0].ColumnName , ds.Relations[0].ParentColumns[0].ColumnName , "DS231");

			// check dataset relation - Child column 
			Assert.AreEqual(dsTarget.Relations[0].ChildColumns[0].ColumnName , ds.Relations[0].ChildColumns[0].ColumnName , "DS232");

			// check allow null constraint
			Assert.AreEqual(true, dsTarget.Tables["Parent"].Columns["ParentBool"].AllowDBNull, "DS233");

			// check Indentity column
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns.Contains("Indentity"), ds.Tables["Parent"].Columns.Contains("Indentity"), "DS234");

			// check Indentity column - AutoIncrementStep
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["Indentity"].AutoIncrementStep, ds.Tables["Parent"].Columns["Indentity"].AutoIncrementStep, "DS235");

			// check Indentity column - AutoIncrement
			Assert.AreEqual(dsTarget.Tables["Parent"].Columns["Indentity"].AutoIncrement, ds.Tables["Parent"].Columns["Indentity"].AutoIncrement, "DS236");

			// check Indentity column - DefaultValue
			Assert.AreEqual(true, dsTarget.Tables["Child"].Columns["String1"].DefaultValue == DBNull.Value , "DS237");

			// check remove colum
			Assert.AreEqual(true, dsTarget.Tables["Child"].Columns.Contains("String2"), "DS238");
			//TestCase for bug #3168
			// Check Relation.Nested value, TestCase for bug #3168
			DataSet orig = new DataSet();

			DataTable parent = orig.Tables.Add("Parent");
			parent.Columns.Add("Id", typeof(int));
			parent.Columns.Add("col1", typeof(string));
			parent.Rows.Add(new object[] {0, "aaa"});

			DataTable child = orig.Tables.Add("Child");
			child.Columns.Add("ParentId", typeof(int));
			child.Columns.Add("col1", typeof(string));
			child.Rows.Add(new object[] {0, "bbb"});

			orig.Relations.Add("Parent_Child", parent.Columns["Id"], child.Columns["ParentId"]);
			orig.Relations["Parent_Child"].Nested = true;

			DataSet merged = new DataSet();
			merged.Merge(orig);
			Assert.AreEqual(orig.Relations["Parent_Child"].Nested, merged.Relations["Parent_Child"].Nested, "DS239");
		}

		[Test] public void Merge_ByDataTable()
		{
			//create source dataset
			DataSet ds = new DataSet();
			//create datatable
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			//add a copy of the datatable to the dataset
			ds.Tables.Add(dt.Copy());

			dt.TableName = "Table2";
			//add primary key
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};
			ds.Tables.Add(dt.Copy());
			//now the dataset hase two tables

			//create target dataset (copy of source dataset)
			DataSet dsTarget = ds.Copy();

			dt = ds.Tables["Table2"];
			//update existing row
			dt.Select("ParentId=1")[0][1] = "OldValue1";
			//add new row
			object[] arrAddedRow = new object[] {99,"NewValue1","NewValue2",new DateTime(0),0.5,true};
			dt.Rows.Add(arrAddedRow);
			//delete existing rows
			foreach (DataRow dr in dt.Select("ParentId=2"))
			{
				dr.Delete();
			}

			// Merge - changed values
			dsTarget.Merge(dt);
			Assert.AreEqual("OldValue1", dsTarget.Tables["Table2"].Select("ParentId=1")[0][1] , "DS240");

			// Merge - added values
			Assert.AreEqual(arrAddedRow, dsTarget.Tables["Table2"].Select("ParentId=99")[0].ItemArray  , "DS241");

			// Merge - deleted row
			Assert.AreEqual(0, dsTarget.Tables["Table2"].Select("ParentId=2").Length , "DS242");

			//test case added due to a reported bug from infogate
			//when merging a DataTable with TableName=null, GH throw null reference exception.
			ds = new DataSet();
			dt = new DataTable();
			dt.Columns.Add("Col1");
			dt.Rows.Add(new object[] {1});

			// Merge - add a table with no name
			ds.Merge(dt);
			Assert.AreEqual(1, ds.Tables.Count, "DS243");

			// Merge - add a table with no name - check Rows.Count
			Assert.AreEqual(dt.Rows.Count , ds.Tables[0].Rows.Count , "DS244");
		}

		[Test] public void Merge_ByDataTablePreserveMissingSchemaAction()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			dt.TableName = "Table1";
			dt.PrimaryKey = new DataColumn[] {dt.Columns[0]};

			//create target dataset (copy of source dataset)
			DataSet dsTarget = new DataSet();
			dsTarget.Tables.Add(dt.Copy());

			//add new column (for checking MissingSchemaAction)
			DataColumn dc = new DataColumn("NewColumn",typeof(float));
			dt.Columns.Add(dc);

			//Update row
			string OldValue = dt.Select("ParentId=1")[0][1].ToString();
			dt.Select("ParentId=1")[0][1] = "NewValue";
			//delete rows
			dt.Select("ParentId=2")[0].Delete();
			//add row
			object[] arrAddedRow = new object[] {99,"NewRowValue1","NewRowValue2",new DateTime(0),0.5,true};
			dt.Rows.Add(arrAddedRow);

			#region "Merge(dt,true,MissingSchemaAction.Ignore )"
			DataSet dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(dt,true,MissingSchemaAction.Ignore );
			// Merge true,Ignore - Column
			Assert.AreEqual(false, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS245");

			// Merge true,Ignore - changed values
			Assert.AreEqual(OldValue, dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS246");

			// Merge true,Ignore - added values
			Assert.AreEqual(arrAddedRow, dsTarget1.Tables["Table1"].Select("ParentId=99")[0].ItemArray  , "DS247");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Select("ParentId=2").Length > 0, "DS248");
			#endregion

			#region "Merge(dt,false,MissingSchemaAction.Ignore )"

			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(dt,false,MissingSchemaAction.Ignore );
			// Merge true,Ignore - Column
			Assert.AreEqual(false, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS249");

			// Merge true,Ignore - changed values
			Assert.AreEqual("NewValue", dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS250");

			// Merge true,Ignore - added values
			Assert.AreEqual(arrAddedRow, dsTarget1.Tables["Table1"].Select("ParentId=99")[0].ItemArray  , "DS251");

			// Merge true,Ignore - deleted row
			Assert.AreEqual(0, dsTarget1.Tables["Table1"].Select("ParentId=2").Length , "DS252");
			#endregion

			#region "Merge(dt,true,MissingSchemaAction.Add  )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(dt,true,MissingSchemaAction.Add  );
			// Merge true,Add - Column
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS253");

			// Merge true,Add - changed values
			Assert.AreEqual(OldValue, dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS254");

			// Merge true,Add - added values
			Assert.AreEqual(1, dsTarget1.Tables["Table1"].Select("ParentId=99").Length , "DS255");

			// Merge true,Add - deleted row
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Select("ParentId=2").Length > 0, "DS256");
			#endregion

			#region "Merge(dt,false,MissingSchemaAction.Add  )"
			dsTarget1 = dsTarget.Copy();
			dsTarget1.Merge(dt,false,MissingSchemaAction.Add  );
			// Merge true,Add - Column
			Assert.AreEqual(true, dsTarget1.Tables["Table1"].Columns.Contains("NewColumn"), "DS257");

			// Merge true,Add - changed values
			Assert.AreEqual("NewValue", dsTarget1.Tables["Table1"].Select("ParentId=1")[0][1] , "DS258");

			// Merge true,Add - added values
			Assert.AreEqual(1, dsTarget1.Tables["Table1"].Select("ParentId=99").Length , "DS259");

			// Merge true,Add - deleted row
			Assert.AreEqual(0, dsTarget1.Tables["Table1"].Select("ParentId=2").Length , "DS260");
			#endregion

			#region "Merge(dt,false/true,MissingSchemaAction.Error  )"
			//		dsTarget1 = dsTarget.Copy();
			//		// Merge true,Error - Column
			//		try {
			//			dsTarget1.Merge(dt,true,MissingSchemaAction.Error );
			//			Assert.Fail("DS261: Merge Failed to throw InvalidOperationException");
			//		}
			//		catch (InvalidOperationException) {}
			//		catch (AssertionException exc) {throw  exc;}
			//		catch (Exception exc)
			//		{
			//			Assert.Fail("DS262: Merge. Wrong exception type. Got:" + exc);
			//		}
			//
			//		// Merge false,Error - Column
			//		try {
			//			dsTarget1.Merge(dt,false,MissingSchemaAction.Error );
			//			Assert.Fail("DS263: Merge Failed to throw InvalidOperationException");
			//		}
			//		catch (InvalidOperationException) {}
			//		catch (AssertionException exc) {throw  exc;}
			//		catch (Exception exc)
			//		{
			//			Assert.Fail("DS264: Merge. Wrong exception type. Got:" + exc);
			//		}
			#endregion
		}

		[Test] public void Namespace()
		{
			DataSet ds = new DataSet();

			// Checking Namespace default
			Assert.AreEqual(String.Empty, ds.Namespace, "DS265");

			// Checking Namespace set/get
			String s = "MyNamespace";
			ds.Namespace=s;
			Assert.AreEqual(s, ds.Namespace, "DS266");
		}

		[Test] public void Prefix()
		{
			DataSet ds = new DataSet();

			// Checking Prefix default
			Assert.AreEqual(String.Empty, ds.Prefix , "DS267");

			// Checking Prefix set/get
			String s = "MyPrefix";
			ds.Prefix=s;
			Assert.AreEqual(s, ds.Prefix, "DS268");
		}
		
		private void privateTestCase(string name, string toTable, string toTestSelect, string toCompareTable, string toCompareSelect, string AssertTag)
		{
			DataRow drToTest = m_ds.Tables[toTable].Select(toTestSelect)[0];
			DataRow drToCompare = m_ds.Tables[toCompareTable].Select(toCompareSelect)[0];
			Assert.AreEqual(m_ds.Tables[toTable].Select(toTestSelect)[0]["Stock_Id"], m_ds.Tables[toCompareTable].Select(toCompareSelect)[0]["Stock_Id"], AssertTag);
		}

		private string dataSetDescription(DataSet ds)
		{
			string desc = string.Empty;
			desc += "DataSet Name=" + ds.DataSetName;
			desc += " Tables count=" + ds.Tables.Count;
			foreach (DataTable dt in ds.Tables)
			{
				desc += " Table Name=" + dt.TableName;
				desc += " Rows count=" + dt.Rows.Count;

				string[] colNames = new string[dt.Columns.Count];
				for(int i = 0; i < dt.Columns.Count; i++)
					colNames[i] = dt.Columns[i].ColumnName;

				Array.Sort(colNames);

				foreach (DataRow dr in dt.Rows)
				{
					desc += " Items count=" + dr.ItemArray.Length;
					foreach (string name in colNames)
					{
						desc += " " + dr[name].ToString();
					}
				}
			}
			return desc;
		}

		[Test] public void Relations()
		{
			DataTable dtChild1,dtChild2,dtParent;
			DataSet ds = new DataSet();
			//Create tables
			dtChild1 = DataProvider.CreateChildDataTable();
			dtChild1.TableName = "Child";
			dtChild2 = DataProvider.CreateChildDataTable();
			dtChild2.TableName = "CHILD";
			dtParent= DataProvider.CreateParentDataTable();
			//Add tables to dataset
			ds.Tables.Add(dtChild1);
			ds.Tables.Add(dtChild2);

			ds.Tables.Add(dtParent);

			DataRelation drl = new DataRelation("Parent-Child",dtParent.Columns["ParentId"],dtChild1.Columns["ParentId"]);
			DataRelation drl1 = new DataRelation("Parent-CHILD",dtParent.Columns["ParentId"],dtChild2.Columns["ParentId"]);

			// Checking Relations - default value
			//Check default
			Assert.AreEqual(0, ds.Relations.Count  , "DS346");

			ds.Relations.Add(drl);

			// Checking Relations Count
			Assert.AreEqual(1, ds.Relations.Count  , "DS347");

			// Checking Relations Value
			Assert.AreEqual(drl, ds.Relations[0] , "DS348");

			// Checking Relations - get by name
			Assert.AreEqual(drl, ds.Relations["Parent-Child"] , "DS349");

			// Checking Relations - get by name case sensetive
			Assert.AreEqual(drl, ds.Relations["PARENT-CHILD"] , "DS350");

			// Checking Relations Count 2
			ds.Relations.Add(drl1);
			Assert.AreEqual(2, ds.Relations.Count  , "DS351");

			// Checking Relations - get by name case sensetive,ArgumentException
			try
			{
				DataRelation tmp = ds.Relations["PARENT-CHILD"];
				Assert.Fail("DS352: Relations Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DS353: Relations. Wrong exception type. Got:" + exc);
			}
		}

		[Test] public void Reset()
		{
			DataTable dt1 = DataProvider.CreateParentDataTable();
			DataTable dt2 = DataProvider.CreateChildDataTable();
			dt1.PrimaryKey  = new DataColumn[] {dt1.Columns[0]};
			dt2.PrimaryKey  = new DataColumn[] {dt2.Columns[0],dt2.Columns[1]};
			DataRelation rel = new DataRelation("Rel",dt1.Columns["ParentId"],dt2.Columns["ParentId"]);
			DataSet ds = new DataSet();
			ds.Tables.AddRange(new DataTable[] {dt1,dt2});
			ds.Relations.Add(rel);

			ds.Reset();

			// Reset - Relations
			Assert.AreEqual(0 , ds.Relations.Count  , "DS354");
			// Reset - Tables
			Assert.AreEqual(0 , ds.Tables.Count  , "DS355");
		}

		[Test] public void ShouldSerializeRelations()
		{
			// DataSet ShouldSerializeRelations
			newDataSet ds = new newDataSet();

			Assert.AreEqual(true, ds.testMethod(), "DS356");
		}

		class newDataSet:DataSet
		{
			public bool testMethod()
			{
				return ShouldSerializeRelations();
			}
		}
		
		[Test] public void ShouldSerializeTables()
		{
			// DataSet ShouldSerializeTables
			newDataSet1 ds = new newDataSet1();

			Assert.AreEqual(true, ds.testMethod(), "DS357");
		}

		class newDataSet1:DataSet
		{
			public bool testMethod()
			{
				return ShouldSerializeTables();
			}
		}
		[Test] public void Tables()
		{
			//References by name to tables and relations in a DataSet are case-sensitive. Two or more tables or relations can exist in a DataSet that have the same name, but that differ in case. For example you can have Table1 and table1. In this situation, a reference to one of the tables by name must match the case of the table name exactly, otherwise an exception is thrown. For example, if the DataSet myDS contains tables Table1 and table1, you would reference Table1 by name as myDS.Tables["Table1"], and table1 as myDS.Tables ["table1"]. Attempting to reference either of the tables as myDS.Tables ["TABLE1"] would generate an exception.
			//The case-sensitivity rule does not apply if only one table or relation exists with a particular name. That is, if no other table or relation object in the DataSet matches the name of that particular table or relation object, even by a difference in case, you can reference the object by name using any case and no exception is thrown. For example, if the DataSet has only Table1, you can reference it using myDS.Tables["TABLE1"].
			//The CaseSensitive property of the DataSet does not affect this behavior. The CaseSensitive property

			DataSet ds = new DataSet();

			DataTable dt1 = new DataTable();
			DataTable dt2 = new DataTable();
			DataTable dt3 = new DataTable();
			dt3.TableName = "Table3";
			DataTable dt4 = new DataTable(dt3.TableName.ToUpper());

			// Checking Tables - default value
			//Check default
			Assert.AreEqual(0, ds.Tables.Count  , "DS358");

			ds.Tables.Add(dt1);
			ds.Tables.Add(dt2);
			ds.Tables.Add(dt3);

			// Checking Tables Count
			Assert.AreEqual(3, ds.Tables.Count  , "DS359");

			// Checking Tables Value 1
			Assert.AreEqual(dt1, ds.Tables[0] , "DS360");

			// Checking Tables Value 2
			Assert.AreEqual(dt2, ds.Tables[1] , "DS361");

			// Checking Tables Value 3
			Assert.AreEqual(dt3, ds.Tables[2] , "DS362");

			// Checking get table by name.ToUpper
			Assert.AreEqual(dt3, ds.Tables[dt3.TableName.ToUpper()] , "DS363");

			// Checking get table by name.ToLower
			Assert.AreEqual(dt3, ds.Tables[dt3.TableName.ToLower()] , "DS364");

			// Checking Tables add with name case insensetive
			ds.Tables.Add(dt4); //same name as Table3, but different case
			Assert.AreEqual(4, ds.Tables.Count  , "DS365");

			// Checking get table by name
			Assert.AreEqual(dt4, ds.Tables[dt4.TableName] , "DS366");

			// Checking get table by name with diferent case, ArgumentException
			try
			{
				DataTable tmp = ds.Tables[dt4.TableName.ToLower()];
				Assert.Fail("DS367: Tables Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DS368: Tables. Wrong exception type. Got:" + exc);
			}
		}
		
		[Test] public void ctor()
		{
			DataSet ds;

			// ctor
			ds = new DataSet();
			Assert.AreEqual(true, ds != null , "DS370");
		}

		[Test] public void ctor_ByDataSetName()
		{
			DataSet ds = null;

			// ctor
			ds = new DataSet("NewDataSet");
			Assert.AreEqual(true, ds != null , "DS371");

			// ctor - name
			Assert.AreEqual("NewDataSet" , ds.DataSetName  , "DS372");
		}

		[Test] public void extendedProperties()
		{
			DataSet ds = new DataSet();
			PropertyCollection pc;

			pc = ds.ExtendedProperties ;

			// Checking ExtendedProperties default
			Assert.AreEqual(true, pc != null, "DS373");

			// Checking ExtendedProperties count
			Assert.AreEqual(0, pc.Count , "DS374");
		}
		
		[Test]	
        	public void MergeTest_ColumnTypeMismatch ()
        	{
			DataSet dataSet = new DataSet ();
			dataSet.Tables.Add (new DataTable ());
			dataSet.Tables [0].Columns.Add (new DataColumn ("id", typeof (int)));
			dataSet.Tables [0].Columns.Add (new DataColumn ("name", typeof (string)));

			DataSet ds = new DataSet ();
			ds.Tables.Add (new DataTable ());
			ds.Tables [0].Columns.Add (new DataColumn ("id", typeof (string)));

			try {
				ds.Merge (dataSet, true, MissingSchemaAction.Add);
				Assert.Fail ("#1");
			} catch (DataException e) {}

			ds = new DataSet ();
			ds.Tables.Add (new DataTable ());
			ds.Tables [0].Columns.Add (new DataColumn("id", typeof (string)));

			ds.Merge (dataSet, true, MissingSchemaAction.Ignore);

			Assert.AreEqual ("Table1", ds.Tables [0].TableName, "#2");
			Assert.AreEqual (1, ds.Tables.Count, "#3");
			Assert.AreEqual (1, ds.Tables [0].Columns.Count, "#4"); 	
			Assert.AreEqual (typeof (string), ds.Tables [0].Columns [0].DataType, "#5");
        	}
			         
       [Test]
       public void MergeTest_SameDataSet_536194 ()
       {
               DataSet dataSet = new DataSet ("Test");
               
               DataTable dataTable = new DataTable("Test");
               dataTable.Columns.Add("Test");
               dataTable.Rows.Add("Test");
               dataSet.Tables.Add(dataTable);
               dataSet.Merge(dataTable);
               Assert.AreEqual (1, dataSet.Tables.Count, "1");
       }

		/*
		//FIXME: DataTableReader not supported
		[Test]	
        	public void LoadTest1 ()
        	{
			DataSet ds1 = new DataSet ();
			DataSet ds2 = new DataSet ();
			DataTable dt1 = new DataTable ("T1");
			DataTable dt2 = new DataTable ("T2");
			DataTable dt3 = new DataTable ("T1");
			DataTable dt4 = new DataTable ("T2");
			dt1.Columns.Add ("ID", typeof (int));
			dt1.Columns.Add ("Name", typeof (string));
			dt2.Columns.Add ("EmpNO", typeof (int));
			dt2.Columns.Add ("EmpName", typeof (string));

			dt1.Rows.Add (new object[] {1, "Andrews"});
			dt1.Rows.Add (new object[] {2, "Mathew"});
			dt1.Rows.Add (new object[] {3, "Jaccob"});

			dt2.Rows.Add (new object[] {1, "Arul"});
			dt2.Rows.Add (new object[] {2, "Jothi"});
			dt2.Rows.Add (new object[] {3, "Murugan"});

			ds2.Tables.Add (dt1);
			ds2.Tables.Add (dt2);
			ds1.Tables.Add (dt3);
			ds1.Tables.Add (dt4);

			DataTableReader reader = ds2.CreateDataReader ();
			//ds1.Load (reader, LoadOption.PreserveChanges, dt3, dt4);
			ds1.Load (reader, LoadOption.OverwriteChanges, dt3, dt4);

			Assert.AreEqual (ds2.Tables.Count, ds1.Tables.Count, "DataSet Tables count mistmatch");
			int i = 0;
			foreach (DataTable dt in ds1.Tables) {
				DataTable dt5 = ds2.Tables[i];
				Assert.AreEqual (dt5.Rows.Count, dt.Rows.Count, "Table " + dt.TableName + " row count mistmatch");
				int j = 0;
				DataRow row1;
				foreach (DataRow row in dt.Rows) {
					row1 = dt5.Rows[j];
					for (int k = 0; k < dt.Columns.Count; k++) {
						Assert.AreEqual (row1[k], row[k], "DataRow " + k + " mismatch");
					}
					j++;
				}
				i++;
			}
		}
		
		[Test]	
        	public void LoadTest2 ()
        	{
			DataSet ds1 = new DataSet ();
			DataSet ds2 = new DataSet ();
			DataTable dt1 = new DataTable ("T1");
			DataTable dt2 = new DataTable ("T2");
			DataTable dt3 = new DataTable ("T1");
			DataTable dt4 = new DataTable ("T2");
			dt1.Columns.Add ("ID", typeof (int));
			dt1.Columns.Add ("Name", typeof (string));
			dt2.Columns.Add ("EmpNO", typeof (int));
			dt2.Columns.Add ("EmpName", typeof (string));

			dt1.Rows.Add (new object[] {1, "Andrews"});
			dt1.Rows.Add (new object[] {2, "Mathew"});
			dt1.Rows.Add (new object[] {3, "Jaccob"});

			dt2.Rows.Add (new object[] {1, "Arul"});
			dt2.Rows.Add (new object[] {2, "Jothi"});
			dt2.Rows.Add (new object[] {3, "Murugan"});

			ds2.Tables.Add (dt1);
			ds2.Tables.Add (dt2);
			ds1.Tables.Add (dt3);
			ds1.Tables.Add (dt4);

			DataTableReader reader = ds2.CreateDataReader ();
			//ds1.Load (reader, LoadOption.PreserveChanges, dt3, dt4);
			ds1.Load (reader, LoadOption.OverwriteChanges, dt3, dt4);

			Assert.AreEqual (ds2.Tables.Count, ds1.Tables.Count, "DataSet Tables count mistmatch");
			int i = 0;
			foreach (DataTable dt in ds1.Tables) {
				DataTable dt5 = ds2.Tables[i];
				Assert.AreEqual (dt5.Rows.Count, dt.Rows.Count, "Table " + dt.TableName + " row count mistmatch");
				int j = 0;
				DataRow row1;
				foreach (DataRow row in dt.Rows) {
					row1 = dt5.Rows[j];
					for (int k = 0; k < dt.Columns.Count; k++) {
						Assert.AreEqual (row1[k], row[k], "DataRow " + k + " mismatch");
					}
					j++;
				}
				i++;
			}
		}
		*/
		private void AssertDataTableValues (DataTable dt)
		{
			Assert.AreEqual ("data1", dt.Rows[0]["_ID"], "1");
			Assert.AreEqual ("data2", dt.Rows[0]["#ID"], "2");
			Assert.AreEqual ("data3", dt.Rows[0]["%ID"], "2");
			Assert.AreEqual ("data4", dt.Rows[0]["$ID"], "2");
			Assert.AreEqual ("data5", dt.Rows[0][":ID"], "2");
			Assert.AreEqual ("data6", dt.Rows[0][".ID"], "2");
			Assert.AreEqual ("data7", dt.Rows[0]["ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["*ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["+ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["-ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["~ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["@ID"], "2");
			Assert.AreEqual ("data8", dt.Rows[0]["&ID"], "2");

		}

		[Test, Ignore]	
		public void Bug537229_BinFormatSerializer_Test ()
		{
			DataSet ds = new DataSet ();
			DataTable dt = new DataTable ();
	        ds.Tables.Add (dt);
	        dt.Columns.Add ("_ID", typeof(String));
	        dt.Columns.Add ("#ID", typeof(String));
	        dt.Columns.Add ("%ID", typeof(String));
	        dt.Columns.Add ("$ID", typeof(String));
	        dt.Columns.Add (":ID", typeof(String));
	        dt.Columns.Add (".ID", typeof(String));
	        dt.Columns.Add ("ID", typeof(String));
	        dt.Columns.Add ("*ID", typeof(String));
	        dt.Columns.Add ("+ID", typeof(String));
	        dt.Columns.Add ("-ID", typeof(String));
	        dt.Columns.Add ("~ID", typeof(String));
	        dt.Columns.Add ("@ID", typeof(String));
	        dt.Columns.Add ("&ID", typeof(String));
	        DataRow row = dt.NewRow ();
	        row["#ID"] = "data2";
	        row["%ID"] = "data3";
	        row["$ID"] = "data4";
	        row["ID"] = "data7";
	        row[":ID"] = "data5";
	        row[".ID"] = "data6";
	        row["_ID"] = "data1";
	        row["*ID"] = "data8";
	        row["+ID"] = "data8";
	        row["-ID"] = "data8";
	        row["~ID"] = "data8";
	        row["@ID"] = "data8";
	        row["&ID"] = "data8";
	        dt.Rows.Add (row);

			AssertDataTableValues (dt);

	        MemoryStream mstm=new MemoryStream();
	        BinaryFormatter bfmt=new BinaryFormatter();
	        bfmt.Serialize(mstm,dt);
	        MemoryStream mstm2=new MemoryStream(mstm.ToArray());
	        DataTable vdt=(DataTable)bfmt.Deserialize(mstm2);
			AssertDataTableValues (vdt);
		}
	}
}
