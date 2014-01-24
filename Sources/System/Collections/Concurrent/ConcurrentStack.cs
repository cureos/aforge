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
	public sealed class ConcurrentStack<T> : IEnumerable<T>, ICollection
	{
		#region FIELDS

		private readonly Stack<T> _stack;
		private readonly object _syncRoot;
 
		#endregion

		#region CONSTRUCTORS

		public ConcurrentStack()
		{
			_stack = new Stack<T>();
			_syncRoot = new object();
		}

		#endregion
		
		#region PROPERTIES

		public int Count
		{
			get { return _stack.Count; }
		}

		public bool IsSynchronized
		{
			get { return true; }
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion

		#region METHODS

		public IEnumerator<T> GetEnumerator()
		{
			return _stack.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public T[] ToArray()
		{
			return _stack.ToArray();
		}

		public void Push(T item)
		{
			_stack.Push(item);
		}

		#endregion
	}
}