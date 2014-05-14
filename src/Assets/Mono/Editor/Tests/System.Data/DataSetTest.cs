// MonoTests.System.Data.DataSetTest.cs
//
// Authors:
//   Ville Palo <vi64pa@koti.soon.fi>
//   Martin Willemoes Hansen <mwh@sysrq.dk>
//   Atsushi Enomoto <atsushi@ximian.com>
//   Hagit Yidov <hagity@mainsoft.com>
//
// (C) Copyright 2002 Ville Palo
// (C) Copyright 2003 Martin Willemoes Hansen
// (C) 2005 Mainsoft Corporation (http://www.mainsoft.com)
// Copyright 2011 Xamarin Inc.
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


using NUnit.Framework;
using System;
using System.IO;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Threading;
using System.Text;

namespace MonoTests.System.Data
{
	[TestFixture]
        public class DataSetTest : DataSetAssertion
        {
        	string EOL = Environment.NewLine;
		CultureInfo currentCultureBackup;

		[SetUp]
		public void Setup () {
			currentCultureBackup = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo ("fi-FI");
		}

		//[SetUp]
		//public void GetReady()
		//{
		//        currentCultureBackup = Thread.CurrentThread.CurrentCulture;
		//        Thread.CurrentThread.CurrentCulture = new CultureInfo ("fi-FI");
		//}

		[TearDown]
		public void Teardown ()
		{
			Thread.CurrentThread.CurrentCulture = currentCultureBackup;
		}

		[Test]
		public void Properties ()
		{
			DataSet ds = new DataSet ();
			Assert.AreEqual (String.Empty, ds.Namespace, "default namespace");
			ds.Namespace = null; // setting null == setting ""
			Assert.AreEqual (String.Empty, ds.Namespace, "after setting null to namespace");

			Assert.AreEqual (String.Empty, ds.Prefix, "default prefix");
			ds.Prefix = null; // setting null == setting ""
			Assert.AreEqual (String.Empty, ds.Prefix, "after setting null to prefix");
		}
		
		[Test]
                public void CloneCopy ()
                {
                        DataTable table = new DataTable ("pTable");
			DataTable table1 = new DataTable ("cTable");
			DataSet set = new DataSet ();

                        set.Tables.Add (table);
                        set.Tables.Add (table1);

			DataColumn col = new DataColumn ();
                        col.ColumnName = "Id";
                        col.DataType = Type.GetType ("System.Int32");
                        table.Columns.Add (col);
                        UniqueConstraint uc = new UniqueConstraint ("UK1", table.Columns[0] );
                        table.Constraints.Add (uc);

                        col = new DataColumn ();
                        col.ColumnName = "Name";
                        col.DataType = Type.GetType ("System.String");
                        table.Columns.Add (col);

                        col = new DataColumn ();
                        col.ColumnName = "Id";
                        col.DataType = Type.GetType ("System.Int32");
                        table1.Columns.Add (col);

                        col = new DataColumn ();
                        col.ColumnName = "Name";
                        col.DataType = Type.GetType ("System.String");
		        table1.Columns.Add (col);
			  ForeignKeyConstraint fc = new ForeignKeyConstraint ("FK1", table.Columns[0], table1.Columns[0] );
                        table1.Constraints.Add (fc);


                        DataRow row = table.NewRow ();

                        row ["Id"] = 147;
                        row ["name"] = "Row1";
                        row.RowError = "Error#1";
                        table.Rows.Add (row);

			// Set column to RO as commonly used by auto-increment fields.
			// ds.Copy() has to omit the RO check when cloning DataRows 
			table.Columns["Id"].ReadOnly = true;
			
                        row = table1.NewRow ();
                        row ["Id"] = 147;
                        row ["Name"] = "Row1";
                        table1.Rows.Add (row);

                        //Setting properties of DataSet
                        set.CaseSensitive = true;
                        set.DataSetName = "My DataSet";
                        set.EnforceConstraints = false;
                        set.Namespace = "Namespace#1";
                        set.Prefix = "Prefix:1";
                        DataRelation dr = new DataRelation ("DR", table.Columns [0],table1.Columns [0]);
                        set.Relations.Add (dr);
                        set.ExtendedProperties.Add ("TimeStamp", DateTime.Now);
                        CultureInfo cultureInfo = new CultureInfo( "ar-SA" );
                        set.Locale = cultureInfo;

                        //Testing Copy ()
                        DataSet copySet = set.Copy ();
                        Assert.AreEqual (set.CaseSensitive, copySet.CaseSensitive, "#A01");
			Assert.AreEqual (set.DataSetName, copySet.DataSetName, "#A02");
                        Assert.AreEqual (set.EnforceConstraints, copySet.EnforceConstraints, "#A03");
                        Assert.AreEqual (set.HasErrors, copySet.HasErrors, "#A04");
                        Assert.AreEqual (set.Namespace, copySet.Namespace, "#A05");
                        Assert.AreEqual (set.Prefix, copySet.Prefix, "#A06");
                        Assert.AreEqual (set.Relations.Count, copySet.Relations.Count, "#A07");
                        Assert.AreEqual (set.Tables.Count, copySet.Tables.Count, "#A08");
                        Assert.AreEqual (set.ExtendedProperties ["TimeStamp"], copySet.ExtendedProperties ["TimeStamp"], "#A09");
                        for (int i = 0;i < copySet.Tables.Count; i++) {
                                Assert.AreEqual (set.Tables [i].Rows.Count, copySet.Tables [i].Rows.Count, "#A10");
                                Assert.AreEqual (set.Tables [i].Columns.Count, copySet.Tables [i].Columns.Count, "#A11");
                        }
                        //Testing Clone ()
                        copySet = set.Clone ();
                        Assert.AreEqual (set.CaseSensitive, copySet.CaseSensitive, "#A12");
                        Assert.AreEqual (set.DataSetName, copySet.DataSetName, "#A13");
                        Assert.AreEqual (set.EnforceConstraints, copySet.EnforceConstraints, "#A14");
                        Assert.IsFalse (copySet.HasErrors, "#A15");
                        Assert.AreEqual (set.Namespace, copySet.Namespace, "#A16");
                        Assert.AreEqual (set.Prefix, copySet.Prefix, "#A17");
                        Assert.AreEqual (set.Relations.Count, copySet.Relations.Count, "#A18");
                        Assert.AreEqual (set.Tables.Count, copySet.Tables.Count, "#A19");
                        Assert.AreEqual (set.ExtendedProperties ["TimeStamp"], copySet.ExtendedProperties ["TimeStamp"], "#A20");
                        for (int i = 0;i < copySet.Tables.Count; i++) {
                                Assert.AreEqual (0, copySet.Tables [i].Rows.Count, "#A21");
                                Assert.AreEqual (set.Tables [i].Columns.Count, copySet.Tables [i].Columns.Count, "#A22");
                        }
		}
		
		[Test]
		public void CloneCopy_TestForeignKeyConstraints ()
		{
			DataTable dirTable = new DataTable("Directories");

			DataColumn dir_UID = new DataColumn("UID", typeof(int));
			dir_UID.Unique = true;
			dir_UID.AllowDBNull = false;

			dirTable.Columns.Add(dir_UID);

			// Build a simple Files table
			DataTable fileTable = new DataTable("Files");

			DataColumn file_DirID = new DataColumn("DirectoryID", typeof(int));
			file_DirID.Unique = false;
			file_DirID.AllowDBNull = false;

			fileTable.Columns.Add(file_DirID);

			// Build the DataSet
			DataSet ds = new DataSet("TestDataset");
			ds.Tables.Add(dirTable);
			ds.Tables.Add(fileTable);

			// Add a foreign key constraint
			DataColumn[] parentColumns = new DataColumn[1];
			parentColumns[0] = ds.Tables["Directories"].Columns["UID"];

			DataColumn[] childColumns = new DataColumn[1];
			childColumns[0] = ds.Tables["Files"].Columns["DirectoryID"];

			ForeignKeyConstraint fk = new ForeignKeyConstraint("FK_Test", parentColumns, childColumns);
			ds.Tables["Files"].Constraints.Add(fk);		
			ds.EnforceConstraints = true;

			Assert.AreEqual (1, ds.Tables["Directories"].Constraints.Count, "#1");
			Assert.AreEqual (1, ds.Tables["Files"].Constraints.Count, "#2");

			// check clone works fine
			DataSet cloned_ds = ds.Clone ();
			Assert.AreEqual (1, cloned_ds.Tables["Directories"].Constraints.Count, "#3");
			Assert.AreEqual (1, cloned_ds.Tables["Files"].Constraints.Count, "#4");

			ForeignKeyConstraint clonedFk =  (ForeignKeyConstraint)cloned_ds.Tables["Files"].Constraints[0];
			Assert.AreEqual ("FK_Test", clonedFk.ConstraintName, "#5");
			Assert.AreEqual (1, clonedFk.Columns.Length, "#6");
			Assert.AreEqual ("DirectoryID", clonedFk.Columns[0].ColumnName, "#7");

			UniqueConstraint clonedUc = (UniqueConstraint)cloned_ds.Tables ["Directories"].Constraints[0];
			UniqueConstraint origUc = (UniqueConstraint)ds.Tables ["Directories"].Constraints[0];
			Assert.AreEqual (origUc.ConstraintName, clonedUc.ConstraintName, "#8");
			Assert.AreEqual (1, clonedUc.Columns.Length, "#9");
			Assert.AreEqual ("UID", clonedUc.Columns[0].ColumnName, "#10");

			// check copy works fine
			DataSet copy_ds = ds.Copy ();
			Assert.AreEqual (1, copy_ds.Tables["Directories"].Constraints.Count, "#11");
			Assert.AreEqual (1, copy_ds.Tables["Files"].Constraints.Count, "#12");

			ForeignKeyConstraint copyFk =  (ForeignKeyConstraint)copy_ds.Tables["Files"].Constraints[0];
			Assert.AreEqual ("FK_Test", copyFk.ConstraintName, "#13");
			Assert.AreEqual (1, copyFk.Columns.Length, "#14");
			Assert.AreEqual ("DirectoryID", copyFk.Columns[0].ColumnName, "#15");

			UniqueConstraint copyUc = (UniqueConstraint)copy_ds.Tables ["Directories"].Constraints[0];
			origUc = (UniqueConstraint)ds.Tables ["Directories"].Constraints[0];
			Assert.AreEqual (origUc.ConstraintName, copyUc.ConstraintName, "#16");
			Assert.AreEqual (1, copyUc.Columns.Length, "#17");
			Assert.AreEqual ("UID", copyUc.Columns[0].ColumnName, "#18");
		}

		/// <summary>
		/// Test for testing DataSet.Clear method with foriegn key relations
		/// This is expected to clear all the related datatable rows also
		/// </summary>
		[Test]
		public void DataSetClearTest ()
		{
		        DataSet ds = new DataSet ();
		        DataTable parent = ds.Tables.Add ("Parent");
		        DataTable child = ds.Tables.Add ("Child");
		        
		        parent.Columns.Add ("id", typeof (int));
		        child.Columns.Add ("ref_id", typeof(int));
		        
		        child.Constraints.Add (new ForeignKeyConstraint ("fk_constraint", parent.Columns [0], child.Columns [0]));
		        
		        DataRow dr = parent.NewRow ();
		        dr [0] = 1;
		        parent.Rows.Add (dr);
		        dr.AcceptChanges ();
		        
		        dr = child.NewRow ();
		        dr [0] = 1;
		        child.Rows.Add (dr);
		        dr.AcceptChanges ();
		        
		        try {
		                ds.Clear (); // this should clear all the rows in parent & child tables
		        } catch (Exception e) {
		                throw (new Exception ("Exception should not have been thrown at Clear method" + e.ToString ()));
		        }
		        Assert.AreEqual (0, parent.Rows.Count, "parent table rows should not exist!");
		        Assert.AreEqual (0, child.Rows.Count, "child table rows should not exist!");
		}

		[Test]
		public void CloneSubClassTest()
		{
			MyDataSet ds1 = new MyDataSet();
                        MyDataSet ds = (MyDataSet)(ds1.Clone());
                     	Assert.AreEqual (2, MyDataSet.count, "A#01");
		}

		#region DataSet.GetChanges Tests
		public void GetChanges_Relations_DifferentRowStatesTest ()
		{
			DataSet ds = new DataSet ("ds");
			DataTable parent = ds.Tables.Add ("parent");
			DataTable child = ds.Tables.Add ("child");
			
			parent.Columns.Add ("id", typeof (int));
			parent.Columns.Add ("name", typeof (string));
			

			child.Columns.Add ("id", typeof (int));
			child.Columns.Add ("parent", typeof (int));
			child.Columns.Add ("name", typeof (string));

			parent.Rows.Add (new object [] { 1, "mono parent 1" } );
			parent.Rows.Add (new object [] { 2, "mono parent 2" } );
			parent.Rows.Add (new object [] { 3, "mono parent 3" } );
			parent.Rows.Add (new object [] { 4, "mono parent 4" } );
			parent.AcceptChanges ();

			child.Rows.Add (new object [] { 1, 1, "mono child 1" } );
			child.Rows.Add (new object [] { 2, 2, "mono child 2" } );
			child.Rows.Add (new object [] { 3, 3, "mono child 3" } );
			child.AcceptChanges ();

			DataRelation relation = ds.Relations.Add ("parent_child", 
								  parent.Columns ["id"],
								  child.Columns ["parent"]);
			
			// modify the parent and get changes
			child.Rows [1]["parent"] = 4;
			DataSet changes = ds.GetChanges ();
			DataRow row = changes.Tables ["parent"].Rows[0];
			Assert.AreEqual ((int) parent.Rows [3][0], (int) row [0], "#RT1");
			Assert.AreEqual (1, changes.Tables ["parent"].Rows.Count, "#RT2 only get parent row with current version");
			ds.RejectChanges ();

			// delete a child row and get changes.
			child.Rows [0].Delete ();
			changes = ds.GetChanges ();
			
			Assert.AreEqual (changes.Tables.Count, 2, "#RT3 Should import parent table as well");
			Assert.AreEqual (1, changes.Tables ["parent"].Rows.Count, "#RT4 only get parent row with original version");
			Assert.AreEqual (1, (int) changes.Tables ["parent"].Rows [0][0], "#RT5 parent row based on original version");
		}
		#endregion // DataSet.GetChanges Tests

		[Test]
		public void RuleTest ()
		{
			DataSet ds = new DataSet ("testds");
			DataTable parent = ds.Tables.Add ("parent");
			DataTable child = ds.Tables.Add ("child");
			
			parent.Columns.Add ("id", typeof (int));
			parent.Columns.Add ("name", typeof (string));
			parent.PrimaryKey = new DataColumn [] {parent.Columns ["id"]} ;

			child.Columns.Add ("id", typeof (int));
			child.Columns.Add ("parent", typeof (int));
			child.Columns.Add ("name", typeof (string));
			child.PrimaryKey = new DataColumn [] {child.Columns ["id"]} ;

			DataRelation relation = ds.Relations.Add ("parent_child", 
								  parent.Columns ["id"],
								  child.Columns ["parent"]);

			parent.Rows.Add (new object [] {1, "mono test 1"});
			parent.Rows.Add (new object [] {2, "mono test 2"});
			parent.Rows.Add (new object [] {3, "mono test 3"});
			
			child.Rows.Add (new object [] {1, 1, "mono child test 1"});
			child.Rows.Add (new object [] {2, 2, "mono child test 2"});
			child.Rows.Add (new object [] {3, 3, "mono child test 3"});
			
			ds.AcceptChanges ();
			
			parent.Rows [0] ["name"] = "mono changed test 1";
			
			Assert.AreEqual (DataRowState.Unchanged, parent.Rows [0].GetChildRows (relation) [0].RowState,
					 "#RT1 child should not be modified");

			ds.RejectChanges ();
			parent.Rows [0] ["id"] = "4";

			DataRow childRow =  parent.Rows [0].GetChildRows (relation) [0];
			Assert.AreEqual (DataRowState.Modified, childRow.RowState, "#RT2 child should be modified");
			Assert.AreEqual (4, (int) childRow ["parent"], "#RT3 child should point to modified row");
		}
		
		#region DataSet.CreateDataReader Tests and DataSet.Load Tests

		private DataSet ds;
		private DataTable dt1, dt2;

		private void localSetup () {
			ds = new DataSet ("test");
			dt1 = new DataTable ("test1");
			dt1.Columns.Add ("id1", typeof (int));
			dt1.Columns.Add ("name1", typeof (string));
			//dt1.PrimaryKey = new DataColumn[] { dt1.Columns["id"] };
			dt1.Rows.Add (new object[] { 1, "mono 1" });
			dt1.Rows.Add (new object[] { 2, "mono 2" });
			dt1.Rows.Add (new object[] { 3, "mono 3" });
			dt1.AcceptChanges ();
			dt2 = new DataTable ("test2");
			dt2.Columns.Add ("id2", typeof (int));
			dt2.Columns.Add ("name2", typeof (string));
			dt2.Columns.Add ("name3", typeof (string));
			//dt2.PrimaryKey = new DataColumn[] { dt2.Columns["id"] };
			dt2.Rows.Add (new object[] { 4, "mono 4", "four" });
			dt2.Rows.Add (new object[] { 5, "mono 5", "five" });
			dt2.Rows.Add (new object[] { 6, "mono 6", "six" });
			dt2.AcceptChanges ();
			ds.Tables.Add (dt1);
			ds.Tables.Add (dt2);
			ds.AcceptChanges ();
		}
		/*
		//FIXME: we don't support DataTableReader yet
		[Test]
		public void CreateDataReader1 () {
			// For First CreateDataReader Overload
			localSetup ();
			DataTableReader dtr = ds.CreateDataReader ();
			Assert.IsTrue (dtr.HasRows, "HasRows");
			int ti = 0;
			do {
				Assert.AreEqual (ds.Tables[ti].Columns.Count, dtr.FieldCount, "CountCols-" + ti);
				int ri = 0;
				while (dtr.Read ()) {
					for (int i = 0; i < dtr.FieldCount; i++)
						Assert.AreEqual (ds.Tables[ti].Rows[ri][i], dtr[i], "RowData-"+ti+"-"+ri+"-"+i);
					ri++;
				}
				ti++;
			} while (dtr.NextResult ());
		}

		[Test]
		public void CreateDataReader2 () {
			// For Second CreateDataReader Overload -
			// compare to ds.Tables
			localSetup ();
			DataTableReader dtr = ds.CreateDataReader (dt1, dt2);
			Assert.IsTrue (dtr.HasRows, "HasRows");
			int ti = 0;
			do {
			        Assert.AreEqual (ds.Tables[ti].Columns.Count, dtr.FieldCount, "CountCols-" + ti);
			        int ri = 0;
			        while (dtr.Read ()) {
			                for (int i = 0; i < dtr.FieldCount; i++)
			                        Assert.AreEqual (ds.Tables[ti].Rows[ri][i], dtr[i], "RowData-" + ti + "-" + ri + "-" + i);
			                ri++;
			        }
			        ti++;
			} while (dtr.NextResult ());
		}

		[Test]
		public void CreateDataReader3 () {
			// For Second CreateDataReader Overload -
			// compare to dt1 and dt2
			localSetup ();
			ds.Tables.Clear ();
			DataTableReader dtr = ds.CreateDataReader (dt1, dt2);
			Assert.IsTrue (dtr.HasRows, "HasRows");
			string name = "dt1";
			DataTable dtn = dt1;
			do {
			        Assert.AreEqual (dtn.Columns.Count, dtr.FieldCount, "CountCols-" + name);
			        int ri = 0;
			        while (dtr.Read ()) {
			                for (int i = 0; i < dtr.FieldCount; i++)
			                        Assert.AreEqual (dtn.Rows[ri][i], dtr[i], "RowData-" + name + "-" + ri + "-" + i);
			                ri++;
			        }
				if (dtn == dt1) {
					dtn = dt2;
					name = "dt2";
				} else {
					dtn = null;
					name = null;
				}
			} while (dtr.NextResult ());
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void CreateDataReaderNoTable () {
			DataSet dsr = new DataSet ();
			DataTableReader dtr = dsr.CreateDataReader ();
		}
		*/
		internal struct fillErrorStruct {
			internal string error;
			internal string tableName;
			internal int rowKey;
			internal bool contFlag;
			internal void init (string tbl, int row, bool cont, string err) {
				tableName = tbl;
				rowKey = row;
				contFlag = cont;
				error = err;
			}
		}
		private fillErrorStruct[] fillErr = new fillErrorStruct[3];
		private int fillErrCounter;
		private void fillErrorHandler (object sender, FillErrorEventArgs e) {
			e.Continue = fillErr[fillErrCounter].contFlag;
			Assert.AreEqual (fillErr[fillErrCounter].tableName, e.DataTable.TableName, "fillErr-T");
			Assert.AreEqual (fillErr[fillErrCounter].contFlag, e.Continue, "fillErr-C");
			fillErrCounter++;
		}
		
		/*
		//FIXME: we don't support DataTableReader yet
		[Test]
		public void Load_Basic () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadBasic");
			DataTable table1 = new DataTable ();
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ();
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.OverwriteChanges, table1, table2);
			CompareTables (dsLoad);
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Load_TableUnknown () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadTableUnknown");
			DataTable table1 = new DataTable ();
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ();
			// table2 is not added to dsLoad [dsLoad.Tables.Add (table2);]
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.OverwriteChanges, table1, table2);
		}

		[Test]
		public void Load_TableConflictT () {
			fillErrCounter = 0;
			fillErr[0].init ("Table1", 1, true,
				"Input string was not in a correct format.Couldn't store <mono 1> in name1 Column.  Expected type is Double.");
			fillErr[1].init ("Table1", 2, true,
				"Input string was not in a correct format.Couldn't store <mono 2> in name1 Column.  Expected type is Double.");
			fillErr[2].init ("Table1", 3, true,
				"Input string was not in a correct format.Couldn't store <mono 3> in name1 Column.  Expected type is Double.");
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadTableConflict");
			DataTable table1 = new DataTable ();
			table1.Columns.Add ("name1", typeof (double));
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ();
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.PreserveChanges,
				     fillErrorHandler, table1, table2);
		}
		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Load_TableConflictF () {
			fillErrCounter = 0;
			fillErr[0].init ("Table1", 1, false,
				"Input string was not in a correct format.Couldn't store <mono 1> in name1 Column.  Expected type is Double.");
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadTableConflict");
			DataTable table1 = new DataTable ();
			table1.Columns.Add ("name1", typeof (double));
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ();
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.Upsert,
				     fillErrorHandler, table1, table2);
		}

		[Test]
		public void Load_StringsAsc () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadStrings");
			DataTable table1 = new DataTable ("First");
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ("Second");
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.OverwriteChanges, "First", "Second");
			CompareTables (dsLoad);
		}

		[Test]
		public void Load_StringsDesc () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadStrings");
			DataTable table1 = new DataTable ("First");
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ("Second");
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.PreserveChanges, "Second", "First");
			Assert.AreEqual (2, dsLoad.Tables.Count, "Tables");
			Assert.AreEqual (3, dsLoad.Tables[0].Rows.Count, "T1-Rows");
			Assert.AreEqual (3, dsLoad.Tables[0].Columns.Count, "T1-Columns");
			Assert.AreEqual (3, dsLoad.Tables[1].Rows.Count, "T2-Rows");
			Assert.AreEqual (2, dsLoad.Tables[1].Columns.Count, "T2-Columns");
		}

		[Test]
		public void Load_StringsNew () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadStrings");
			DataTable table1 = new DataTable ("First");
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ("Second");
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.Upsert, "Third", "Fourth");
			Assert.AreEqual (4, dsLoad.Tables.Count, "Tables");
			Assert.AreEqual ("First", dsLoad.Tables[0].TableName, "T1-Name");
			Assert.AreEqual (0, dsLoad.Tables[0].Rows.Count, "T1-Rows");
			Assert.AreEqual (0, dsLoad.Tables[0].Columns.Count, "T1-Columns");
			Assert.AreEqual ("Second", dsLoad.Tables[1].TableName, "T2-Name");
			Assert.AreEqual (0, dsLoad.Tables[1].Rows.Count, "T2-Rows");
			Assert.AreEqual (0, dsLoad.Tables[1].Columns.Count, "T2-Columns");
			Assert.AreEqual ("Third", dsLoad.Tables[2].TableName, "T3-Name");
			Assert.AreEqual (3, dsLoad.Tables[2].Rows.Count, "T3-Rows");
			Assert.AreEqual (2, dsLoad.Tables[2].Columns.Count, "T3-Columns");
			Assert.AreEqual ("Fourth", dsLoad.Tables[3].TableName, "T4-Name");
			Assert.AreEqual (3, dsLoad.Tables[3].Rows.Count, "T4-Rows");
			Assert.AreEqual (3, dsLoad.Tables[3].Columns.Count, "T4-Columns");
		}

		[Test]
		public void Load_StringsNewMerge () {
			localSetup ();
			DataSet dsLoad = new DataSet ("LoadStrings");
			DataTable table1 = new DataTable ("First");
			table1.Columns.Add ("col1", typeof (string));
			table1.Rows.Add (new object[] { "T1Row1" });
			dsLoad.Tables.Add (table1);
			DataTable table2 = new DataTable ("Second");
			table2.Columns.Add ("col2", typeof (string));
			table2.Rows.Add (new object[] { "T2Row1" });
			table2.Rows.Add (new object[] { "T2Row2" });
			dsLoad.Tables.Add (table2);
			DataTableReader dtr = ds.CreateDataReader ();
			dsLoad.Load (dtr, LoadOption.OverwriteChanges, "Third", "First");
			Assert.AreEqual (3, dsLoad.Tables.Count, "Tables");
			Assert.AreEqual ("First", dsLoad.Tables[0].TableName, "T1-Name");
			Assert.AreEqual (4, dsLoad.Tables[0].Rows.Count, "T1-Rows");
			Assert.AreEqual (4, dsLoad.Tables[0].Columns.Count, "T1-Columns");
			Assert.AreEqual ("Second", dsLoad.Tables[1].TableName, "T2-Name");
			Assert.AreEqual (2, dsLoad.Tables[1].Rows.Count, "T2-Rows");
			Assert.AreEqual (1, dsLoad.Tables[1].Columns.Count, "T2-Columns");
			Assert.AreEqual ("Third", dsLoad.Tables[2].TableName, "T3-Name");
			Assert.AreEqual (3, dsLoad.Tables[2].Rows.Count, "T3-Rows");
			Assert.AreEqual (2, dsLoad.Tables[2].Columns.Count, "T3-Columns");
		}
		*/
		private void CompareTables (DataSet dsLoad) {
			Assert.AreEqual (ds.Tables.Count, dsLoad.Tables.Count, "NumTables");
			for (int tc = 0; tc < dsLoad.Tables.Count; tc++) {
				Assert.AreEqual (ds.Tables[tc].Columns.Count, dsLoad.Tables[tc].Columns.Count, "Table" + tc + "-NumCols");
				Assert.AreEqual (ds.Tables[tc].Rows.Count, dsLoad.Tables[tc].Rows.Count, "Table" + tc + "-NumRows");
				for (int cc = 0; cc < dsLoad.Tables[tc].Columns.Count; cc++) {
					Assert.AreEqual (ds.Tables[tc].Columns[cc].ColumnName,
							 dsLoad.Tables[tc].Columns[cc].ColumnName,
							 "Table" + tc + "-" + "Col" + cc + "-Name");
				}
				for (int rc = 0; rc < dsLoad.Tables[tc].Rows.Count; rc++) {
					for (int cc = 0; cc < dsLoad.Tables[tc].Columns.Count; cc++) {
						Assert.AreEqual (ds.Tables[tc].Rows[rc].ItemArray[cc],
								 dsLoad.Tables[tc].Rows[rc].ItemArray[cc],
								 "Table" + tc + "-Row" + rc + "-Col" + cc + "-Data");
					}
				}
			}
		}

		#endregion // DataSet.CreateDataReader Tests and DataSet.Load Tests
	}

	 public  class MyDataSet:DataSet {

	     public static int count = 0;
                                                                                                    
             public MyDataSet() {

                    count++;
             }
                                                                                                    
         }
	

}
