//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Data
{
    public class DataRowCollection : List<DataRow>
    {
        public void Add(IEnumerable<object> cells)
        {
            Add(new DataRow(cells));
        }
    }
}