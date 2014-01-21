//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

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