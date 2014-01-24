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

namespace System.Threading.Tasks
{
	public static class Parallel
	{
		#region METHODS

		public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body)
		{
			for (var i = fromInclusive; i < toExclusive; ++i) body(i);
            return new ParallelLoopResult();
		}

        public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> body)
	    {
            for (var i = fromInclusive; i < toExclusive; ++i) body(i, new ParallelLoopState());
            return new ParallelLoopResult();
        }

        public static ParallelLoopResult For(int fromInclusive, int toExclusive, ParallelOptions parallelOptions, Action<int> body)
		{
			for (var i = fromInclusive; i < toExclusive; ++i) body(i);
            return new ParallelLoopResult();
        }

        public static ParallelLoopResult For<TLocal>(int fromInclusive, int toExclusive, Func<TLocal> localInit,
			Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
		{
			for (var i = fromInclusive; i < toExclusive; ++i)
				localFinally(body(i, null, localInit()));
            return new ParallelLoopResult();
        }

        public static ParallelLoopResult For<TLocal>(int fromInclusive, int toExclusive, ParallelOptions parallelOptions,
			Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
		{
			for (var i = fromInclusive; i < toExclusive; ++i)
				localFinally(body(i, null, localInit()));
            return new ParallelLoopResult();
        }

        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
		{
			foreach (var item in source) body(item);
            return new ParallelLoopResult();
        }
		
		#endregion
	}
}