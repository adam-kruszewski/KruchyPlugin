using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Instruction : ParsedUnit, IWithPlaceInCode
    {
        public MethodConstructorBase CodeUnit { get; set; }

        public PlaceInFile StartPosition
        {
            get => base.StartPosition;
            set => base.StartPosition = value;
        }

        public PlaceInFile EndPosition
        {
            get => base.EndPosition;
            set => base.EndPosition = value;
        }

        public string Text { get; set; }
    }
}
