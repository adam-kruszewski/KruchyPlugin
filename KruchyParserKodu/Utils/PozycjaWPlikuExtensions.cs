using KruchyParserKodu.ParserKodu;
using Microsoft.CodeAnalysis.Text;

namespace KruchyParserKodu.Utils
{
    public static class PozycjaWPlikuExtensions
    {
        public static PozycjaWPliku ToPozycjaWPliku(this LinePosition linePosition)
        {
            return new PozycjaWPliku(
                linePosition.Line,
                linePosition.Character);
        }
    }
}
