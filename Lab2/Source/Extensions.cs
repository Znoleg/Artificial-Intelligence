using System.Collections.Generic;

namespace Lab2
{
    public static class Extensions
    {
        public static string ToSingleStr(this IEnumerable<string> strings, string separator = ", ") =>
            string.Join(separator, strings);
    }
}
