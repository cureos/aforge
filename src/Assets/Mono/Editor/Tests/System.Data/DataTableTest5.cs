// Authors:
//   Nagappan A <anagappan@novell.com>
//
// Copyright (c) 2007 Novell, Inc
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
using System.Data;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace MonoTests.System.Data
{
	[TestFixture]
	public class DataTableTest5
	{
		string tempFile;
		DataSet dataSet;
		DataTable dummyTable;
		DataTable parentTable1;
		DataTable childTable;
		DataTable secondChildTable;

		[SetUp]
		public void SetUp ()
		{
			tempFile = Path.GetTempFileName ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (tempFile != null)
				File.Delete (tempFile);
		}
		
		private void MakeParentTable1 ()
		{
			// Create a new Table
			parentTable1 = new DataTable ("ParentTable");
			dataSet = new DataSet ("XmlDataSet");
			DataColumn column;
			DataRow row;

			// Create new DataColumn, set DataType,
			// ColumnName and add to Table.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "id";
			column.Unique = true;
			// Add the Column to the DataColumnCollection.
			parentTable1.Columns.Add (column);

			// Create second column
			column = new DataColumn ();
			column.DataType = typeof (string);
			column.ColumnName = "ParentItem";
			column.AutoIncrement = false;
			column.Caption = "ParentItem";
			column.Unique = false;
			// Add the column to the table
			parentTable1.Columns.Add (column);

			// Create third column.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "DepartmentID";
			column.Caption = "DepartmentID";
			// Add the column to the table.
			parentTable1.Columns.Add (column);

			// Make the ID column the primary key column.
			DataColumn [] PrimaryKeyColumns = new DataColumn [2];
			PrimaryKeyColumns [0] = parentTable1.Columns ["id"];
			PrimaryKeyColumns [1] = parentTable1.Columns ["DepartmentID"];
			parentTable1.PrimaryKey = PrimaryKeyColumns;

			dataSet.Tables.Add (parentTable1);

			// Create three new DataRow objects and add 
			// them to the DataTable
			for (int i = 0; i <= 2; i++) {
				row = parentTable1.NewRow ();
				row ["id"] = i + 1;
				row ["ParentItem"] = "ParentItem " + (i + 1);
				row ["DepartmentID"] = i + 1;
				parentTable1.Rows.Add (row);
			}
		}

		private void MakeDummyTable ()
		{
			// Create a new Table
			dataSet = new DataSet ();
			dummyTable = new DataTable ("DummyTable");
			DataColumn column;
			DataRow row;

			// Create new DataColumn, set DataType, 
			// ColumnName and add to Table.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "id";
			column.Unique = true;
			// Add the Column to the DataColumnCollection.
			dummyTable.Columns.Add (column);

			// Create second column
			column = new DataColumn ();
			column.DataType = typeof (string);
			column.ColumnName = "DummyItem";
			column.AutoIncrement = false;
			column.Caption = "DummyItem";
			column.Unique = false;
			// Add the column to the table
			dummyTable.Columns.Add (column);

			dataSet.Tables.Add (dummyTable);

			// Create three new DataRow objects and add 
			// them to the DataTable
			for (int i = 0; i <= 2; i++) {
				row = dummyTable.NewRow ();
				row ["id"] = i + 1;
				row ["DummyItem"] = "DummyItem " + (i + 1);
				dummyTable.Rows.Add (row);
			}

			DataRow row1 = dummyTable.Rows [1];
			dummyTable.AcceptChanges ();
			row1.BeginEdit ();
			row1 [1] = "Changed_DummyItem " + 2;
			row1.EndEdit ();
		}

		private void MakeChildTable ()
		{
			// Create a new Table
			childTable = new DataTable ("ChildTable");
			DataColumn column;
			DataRow row;

			// Create first column and add to the DataTable.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "ChildID";
			column.AutoIncrement = true;
			column.Caption = "ID";
			column.Unique = true;

			// Add the column to the DataColumnCollection
			childTable.Columns.Add (column);

			// Create second column
			column = new DataColumn ();
			column.DataType = typeof (string);
			column.ColumnName = "ChildItem";
			column.AutoIncrement = false;
			column.Caption = "ChildItem";
			column.Unique = false;
			childTable.Columns.Add (column);

			//Create third column
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "ParentID";
			column.AutoIncrement = false;
			column.Caption = "ParentID";
			column.Unique = false;
			childTable.Columns.Add (column);

			dataSet.Tables.Add (childTable);

			// Create three sets of DataRow objects, 
			// five rows each, and add to DataTable.
			for (int i = 0; i <= 1; i++) {
				row = childTable.NewRow ();
				row ["childID"] = i + 1;
				row ["ChildItem"] = "ChildItem " + (i + 1);
				row ["ParentID"] = 1;
				childTable.Rows.Add (row);
			}
			for (int i = 0; i <= 1; i++) {
				row = childTable.NewRow ();
				row ["childID"] = i + 5;
				row ["ChildItem"] = "ChildItem " + (i + 1);
				row ["ParentID"] = 2;
				childTable.Rows.Add (row);
			}
			for (int i = 0; i <= 1; i++) {
				row = childTable.NewRow ();
				row ["childID"] = i + 10;
				row ["ChildItem"] = "ChildItem " + (i + 1);
				row ["ParentID"] = 3;
				childTable.Rows.Add (row);
			}
		}

		private void MakeSecondChildTable ()
		{
			// Create a new Table
			secondChildTable = new DataTable ("SecondChildTable");
			DataColumn column;
			DataRow row;

			// Create first column and add to the DataTable.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "ChildID";
			column.AutoIncrement = true;
			column.Caption = "ID";
			column.ReadOnly = true;
			column.Unique = true;

			// Add the column to the DataColumnCollection.
			secondChildTable.Columns.Add (column);

			// Create second column.
			column = new DataColumn ();
			column.DataType = typeof (string);
			column.ColumnName = "ChildItem";
			column.AutoIncrement = false;
			column.Caption = "ChildItem";
			column.ReadOnly = false;
			column.Unique = false;
			secondChildTable.Columns.Add (column);

			//Create third column.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "ParentID";
			column.AutoIncrement = false;
			column.Caption = "ParentID";
			column.ReadOnly = false;
			column.Unique = false;
			secondChildTable.Columns.Add (column);

			//Create fourth column.
			column = new DataColumn ();
			column.DataType = typeof (int);
			column.ColumnName = "DepartmentID";
			column.Caption = "DepartmentID";
			column.Unique = false;
			secondChildTable.Columns.Add (column);

			dataSet.Tables.Add (secondChildTable);
			// Create three sets of DataRow objects, 
			// five rows each, and add to DataTable.
			for (int i = 0; i <= 1; i++) {
				row = secondChildTable.NewRow ();
				row ["childID"] = i + 1;
				row ["ChildItem"] = "SecondChildItem " + (i + 1);
				row ["ParentID"] = 1;
				row ["DepartmentID"] = 1;
				secondChildTable.Rows.Add (row);
			}
			for (int i = 0; i <= 1; i++) {
				row = secondChildTable.NewRow ();
				row ["childID"] = i + 5;
				row ["ChildItem"] = "SecondChildItem " + (i + 1);
				row ["ParentID"] = 2;
				row ["DepartmentID"] = 2;
				secondChildTable.Rows.Add (row);
			}
			for (int i = 0; i <= 1; i++) {
				row = secondChildTable.NewRow ();
				row ["childID"] = i + 10;
				row ["ChildItem"] = "SecondChildItem " + (i + 1);
				row ["ParentID"] = 3;
				row ["DepartmentID"] = 3;
				secondChildTable.Rows.Add (row);
			}
		}

		private void MakeDataRelation ()
		{
			DataColumn parentColumn = dataSet.Tables ["ParentTable"].Columns ["id"];
			DataColumn childColumn = dataSet.Tables ["ChildTable"].Columns ["ParentID"];
			DataRelation relation = new DataRelation ("ParentChild_Relation1", parentColumn, childColumn);
			dataSet.Tables ["ChildTable"].ParentRelations.Add (relation);

			DataColumn [] parentColumn1 = new DataColumn [2];
			DataColumn [] childColumn1 = new DataColumn [2];

			parentColumn1 [0] = dataSet.Tables ["ParentTable"].Columns ["id"];
			parentColumn1 [1] = dataSet.Tables ["ParentTable"].Columns ["DepartmentID"];

			childColumn1 [0] = dataSet.Tables ["SecondChildTable"].Columns ["ParentID"];
			childColumn1 [1] = dataSet.Tables ["SecondChildTable"].Columns ["DepartmentID"];

			DataRelation secondRelation = new DataRelation ("ParentChild_Relation2", parentColumn1, childColumn1);
			dataSet.Tables ["SecondChildTable"].ParentRelations.Add (secondRelation);
		}

		private void MakeDataRelation (DataTable dt)
		{
			DataColumn parentColumn = dt.Columns ["id"];
			DataColumn childColumn = dataSet.Tables ["ChildTable"].Columns ["ParentID"];
			DataRelation relation = new DataRelation ("ParentChild_Relation1", parentColumn, childColumn);
			dataSet.Tables ["ChildTable"].ParentRelations.Add (relation);

			DataColumn [] parentColumn1 = new DataColumn [2];
			DataColumn [] childColumn1 = new DataColumn [2];

			parentColumn1 [0] = dt.Columns ["id"];
			parentColumn1 [1] = dt.Columns ["DepartmentID"];

			childColumn1 [0] = dataSet.Tables ["SecondChildTable"].Columns ["ParentID"];
			childColumn1 [1] = dataSet.Tables ["SecondChildTable"].Columns ["DepartmentID"];

			DataRelation secondRelation = new DataRelation ("ParentChild_Relation2", parentColumn1, childColumn1);
			dataSet.Tables ["SecondChildTable"].ParentRelations.Add (secondRelation);
		}

		//Test properties of a table which does not belongs to a DataSet
		private void VerifyTableSchema (DataTable table, string tableName, DataSet ds)
		{
			//Test Schema 
			//Check Properties of Table
			Assert.AreEqual (string.Empty, table.Namespace, "#1");
			Assert.AreEqual (ds, table.DataSet, "#2");
			Assert.AreEqual (3, table.Columns.Count, "#3");
			Assert.AreEqual (false, table.CaseSensitive, "#5");
			Assert.AreEqual (tableName, table.TableName, "#6");
			Assert.AreEqual (2, table.Constraints.Count, "#7");
			Assert.AreEqual (string.Empty, table.Prefix, "#8");
			Assert.AreEqual (2, table.Constraints .Count, "#10");
			Assert.AreEqual (typeof (UniqueConstraint), table.Constraints [0].GetType (), "#11");
			Assert.AreEqual (typeof (UniqueConstraint), table.Constraints [1].GetType (), "#12");
			Assert.AreEqual (2, table.PrimaryKey.Length, "#13");
			Assert.AreEqual ("id", table.PrimaryKey [0].ToString (), "#14");
			Assert.AreEqual ("DepartmentID", table.PrimaryKey [1].ToString (), "#15");
			Assert.AreEqual (0, table.ParentRelations.Count, "#16");
			Assert.AreEqual (0, table.ChildRelations.Count, "#17");

			//Check properties of each column
			//First Column
			DataColumn col = table.Columns [0];
			Assert.AreEqual (false, col.AllowDBNull, "#18");
			Assert.AreEqual (false, col.AutoIncrement, "#19");
			Assert.AreEqual (0, col.AutoIncrementSeed, "#20");
			Assert.AreEqual (1, col.AutoIncrementStep, "#21");
			Assert.AreEqual ("Element", col.ColumnMapping.ToString (), "#22");
			Assert.AreEqual ("id", col.Caption, "#23");
			Assert.AreEqual ("id", col.ColumnName, "#24");
			Assert.AreEqual (typeof (int), col.DataType, "#25");
			Assert.AreEqual (string.Empty, col.DefaultValue.ToString (), "#26");
			//Assert.AreEqual (false, col.DesignMode, "#27");
			Assert.AreEqual ("System.Data.PropertyCollection", col.ExtendedProperties.ToString (), "#28");
			Assert.AreEqual (-1, col.MaxLength, "#29");
			Assert.AreEqual (0, col.Ordinal, "#30");
			Assert.AreEqual (string.Empty, col.Prefix, "#31");
			Assert.AreEqual ("ParentTable", col.Table.ToString (), "#32");
			Assert.AreEqual (true, col.Unique, "#33");

			//Second Column
			col = table.Columns [1];
			Assert.AreEqual (true, col.AllowDBNull, "#34");
			Assert.AreEqual (false, col.AutoIncrement, "#35");
			Assert.AreEqual (0, col.AutoIncrementSeed, "#36");
			Assert.AreEqual (1, col.AutoIncrementStep, "#37");
			Assert.AreEqual ("Element", col.ColumnMapping.ToString (), "#38");
			Assert.AreEqual ("ParentItem", col.Caption, "#39");
			Assert.AreEqual ("ParentItem", col.ColumnName, "#40");
			Assert.AreEqual (typeof (string), col.DataType, "#41");
			Assert.AreEqual (string.Empty, col.DefaultValue.ToString (), "#42");
			//Assert.AreEqual (false, col.DesignMode, "#43");
			Assert.AreEqual ("System.Data.PropertyCollection", col.ExtendedProperties.ToString (), "#44");
			Assert.AreEqual (-1, col.MaxLength, "#45");
			Assert.AreEqual (1, col.Ordinal, "#46");
			Assert.AreEqual (string.Empty, col.Prefix, "#47");
			Assert.AreEqual ("ParentTable", col.Table.ToString (), "#48");
			Assert.AreEqual (false, col.Unique, "#49");

			//Third Column
			col = table.Columns [2];
			Assert.AreEqual (false, col.AllowDBNull, "#50");
			Assert.AreEqual (false, col.AutoIncrement, "#51");
			Assert.AreEqual (0, col.AutoIncrementSeed, "#52");
			Assert.AreEqual (1, col.AutoIncrementStep, "#53");
			Assert.AreEqual ("Element", col.ColumnMapping.ToString (), "#54");
			Assert.AreEqual ("DepartmentID", col.Caption, "#55");
			Assert.AreEqual ("DepartmentID", col.ColumnName, "#56");
			Assert.AreEqual (typeof (int), col.DataType, "#57");
			Assert.AreEqual (string.Empty, col.DefaultValue.ToString (), "#58");
			//Assert.AreEqual (false, col.DesignMode, "#59");
			Assert.AreEqual ("System.Data.PropertyCollection", col.ExtendedProperties.ToString (), "#60");
			Assert.AreEqual (-1, col.MaxLength, "#61");
			Assert.AreEqual (2, col.Ordinal, "#62");
			Assert.AreEqual (string.Empty, col.Prefix, "#63");
			Assert.AreEqual ("ParentTable", col.Table.ToString (), "#64");
			Assert.AreEqual (false, col.Unique, "#65");

			//Test the Xml
			Assert.AreEqual (3, table.Rows.Count, "#66");
			//Test values of each row
			DataRow row = table.Rows [0];
			Assert.AreEqual (1, row ["id"], "#67");
			Assert.AreEqual ("ParentItem 1", row ["ParentItem"], "#68");
			Assert.AreEqual (1, row ["DepartmentID"], "#69");

			row = table.Rows [1];
			Assert.AreEqual (2, row ["id"], "#70");
			Assert.AreEqual ("ParentItem 2", row ["ParentItem"], "#71");
			Assert.AreEqual (2, row ["DepartmentID"], "#72");

			row = table.Rows [2];
			Assert.AreEqual (3, row ["id"], "#73");
			Assert.AreEqual ("ParentItem 3", row ["ParentItem"], "#74");
			Assert.AreEqual (3, row ["DepartmentID"], "#75");
		}
	}
}
