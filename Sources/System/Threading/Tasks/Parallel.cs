//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Generic;

namespace System.Threading.Tasks
{
	public static class Parallel
	{
		#region METHODS

		public static void For(int fromInclusive, int toExclusive, Action<int> body)
		{
			for (var i = fromInclusive; i < toExclusive; ++i) body(i);
		}

	    public static void For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> body)
	    {
            for (var i = fromInclusive; i < toExclusive; ++i) body(i, new ParallelLoopState());
        }

	    public static void For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int> body)
		{
			for (var i = fromInclusive; i < toExclusive; ++i) body(i);
		}

		public static void For<TLocal>(int fromInclusive, int toExclusive, Func<TLocal> localInit,
			Func<int, object, TLocal, TLocal> body, Action<TLocal> localFinally)
		{
			for (var i = fromInclusive; i < toExclusive; ++i)
				localFinally(body(i, null, localInit()));
		}

		public static void For<TLocal>(int fromInclusive, int toExclusive, ParallelOptions parallelOptions,
			Func<TLocal> localInit, Func<int, object, TLocal, TLocal> body, Action<TLocal> localFinally)
		{
			for (var i = fromInclusive; i < toExclusive; ++i)
				localFinally(body(i, null, localInit()));
		}

		public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
		{
			foreach (var item in source) body(item);
		}
		
		#endregion
	}
}