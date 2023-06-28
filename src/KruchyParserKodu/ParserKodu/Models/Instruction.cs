using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Instruction : ParsowanaJednostka, IWithPlaceInCode
    {
        public MethodConstructorBase CodeUnit { get; set; }

        public PozycjaWPliku StartPosition
        {
            get => Poczatek;
            set => Poczatek = value;
        }

        public PozycjaWPliku EndPosition
        {
            get => Koniec;
            set => Koniec = value;
        }
    }
}
