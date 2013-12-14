//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Data
{
    public class DataRow : List<object>
    {
        #region FIELDS

        private readonly List<object> _cells;
 
        #endregion

        #region CONSTRUCTORS

        public DataRow(IEnumerable<object> cells)
        {
            _cells = new List<object>(cells);
        }

        #endregion

        #region INDEXERS

        public object this[string columnName]
        {
            get { return null; }
            set { }
        }

        public object this[DataColumn columnName]
        {
            get { return null; }
            set { }
        }

        #endregion

        #region PROPERTIES

        public DataTable Table { get; private set; }
        
        #endregion
    }
}