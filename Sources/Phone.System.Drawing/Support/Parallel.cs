using System.Collections.Generic;

namespace System.Threading.Tasks
{
    internal class Parallel
    {
        internal static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            foreach (var item in source) body(item);
        }
    }
}