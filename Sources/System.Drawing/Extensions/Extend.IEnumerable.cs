/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagePixelEnumerator.Extensions
{
    internal static partial class Extend
    {
        /// <summary>
        /// Selects distinct items by a given selector.
        /// </summary>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        {
            HashSet<TKey> keys = new HashSet<TKey>();
            return items.Where(item => keys.Add(selector(item)));
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            using (IEnumerator<T> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence was empty");

                T max = sourceIterator.Current;
                TKey maxKey = selector.Invoke(max);

                while (sourceIterator.MoveNext())
                {
                    T candidate = sourceIterator.Current;
                    TKey candidateProjected = selector.Invoke(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }
    }
}
