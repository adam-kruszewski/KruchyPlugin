using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class GenericParameter : ParsedUnit, IWithName
    {
        public string Name { get; set; }
    }
}
