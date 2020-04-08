using System;
using System.Collections.Generic;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var index = -1;
            foreach (var item in source)
            {
                index++;
                if (predicate(item))
                {
                    return index;
                }
            }

            return index;
        }
    }
}