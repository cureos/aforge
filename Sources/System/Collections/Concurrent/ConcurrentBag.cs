/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public class ConcurrentBag<T> : List<T>
    {
        #region FIELDS

        private int _peekCounter = 0;

        #endregion

        #region METHODS

        public new bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public new bool RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public new int RemoveAll(Predicate<T> match)
        {
            throw new NotSupportedException();
        }

        public new void RemoveRange(int index, int count)
        {
            throw new NotSupportedException();
        }

        public bool TryPeek(out T result)
        {
            if (_peekCounter < Count)
            {
                result = this[_peekCounter++];
                return true;
            }

            result = default(T);
            return false;
        }

        #endregion
    }
}