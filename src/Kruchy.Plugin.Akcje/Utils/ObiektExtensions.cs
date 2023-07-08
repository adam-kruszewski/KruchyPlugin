﻿using System.Collections.Generic;
using System.Linq;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Akcje.Utils
{
    public static class ObiektExtensions
    {
        public static IEnumerable<Pole> SzukajPolPrivateReadOnly(
            this DefinedItem obiekt)
        {
            return obiekt.Fields.Where(o => SzukajPolPrivateReadOnly(o));
        }

        private static bool SzukajPolPrivateReadOnly(Pole pole)
        {
            var modyfikatory = pole.Modyfikatory.Select(o => o.Name);
            return modyfikatory.Contains("private")
                && modyfikatory.Contains("readonly");
        }
    }
}