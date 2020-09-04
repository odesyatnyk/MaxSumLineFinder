using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxSumLineFinder.UI.CommandLine
{
    public static class IEnumerableExtensions
    {
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return source.Aggregate(string.Empty, (current, next) => string.Concat(current, string.IsNullOrWhiteSpace(current) ? string.Empty : ", ", next.ToString()));
    }
    }
}
