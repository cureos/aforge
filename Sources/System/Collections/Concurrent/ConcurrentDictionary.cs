//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	public class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory,
			Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue value;
			if (ContainsKey(key))
			{
				value = updateValueFactory(key, this[key]);
				this[key] = value;
			}
			else
			{
				value = addValueFactory(key);
				Add(key, value);
			}
			return value;
		}
	}
}