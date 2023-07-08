using KruchyParserKodu.ParserKodu.Models;
using Microsoft.CodeAnalysis.Text;

namespace KruchyParserKodu.Utils
{
    public static class PlaceInFileExtensions
    {
        public static PlaceInFile ToPlaceInFile(this LinePosition linePosition)
        {
            return new PlaceInFile(
                linePosition.Line,
                linePosition.Character);
        }
    }
}
