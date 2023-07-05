using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu.Interfaces
{
    public interface IWithPlaceInCode
    {
        PlaceInFile StartPosition { get; set; }

        PlaceInFile EndPosition { get; set; }
    }
}
