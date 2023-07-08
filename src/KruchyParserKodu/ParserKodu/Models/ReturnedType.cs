using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class ReturnedType : ParsedUnit, IWithName
    {
        public string Name { get; set; }
    }
}
