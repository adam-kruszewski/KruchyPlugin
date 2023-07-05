using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    interface IWithParameterBraces
    {
        PlaceInFile StartingParameterBrace { get; set; }
        PlaceInFile ClosingParameterBrace { get; set; }
    }
}