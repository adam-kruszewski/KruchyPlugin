using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu
{
    public static class IWithOwnerExtensions
    {
        public static int WyliczPoziomMetody(this IWithOwner obiekt)
        {
            if (obiekt == null)
                return 0;

            if (obiekt.Owner == null)
                return 1;

            return WyliczPoziomMetody(obiekt.Owner) + 1;
        }
    }
}