//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Globalization;

namespace System.Data
{
    public class DataTable
    {
        #region PROPERTIES

        public DataColumnCollection Columns { get; private set; }

        public DataRowCollection Rows { get; private set; }

        public CultureInfo Locale { get; set; }

        public DataView DefaultView { get; private set; }

        #endregion

        #region METHODS

        public DataTable Clone()
        {
            throw new NotImplementedException();
        }

        public DataTable Copy()
        {
            throw new NotImplementedException();
        }

        public void ImportRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        public DataRow NewRow()
        {
            throw new NotImplementedException();
        }

        public object Compute(string p0, string empty)
        {
            throw new NotImplementedException();
        }

        public DataRow[] Select(string filter)
        {
            throw new NotImplementedException();
        }

        public DataRow[] Select(string expression, string orderBy)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}