using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public static class MetodaExtension
    {
        public static bool TaSamaMetoda(this Metoda m1, Metoda m2)
        {
            if (m1.Name != m2.Name)
                return false;

            if (m1.Parametry.Count != m2.Parametry.Count)
                return false;

            if (!TakieSameParametry(m1.Parametry, m2.Parametry))
                return false;

            return true;
        }

        private static bool TakieSameParametry(
            IList<Parametr> list1,
            IList<Parametr> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].NazwaTypu != list2[i].NazwaTypu)
                    return false;
                if (list1[i].NazwaParametru != list2[i].NazwaParametru)
                    return false;
            }
            return true;
        }
    }
}