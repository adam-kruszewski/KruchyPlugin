using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    public interface IWithOwner
    {
        Obiekt Owner { get; set; }
    }
}