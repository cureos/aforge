//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Data
{
    public sealed class DataColumn
    {
        #region FIELDS

        private string _caption;
        
        #endregion

        #region CONSTRUCTORS

        internal DataColumn(DataTable table, string columnName, Type dataType)
        {
            Table = table;
            ColumnName = columnName;
            DataType = dataType;
        }

        #endregion

        #region PROPERTIES

        public DataTable Table { get; private set; }

        public Type DataType { get; set; }

        public string ColumnName { get; set; }

        public string Caption
        {
            get { return _caption ?? ColumnName; }
            set { _caption = value; }
        }

        #endregion
    }
}