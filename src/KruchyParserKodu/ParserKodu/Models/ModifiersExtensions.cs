using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public static class ModifiersExtensions
    {
        public static bool ContainsModifier(
            this IEnumerable<Modifier> modifiers,
            string searched)
        {
            return modifiers.Any(o => o.Name == searched);
        }
    }
}