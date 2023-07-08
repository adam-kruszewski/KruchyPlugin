using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    public interface IWithOwner
    {
        DefinedItem Owner { get; set; }
    }
}