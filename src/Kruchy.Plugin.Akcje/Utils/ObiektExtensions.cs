using System.Collections.Generic;
using System.Linq;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class ObiektExtensions
    {
        public static IEnumerable<Field> SzukajPolPrivateReadOnly(
            this DefinedItem obiekt)
        {
            return obiekt.Fields.Where(o => SzukajPolPrivateReadOnly(o));
        }

        private static bool SzukajPolPrivateReadOnly(Field pole)
        {
            var modyfikatory = pole.Modifiers.Select(o => o.Name);
            return modyfikatory.Contains("private")
                && modyfikatory.Contains("readonly");
        }
    }
}