
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    interface IWithBraces
    {
        PlaceInFile StartingBrace { get; set; }
        PlaceInFile ClosingBrace { get; set; }
    }
}