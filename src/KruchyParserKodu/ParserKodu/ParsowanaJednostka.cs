
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu
{
    public abstract class ParsowanaJednostka
    {
        public PlaceInFile Poczatek { get; set; }
        public PlaceInFile Koniec { get; set; }

        public ParsowanaJednostka()
        {
            Poczatek = new PlaceInFile();
            Koniec = new PlaceInFile();
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Poczatek, Koniec);
        }
    }
}