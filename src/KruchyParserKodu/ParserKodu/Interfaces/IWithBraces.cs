
namespace KruchyParserKodu.ParserKodu.Interfaces
{
    interface IWithBraces
    {
        PozycjaWPliku StartingBrace { get; set; }
        PozycjaWPliku ClosingBrace { get; set; }
    }
}