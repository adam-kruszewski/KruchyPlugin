using KruchyParserKodu.ParserKodu.Models;
using Microsoft.CodeAnalysis.Text;

namespace KruchyParserKodu.Utils
{
    public static class PozycjaWPlikuExtensions
    {
        public static PlaceInFile ToPozycjaWPliku(this LinePosition linePosition)
        {
            return new PlaceInFile(
                linePosition.Line,
                linePosition.Character);
        }
    }
}
