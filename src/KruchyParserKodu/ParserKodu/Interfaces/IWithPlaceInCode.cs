namespace KruchyParserKodu.ParserKodu.Interfaces
{
    public interface IWithPlaceInCode
    {
        PozycjaWPliku StartPosition { get; set; }

        PozycjaWPliku EndPosition { get; set; }
    }
}
