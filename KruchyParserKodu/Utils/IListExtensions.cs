using System.Collections.Generic;

namespace KruchyParserKodu.Utils
{
    public static class IListExtensions
    {
        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> elements)
        {
            foreach (var element in elements)
                list.Add(element);

            return list;
        }
    }
}