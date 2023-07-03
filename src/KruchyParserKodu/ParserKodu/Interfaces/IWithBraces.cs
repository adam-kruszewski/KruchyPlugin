
namespace KruchyParserKodu.ParserKodu.Interfaces
{
    interface IWithBraces
    {
        PozycjaWPliku StartingBrace { get; set; }
        PozycjaWPliku FinishingBrace { get; set; }
    }
}