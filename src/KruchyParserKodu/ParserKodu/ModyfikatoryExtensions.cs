using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public static class ModyfikatoryExtensions
    {
        public static bool ZawieraModyfikator(
            this IEnumerable<Modifier> modyfikatory,
            string szukany)
        {
            return modyfikatory.Any(o => o.Name == szukany);
        }
    }
}