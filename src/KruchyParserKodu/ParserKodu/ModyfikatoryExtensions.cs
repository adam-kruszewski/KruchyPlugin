using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public static class ModyfikatoryExtensions
    {
        public static bool ZawieraModyfikator(
            this IEnumerable<Modyfikator> modyfikatory,
            string szukany)
        {
            return modyfikatory.Any(o => o.Nazwa == szukany);
        }
    }
}