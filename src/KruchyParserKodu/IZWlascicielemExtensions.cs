using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu
{
    public static class IZWlascicielemExtensions
    {
        public static int WyliczPoziomMetody(this IZWlascicielem obiekt)
        {
            if (obiekt == null)
                return 0;

            if (obiekt.Wlasciciel == null)
                return 1;

            return WyliczPoziomMetody(obiekt.Wlasciciel) + 1;
        }
    }
}