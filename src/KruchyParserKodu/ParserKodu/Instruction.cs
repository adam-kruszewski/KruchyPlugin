using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu
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
