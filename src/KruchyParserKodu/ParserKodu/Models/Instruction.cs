using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Instruction : ParsowanaJednostka, IWithPlaceInCode
    {
        public MethodConstructorBase CodeUnit { get; set; }

        public PlaceInFile StartPosition
        {
            get => Poczatek;
            set => Poczatek = value;
        }

        public PlaceInFile EndPosition
        {
            get => Koniec;
            set => Koniec = value;
        }

        public string Text { get; set; }
    }
}
