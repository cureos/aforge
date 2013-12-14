//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.Data
{
    public class DataColumn
    {
        #region FIELDS

        private string _caption;
        
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