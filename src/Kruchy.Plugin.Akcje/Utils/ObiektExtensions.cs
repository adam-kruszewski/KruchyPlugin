using System.Collections.Generic;
using System.Linq;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class ObiektExtensions
    {
        public static IEnumerable<Pole> SzukajPolPrivateReadOnly(
            this Obiekt obiekt)
        {
            return obiekt.Pola.Where(o => SzukajPolPrivateReadOnly(o));
        }

        private static bool SzukajPolPrivateReadOnly(Pole pole)
        {
            var modyfikatory = pole.Modyfikatory.Select(o => o.Name);
            return modyfikatory.Contains("private")
                && modyfikatory.Contains("readonly");
        }
    }
}