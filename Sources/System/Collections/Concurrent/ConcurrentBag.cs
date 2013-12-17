//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

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