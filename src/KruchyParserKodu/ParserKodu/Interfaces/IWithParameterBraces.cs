using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    interface IWithParameterBraces
    {
        PozycjaWPliku StartingParameterBrace { get; set; }
        PozycjaWPliku ClosingParameterBrace { get; set; }
    }
}