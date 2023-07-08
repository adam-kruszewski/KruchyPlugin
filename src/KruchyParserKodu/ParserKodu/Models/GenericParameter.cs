using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class GenericParameter : ParsowanaJednostka, IWithName
    {
        public string Name { get; set; }
    }
}
