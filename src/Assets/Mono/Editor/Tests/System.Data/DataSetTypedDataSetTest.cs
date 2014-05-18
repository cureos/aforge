// Authors:
//   Rafael Mizrahi   <rafim@mainsoft.com>
//   Erez Lotan       <erezl@mainsoft.com>
//   Oren Gurfinkel   <oreng@mainsoft.com>
//   Ofer Borstein
//
// Copyright (c) 2004 Mainsoft Co.
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
using System.ComponentModel;
using System.Data;
using MonoTests.System.Data.Utils;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Globalization;

namespace MonoTests.System.Data
{
	[TestFixture]
	public class DataSetTypedDataSetTest
	{
		private string EventStatus = string.Empty ;

		protected void T_Changing(object sender, myTypedDataSet.OrdersRowChangeEvent e) 
		{ 
			EventStatus += "A";		
		}

		protected void T_Changed(object sender, myTypedDataSet.OrdersRowChangeEvent e) 
		{ 
			EventStatus += "B";
		}

		protected void T_Deleting(object sender, myTypedDataSet.OrdersRowChangeEvent e) 
		{ 
			EventStatus += "A";
		}

		protected void T_Deleted(object sender, myTypedDataSet.OrdersRowChangeEvent e) 
		{ 
			EventStatus += "B";
		}

		[Serializable()]
		[DesignerCategoryAttribute("code")]
		//[ToolboxItem(true)]
		public class myTypedDataSet : DataSet 
		{        
			private Order_DetailsDataTable tableOrder_Details;
        
			private OrdersDataTable tableOrders;
        
			private DataRelation relationOrdersOrder_x0020_Details;
        
			public myTypedDataSet() 
			{
				this.InitClass();
				CollectionChangeEventHandlerDerived schemaChangedHandler = new CollectionChangeEventHandlerDerived(this.SchemaChanged);
				this.Tables.CollectionChanged += schemaChangedHandler;
				this.Relations.CollectionChanged += schemaChangedHandler;
			}
        
			[Browsable(false)]
			//[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
			public Order_DetailsDataTable Order_Details 
			{
				get 
				{
					return this.tableOrder_Details;
				}
			}
        
			[Browsable(false)]
			//[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
			public OrdersDataTable Orders 
			{
				get 
				{
					return this.tableOrders;
				}
			}
        
			public DataSet Clone() 
			{
				myTypedDataSet cln = ((myTypedDataSet)(base.Clone()));
				cln.InitVars();
				return cln;
			}
        
			protected bool ShouldSerializeTables() 
			{
				return false;
			}
        
			protected bool ShouldSerializeRelations() 
			{
				return false;
			}
        
			internal void InitVars() 
			{
				this.tableOrder_Details = ((Order_DetailsDataTable)(this.Tables["Order Details"]));
				if ((this.tableOrder_Details != null)) 
				{
					this.tableOrder_Details.InitVars();
				}
				this.tableOrders = ((OrdersDataTable)(this.Tables["Orders"]));
				if ((this.tableOrders != null)) 
				{
					this.tableOrders.InitVars();
				}
				this.relationOrdersOrder_x0020_Details = this.Relations["OrdersOrder_x0020_Details"];
			}
        
			private void InitClass() 
			{
				this.DataSetName = "myTypedDataSet";
				this.Prefix = "";
				this.Namespace = "http://www.tempuri.org/myTypedDataSet.xsd";
				this.Locale = new CultureInfo("en-US");
				this.CaseSensitive = false;
				this.EnforceConstraints = true;
				this.tableOrder_Details = new Order_DetailsDataTable();
				this.Tables.Add(this.tableOrder_Details);
				this.tableOrders = new OrdersDataTable();
				this.Tables.Add(this.tableOrders);
				ForeignKeyConstraint fkc;
				fkc = new ForeignKeyConstraint("OrdersOrder_x0020_Details", new DataColumn[] {
																								 this.tableOrders.OrderIDColumn}, new DataColumn[] {
																																					   this.tableOrder_Details.OrderIDColumn});
				this.tableOrder_Details.Constraints.Add(fkc);
				fkc.AcceptRejectRule = AcceptRejectRule.None;
				fkc.DeleteRule = Rule.Cascade;
				fkc.UpdateRule = Rule.Cascade;
				this.relationOrdersOrder_x0020_Details = new DataRelation("OrdersOrder_x0020_Details", new DataColumn[] {
																															this.tableOrders.OrderIDColumn}, new DataColumn[] {
																																												  this.tableOrder_Details.OrderIDColumn}, false);
				this.Relations.Add(this.relationOrdersOrder_x0020_Details);
			}
        
			private bool ShouldSerializeOrder_Details() 
			{
				return false;
			}
        
			private bool ShouldSerializeOrders() 
			{
				return false;
			}
        
			private void SchemaChanged(object sender, CollectionChangeEventArgsDerived e) 
			{
				if ((e.Action == CollectionChangeActionDerived.Remove)) 
				{
					this.InitVars();
				}
			}
        
			public delegate void Order_DetailsRowChangeEventHandler(object sender, Order_DetailsRowChangeEvent e);
        
			public delegate void OrdersRowChangeEventHandler(object sender, OrdersRowChangeEvent e);
        
				public class Order_DetailsDataTable : DataTable, IEnumerable 
			{
            
				private DataColumn columnOrderID;
            
				private DataColumn columnProductID;
            
				private DataColumn columnUnitPrice;
            
				private DataColumn columnQuantity;
            
				private DataColumn columnDiscount;
            
				internal Order_DetailsDataTable() : 
					base("Order Details") 
				{
					this.InitClass();
				}
            
				internal Order_DetailsDataTable(DataTable table) : 
					base(table.TableName) 
				{
					if ((table.CaseSensitive != table.DataSet.CaseSensitive)) 
					{
						this.CaseSensitive = table.CaseSensitive;
					}
					if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) 
					{
						this.Locale = table.Locale;
					}
					if ((table.Namespace != table.DataSet.Namespace)) 
					{
						this.Namespace = table.Namespace;
					}
					this.Prefix = table.Prefix;
					this.MinimumCapacity = table.MinimumCapacity;
					this.DisplayExpression = table.DisplayExpression;
				}
            
				[Browsable(false)]
				public int Count 
				{
					get 
					{
						return this.Rows.Count;
					}
				}
            
				internal DataColumn OrderIDColumn 
				{
					get 
					{
						return this.columnOrderID;
					}
				}
            
				internal DataColumn ProductIDColumn 
				{
					get 
					{
						return this.columnProductID;
					}
				}
            
				internal DataColumn UnitPriceColumn 
				{
					get 
					{
						return this.columnUnitPrice;
					}
				}
            
				internal DataColumn QuantityColumn 
				{
					get 
					{
						return this.columnQuantity;
					}
				}
            
				internal DataColumn DiscountColumn 
				{
					get 
					{
						return this.columnDiscount;
					}
				}
            
				public Order_DetailsRow this[int index] 
				{
					get 
					{
						return ((Order_DetailsRow)(this.Rows[index]));
					}
				}
            
				public event Order_DetailsRowChangeEventHandler Order_DetailsRowChanged;
            
				public event Order_DetailsRowChangeEventHandler Order_DetailsRowChanging;
            
				public event Order_DetailsRowChangeEventHandler Order_DetailsRowDeleted;
            
				public event Order_DetailsRowChangeEventHandler Order_DetailsRowDeleting;
            
				public void AddOrder_DetailsRow(Order_DetailsRow row) 
				{
					this.Rows.Add(row);
				}
            
				public Order_DetailsRow AddOrder_DetailsRow(OrdersRow parentOrdersRowByOrdersOrder_x0020_Details, int ProductID, Decimal UnitPrice, short Quantity, string Discount) 
				{
					Order_DetailsRow rowOrder_DetailsRow = ((Order_DetailsRow)(this.NewRow()));
					rowOrder_DetailsRow.ItemArray = new object[] {
																	 parentOrdersRowByOrdersOrder_x0020_Details[0],
																	 ProductID,
																	 UnitPrice,
																	 Quantity,
																	 Discount};
					this.Rows.Add(rowOrder_DetailsRow);
					return rowOrder_DetailsRow;
				}
            
				public Order_DetailsRow FindByOrderIDProductID(int OrderID, int ProductID) 
				{
					return ((Order_DetailsRow)(this.Rows.Find(new object[] {
																			   OrderID,
																			   ProductID})));
				}
            
				public IEnumerator GetEnumerator() 
				{
					return this.Rows.GetEnumerator();
				}
            
				public DataTable Clone() 
				{
					Order_DetailsDataTable cln = ((Order_DetailsDataTable)(base.Clone()));
					cln.InitVars();
					return cln;
				}
            
				protected DataTable CreateInstance() 
				{
					return new Order_DetailsDataTable();
				}
            
				internal void InitVars() 
				{
					this.columnOrderID = this.Columns["OrderID"];
					this.columnProductID = this.Columns["ProductID"];
					this.columnUnitPrice = this.Columns["UnitPrice"];
					this.columnQuantity = this.Columns["Quantity"];
					this.columnDiscount = this.Columns["Discount"];
				}
            
				private void InitClass() 
				{
					this.columnOrderID = new DataColumn("OrderID", typeof(int), null, MappingType.Element);
					this.Columns.Add(this.columnOrderID);
					this.columnProductID = new DataColumn("ProductID", typeof(int), null, MappingType.Element);
					this.Columns.Add(this.columnProductID);
					this.columnUnitPrice = new DataColumn("UnitPrice", typeof(Decimal), null, MappingType.Element);
					this.Columns.Add(this.columnUnitPrice);
					this.columnQuantity = new DataColumn("Quantity", typeof(short), null, MappingType.Element);
					this.Columns.Add(this.columnQuantity);
					this.columnDiscount = new DataColumn("Discount", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnDiscount);
					this.Constraints.Add(new UniqueConstraint("Constraint1", new DataColumn[] {
																								  this.columnOrderID,
																								  this.columnProductID}, true));
					this.columnOrderID.AllowDBNull = false;
					this.columnProductID.AllowDBNull = false;
					this.columnUnitPrice.AllowDBNull = false;
					this.columnQuantity.AllowDBNull = false;
					this.columnDiscount.ReadOnly = true;
				}
            
				public Order_DetailsRow NewOrder_DetailsRow() 
				{
					return ((Order_DetailsRow)(this.NewRow()));
				}
            
				protected DataRow NewRowFromBuilder(DataRowBuilder builder) 
				{
					return new Order_DetailsRow(builder);
				}
            
				protected Type GetRowType() 
				{
					return typeof(Order_DetailsRow);
				}
            
				protected void OnRowChanged(DataRowChangeEventArgs e) 
				{
					base.OnRowChanged(e);
					if ((this.Order_DetailsRowChanged != null)) 
					{
						this.Order_DetailsRowChanged(this, new Order_DetailsRowChangeEvent(((Order_DetailsRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowChanging(DataRowChangeEventArgs e) 
				{
					base.OnRowChanging(e);
					if ((this.Order_DetailsRowChanging != null)) 
					{
						this.Order_DetailsRowChanging(this, new Order_DetailsRowChangeEvent(((Order_DetailsRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowDeleted(DataRowChangeEventArgs e) 
				{
					base.OnRowDeleted(e);
					if ((this.Order_DetailsRowDeleted != null)) 
					{
						this.Order_DetailsRowDeleted(this, new Order_DetailsRowChangeEvent(((Order_DetailsRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowDeleting(DataRowChangeEventArgs e) 
				{
					base.OnRowDeleting(e);
					if ((this.Order_DetailsRowDeleting != null)) 
					{
						this.Order_DetailsRowDeleting(this, new Order_DetailsRowChangeEvent(((Order_DetailsRow)(e.Row)), e.Action));
					}
				}
            
				public void RemoveOrder_DetailsRow(Order_DetailsRow row) 
				{
					this.Rows.Remove(row);
				}
			}
        
				public class Order_DetailsRow : DataRow 
			{
            
				private Order_DetailsDataTable tableOrder_Details;
            
				internal Order_DetailsRow(DataRowBuilder rb) : 
					base(rb) 
				{
					this.tableOrder_Details = ((Order_DetailsDataTable)(this.Table));
				}
            
				public int OrderID 
				{
					get 
					{
						return ((int)(this[this.tableOrder_Details.OrderIDColumn]));
					}
					set 
					{
						this[this.tableOrder_Details.OrderIDColumn] = value;
					}
				}
            
				public int ProductID 
				{
					get 
					{
						return ((int)(this[this.tableOrder_Details.ProductIDColumn]));
					}
					set 
					{
						this[this.tableOrder_Details.ProductIDColumn] = value;
					}
				}
            
				public Decimal UnitPrice 
				{
					get 
					{
						return ((Decimal)(this[this.tableOrder_Details.UnitPriceColumn]));
					}
					set 
					{
						this[this.tableOrder_Details.UnitPriceColumn] = value;
					}
				}
            
				public short Quantity 
				{
					get 
					{
						return ((short)(this[this.tableOrder_Details.QuantityColumn]));
					}
					set 
					{
						this[this.tableOrder_Details.QuantityColumn] = value;
					}
				}
            
				public string Discount 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrder_Details.DiscountColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrder_Details.DiscountColumn] = value;
					}
				}
            
				public OrdersRow OrdersRow 
				{
					get 
					{
						return ((OrdersRow)(this.GetParentRow(this.Table.ParentRelations["OrdersOrder_x0020_Details"])));
					}
					set 
					{
						this.SetParentRow(value, this.Table.ParentRelations["OrdersOrder_x0020_Details"]);
					}
				}
            
				public bool IsDiscountNull() 
				{
					return this.IsNull(this.tableOrder_Details.DiscountColumn);
				}
            
				public void SetDiscountNull() 
				{
					this[this.tableOrder_Details.DiscountColumn] = Convert.DBNull;
				}
			}
        
				public class Order_DetailsRowChangeEvent : EventArgs 
			{
            
				private Order_DetailsRow eventRow;
            
				private DataRowAction eventAction;
            
				public Order_DetailsRowChangeEvent(Order_DetailsRow row, DataRowAction action) 
				{
					this.eventRow = row;
					this.eventAction = action;
				}
            
				public Order_DetailsRow Row 
				{
					get 
					{
						return this.eventRow;
					}
				}
            
				public DataRowAction Action 
				{
					get 
					{
						return this.eventAction;
					}
				}
			}
        
				public class OrdersDataTable : DataTable, IEnumerable 
			{
            
				private DataColumn columnOrderID;
            
				private DataColumn columnCustomerID;
            
				private DataColumn columnEmployeeID;
            
				private DataColumn columnOrderDate;
            
				private DataColumn columnRequiredDate;
            
				private DataColumn columnShippedDate;
            
				private DataColumn columnShipVia;
            
				private DataColumn columnFreight;
            
				private DataColumn columnShipName;
            
				private DataColumn columnShipAddress;
            
				private DataColumn columnShipCity;
            
				private DataColumn columnShipRegion;
            
				private DataColumn columnShipPostalCode;
            
				private DataColumn columnShipCountry;
            
				internal OrdersDataTable() : 
					base("Orders") 
				{
					this.InitClass();
				}
            
				internal OrdersDataTable(DataTable table) : 
					base(table.TableName) 
				{
					if ((table.CaseSensitive != table.DataSet.CaseSensitive)) 
					{
						this.CaseSensitive = table.CaseSensitive;
					}
					if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) 
					{
						this.Locale = table.Locale;
					}
					if ((table.Namespace != table.DataSet.Namespace)) 
					{
						this.Namespace = table.Namespace;
					}
					this.Prefix = table.Prefix;
					this.MinimumCapacity = table.MinimumCapacity;
					this.DisplayExpression = table.DisplayExpression;
				}
            
				[Browsable(false)]
				public int Count 
				{
					get 
					{
						return this.Rows.Count;
					}
				}
            
				internal DataColumn OrderIDColumn 
				{
					get 
					{
						return this.columnOrderID;
					}
				}
            
				internal DataColumn CustomerIDColumn 
				{
					get 
					{
						return this.columnCustomerID;
					}
				}
            
				internal DataColumn EmployeeIDColumn 
				{
					get 
					{
						return this.columnEmployeeID;
					}
				}
            
				internal DataColumn OrderDateColumn 
				{
					get 
					{
						return this.columnOrderDate;
					}
				}
            
				internal DataColumn RequiredDateColumn 
				{
					get 
					{
						return this.columnRequiredDate;
					}
				}
            
				internal DataColumn ShippedDateColumn 
				{
					get 
					{
						return this.columnShippedDate;
					}
				}
            
				internal DataColumn ShipViaColumn 
				{
					get 
					{
						return this.columnShipVia;
					}
				}
            
				internal DataColumn FreightColumn 
				{
					get 
					{
						return this.columnFreight;
					}
				}
            
				internal DataColumn ShipNameColumn 
				{
					get 
					{
						return this.columnShipName;
					}
				}
            
				internal DataColumn ShipAddressColumn 
				{
					get 
					{
						return this.columnShipAddress;
					}
				}
            
				internal DataColumn ShipCityColumn 
				{
					get 
					{
						return this.columnShipCity;
					}
				}
            
				internal DataColumn ShipRegionColumn 
				{
					get 
					{
						return this.columnShipRegion;
					}
				}
            
				internal DataColumn ShipPostalCodeColumn 
				{
					get 
					{
						return this.columnShipPostalCode;
					}
				}
            
				internal DataColumn ShipCountryColumn 
				{
					get 
					{
						return this.columnShipCountry;
					}
				}
            
				public OrdersRow this[int index] 
				{
					get 
					{
						return ((OrdersRow)(this.Rows[index]));
					}
				}
            
				public event OrdersRowChangeEventHandler OrdersRowChanged;
            
				public event OrdersRowChangeEventHandler OrdersRowChanging;
            
				public event OrdersRowChangeEventHandler OrdersRowDeleted;
            
				public event OrdersRowChangeEventHandler OrdersRowDeleting;
            
				public void AddOrdersRow(OrdersRow row) 
				{
					this.Rows.Add(row);
				}
            
				public OrdersRow AddOrdersRow(string CustomerID, int EmployeeID, DateTime OrderDate, DateTime RequiredDate, DateTime ShippedDate, int ShipVia, Decimal Freight, string ShipName, string ShipAddress, string ShipCity, string ShipRegion, string ShipPostalCode, string ShipCountry) 
				{
					OrdersRow rowOrdersRow = ((OrdersRow)(this.NewRow()));
					rowOrdersRow.ItemArray = new object[] {
															  null,
															  CustomerID,
															  EmployeeID,
															  OrderDate,
															  RequiredDate,
															  ShippedDate,
															  ShipVia,
															  Freight,
															  ShipName,
															  ShipAddress,
															  ShipCity,
															  ShipRegion,
															  ShipPostalCode,
															  ShipCountry};
					this.Rows.Add(rowOrdersRow);
					return rowOrdersRow;
				}
            
				public OrdersRow FindByOrderID(int OrderID) 
				{
					return ((OrdersRow)(this.Rows.Find(new object[] {
																		OrderID})));
				}
            
				public IEnumerator GetEnumerator() 
				{
					return this.Rows.GetEnumerator();
				}
            
				public DataTable Clone() 
				{
					OrdersDataTable cln = ((OrdersDataTable)(base.Clone()));
					cln.InitVars();
					return cln;
				}
            
				protected DataTable CreateInstance() 
				{
					return new OrdersDataTable();
				}
            
				internal void InitVars() 
				{
					this.columnOrderID = this.Columns["OrderID"];
					this.columnCustomerID = this.Columns["CustomerID"];
					this.columnEmployeeID = this.Columns["EmployeeID"];
					this.columnOrderDate = this.Columns["OrderDate"];
					this.columnRequiredDate = this.Columns["RequiredDate"];
					this.columnShippedDate = this.Columns["ShippedDate"];
					this.columnShipVia = this.Columns["ShipVia"];
					this.columnFreight = this.Columns["Freight"];
					this.columnShipName = this.Columns["ShipName"];
					this.columnShipAddress = this.Columns["ShipAddress"];
					this.columnShipCity = this.Columns["ShipCity"];
					this.columnShipRegion = this.Columns["ShipRegion"];
					this.columnShipPostalCode = this.Columns["ShipPostalCode"];
					this.columnShipCountry = this.Columns["ShipCountry"];
				}
            
				private void InitClass() 
				{
					this.columnOrderID = new DataColumn("OrderID", typeof(int), null, MappingType.Element);
					this.Columns.Add(this.columnOrderID);
					this.columnCustomerID = new DataColumn("CustomerID", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnCustomerID);
					this.columnEmployeeID = new DataColumn("EmployeeID", typeof(int), null, MappingType.Element);
					this.Columns.Add(this.columnEmployeeID);
					this.columnOrderDate = new DataColumn("OrderDate", typeof(DateTime), null, MappingType.Element);
					this.Columns.Add(this.columnOrderDate);
					this.columnRequiredDate = new DataColumn("RequiredDate", typeof(DateTime), null, MappingType.Element);
					this.Columns.Add(this.columnRequiredDate);
					this.columnShippedDate = new DataColumn("ShippedDate", typeof(DateTime), null, MappingType.Element);
					this.Columns.Add(this.columnShippedDate);
					this.columnShipVia = new DataColumn("ShipVia", typeof(int), null, MappingType.Element);
					this.Columns.Add(this.columnShipVia);
					this.columnFreight = new DataColumn("Freight", typeof(Decimal), null, MappingType.Element);
					this.Columns.Add(this.columnFreight);
					this.columnShipName = new DataColumn("ShipName", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipName);
					this.columnShipAddress = new DataColumn("ShipAddress", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipAddress);
					this.columnShipCity = new DataColumn("ShipCity", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipCity);
					this.columnShipRegion = new DataColumn("ShipRegion", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipRegion);
					this.columnShipPostalCode = new DataColumn("ShipPostalCode", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipPostalCode);
					this.columnShipCountry = new DataColumn("ShipCountry", typeof(string), null, MappingType.Element);
					this.Columns.Add(this.columnShipCountry);
					this.Constraints.Add(new UniqueConstraint("Constraint1", new DataColumn[] {
																								  this.columnOrderID}, true));
					this.columnOrderID.AutoIncrement = true;
					this.columnOrderID.AllowDBNull = false;
					this.columnOrderID.ReadOnly = true;
					this.columnOrderID.Unique = true;
				}
            
				public OrdersRow NewOrdersRow() 
				{
					return ((OrdersRow)(this.NewRow()));
				}
            
				protected DataRow NewRowFromBuilder(DataRowBuilder builder) 
				{
					return new OrdersRow(builder);
				}
            
				protected Type GetRowType() 
				{
					return typeof(OrdersRow);
				}
            
				protected void OnRowChanged(DataRowChangeEventArgs e) 
				{
					base.OnRowChanged(e);
					if ((this.OrdersRowChanged != null)) 
					{
						this.OrdersRowChanged(this, new OrdersRowChangeEvent(((OrdersRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowChanging(DataRowChangeEventArgs e) 
				{
					base.OnRowChanging(e);
					if ((this.OrdersRowChanging != null)) 
					{
						this.OrdersRowChanging(this, new OrdersRowChangeEvent(((OrdersRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowDeleted(DataRowChangeEventArgs e) 
				{
					base.OnRowDeleted(e);
					if ((this.OrdersRowDeleted != null)) 
					{
						this.OrdersRowDeleted(this, new OrdersRowChangeEvent(((OrdersRow)(e.Row)), e.Action));
					}
				}
            
				protected void OnRowDeleting(DataRowChangeEventArgs e) 
				{
					base.OnRowDeleting(e);
					if ((this.OrdersRowDeleting != null)) 
					{
						this.OrdersRowDeleting(this, new OrdersRowChangeEvent(((OrdersRow)(e.Row)), e.Action));
					}
				}
            
				public void RemoveOrdersRow(OrdersRow row) 
				{
					this.Rows.Remove(row);
				}
			}
        
				public class OrdersRow : DataRow 
			{
            
				private OrdersDataTable tableOrders;
            
				internal OrdersRow(DataRowBuilder rb) : 
					base(rb) 
				{
					this.tableOrders = ((OrdersDataTable)(this.Table));
				}
            
				public int OrderID 
				{
					get 
					{
						return ((int)(this[this.tableOrders.OrderIDColumn]));
					}
					set 
					{
						this[this.tableOrders.OrderIDColumn] = value;
					}
				}
            
				public string CustomerID 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.CustomerIDColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.CustomerIDColumn] = value;
					}
				}
            
				public int EmployeeID 
				{
					get 
					{
						try 
						{
							return ((int)(this[this.tableOrders.EmployeeIDColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.EmployeeIDColumn] = value;
					}
				}
            
				public DateTime OrderDate 
				{
					get 
					{
						try 
						{
							return ((DateTime)(this[this.tableOrders.OrderDateColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.OrderDateColumn] = value;
					}
				}
            
				public DateTime RequiredDate 
				{
					get 
					{
						try 
						{
							return ((DateTime)(this[this.tableOrders.RequiredDateColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.RequiredDateColumn] = value;
					}
				}
            
				public DateTime ShippedDate 
				{
					get 
					{
						try 
						{
							return ((DateTime)(this[this.tableOrders.ShippedDateColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShippedDateColumn] = value;
					}
				}
            
				public int ShipVia 
				{
					get 
					{
						try 
						{
							return ((int)(this[this.tableOrders.ShipViaColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipViaColumn] = value;
					}
				}
            
				public Decimal Freight 
				{
					get 
					{
						try 
						{
							return ((Decimal)(this[this.tableOrders.FreightColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.FreightColumn] = value;
					}
				}
            
				public string ShipName 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipNameColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipNameColumn] = value;
					}
				}
            
				public string ShipAddress 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipAddressColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipAddressColumn] = value;
					}
				}
            
				public string ShipCity 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipCityColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipCityColumn] = value;
					}
				}
            
				public string ShipRegion 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipRegionColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipRegionColumn] = value;
					}
				}
            
				public string ShipPostalCode 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipPostalCodeColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipPostalCodeColumn] = value;
					}
				}
            
				public string ShipCountry 
				{
					get 
					{
						try 
						{
							return ((string)(this[this.tableOrders.ShipCountryColumn]));
						}
						catch (InvalidCastException e) 
						{
							throw new StrongTypingException("Cannot get value because it is DBNull.", e);
						}
					}
					set 
					{
						this[this.tableOrders.ShipCountryColumn] = value;
					}
				}
            
				public bool IsCustomerIDNull() 
				{
					return this.IsNull(this.tableOrders.CustomerIDColumn);
				}
            
				public void SetCustomerIDNull() 
				{
					this[this.tableOrders.CustomerIDColumn] = Convert.DBNull;
				}
            
				public bool IsEmployeeIDNull() 
				{
					return this.IsNull(this.tableOrders.EmployeeIDColumn);
				}
            
				public void SetEmployeeIDNull() 
				{
					this[this.tableOrders.EmployeeIDColumn] = Convert.DBNull;
				}
            
				public bool IsOrderDateNull() 
				{
					return this.IsNull(this.tableOrders.OrderDateColumn);
				}
            
				public void SetOrderDateNull() 
				{
					this[this.tableOrders.OrderDateColumn] = Convert.DBNull;
				}
            
				public bool IsRequiredDateNull() 
				{
					return this.IsNull(this.tableOrders.RequiredDateColumn);
				}
            
				public void SetRequiredDateNull() 
				{
					this[this.tableOrders.RequiredDateColumn] = Convert.DBNull;
				}
            
				public bool IsShippedDateNull() 
				{
					return this.IsNull(this.tableOrders.ShippedDateColumn);
				}
            
				public void SetShippedDateNull() 
				{
					this[this.tableOrders.ShippedDateColumn] = Convert.DBNull;
				}
            
				public bool IsShipViaNull() 
				{
					return this.IsNull(this.tableOrders.ShipViaColumn);
				}
            
				public void SetShipViaNull() 
				{
					this[this.tableOrders.ShipViaColumn] = Convert.DBNull;
				}
            
				public bool IsFreightNull() 
				{
					return this.IsNull(this.tableOrders.FreightColumn);
				}
            
				public void SetFreightNull() 
				{
					this[this.tableOrders.FreightColumn] = Convert.DBNull;
				}
            
				public bool IsShipNameNull() 
				{
					return this.IsNull(this.tableOrders.ShipNameColumn);
				}
            
				public void SetShipNameNull() 
				{
					this[this.tableOrders.ShipNameColumn] = Convert.DBNull;
				}
            
				public bool IsShipAddressNull() 
				{
					return this.IsNull(this.tableOrders.ShipAddressColumn);
				}
            
				public void SetShipAddressNull() 
				{
					this[this.tableOrders.ShipAddressColumn] = Convert.DBNull;
				}
            
				public bool IsShipCityNull() 
				{
					return this.IsNull(this.tableOrders.ShipCityColumn);
				}
            
				public void SetShipCityNull() 
				{
					this[this.tableOrders.ShipCityColumn] = Convert.DBNull;
				}
            
				public bool IsShipRegionNull() 
				{
					return this.IsNull(this.tableOrders.ShipRegionColumn);
				}
            
				public void SetShipRegionNull() 
				{
					this[this.tableOrders.ShipRegionColumn] = Convert.DBNull;
				}
            
				public bool IsShipPostalCodeNull() 
				{
					return this.IsNull(this.tableOrders.ShipPostalCodeColumn);
				}
            
				public void SetShipPostalCodeNull() 
				{
					this[this.tableOrders.ShipPostalCodeColumn] = Convert.DBNull;
				}
            
				public bool IsShipCountryNull() 
				{
					return this.IsNull(this.tableOrders.ShipCountryColumn);
				}
            
				public void SetShipCountryNull() 
				{
					this[this.tableOrders.ShipCountryColumn] = Convert.DBNull;
				}
            
				public Order_DetailsRow[] GetOrder_DetailsRows() 
				{
					return ((Order_DetailsRow[])(this.GetChildRows(this.Table.ChildRelations["OrdersOrder_x0020_Details"])));
				}
			}
        
				public class OrdersRowChangeEvent : EventArgs 
			{
            
				private OrdersRow eventRow;
            
				private DataRowAction eventAction;
            
				public OrdersRowChangeEvent(OrdersRow row, DataRowAction action) 
				{
					this.eventRow = row;
					this.eventAction = action;
				}
            
				public OrdersRow Row 
				{
					get 
					{
						return this.eventRow;
					}
				}
            
				public DataRowAction Action 
				{
					get 
					{
						return this.eventAction;
					}
				}
			}
		}
	}
}
