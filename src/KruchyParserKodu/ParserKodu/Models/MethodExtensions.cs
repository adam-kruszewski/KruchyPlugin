using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public static class MethodExtensions
    {
        public static bool TheSameMethod(this Method m1, Method m2)
        {
            if (m1.Name != m2.Name)
                return false;

            if (m1.Parametry.Count != m2.Parametry.Count)
                return false;

            if (!TheSameParameters(m1.Parametry, m2.Parametry))
                return false;

            return true;
        }

        private static bool TheSameParameters(
            IList<Parameter> list1,
            IList<Parameter> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].TypeName != list2[i].TypeName)
                    return false;
                if (list1[i].ParameterName != list2[i].ParameterName)
                    return false;
            }
            return true;
        }
    }
}