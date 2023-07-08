using KruchyParserKodu.ParserKodu.Interfaces;

namespace KruchyParserKodu.ParserKodu.Models
{
    public abstract class ParsedUnit : IWithPlaceInCode
    {
        public PlaceInFile StartPosition { get; set; }
        public PlaceInFile EndPosition { get; set; }

        public ParsedUnit()
        {
            StartPosition = new PlaceInFile();
            EndPosition = new PlaceInFile();
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", StartPosition, EndPosition);
        }
    }
}