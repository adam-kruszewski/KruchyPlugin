
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu
{
    public abstract class ParsowanaJednostka
    {
        public PozycjaWPliku Poczatek { get; set; }
        public PozycjaWPliku Koniec { get; set; }

        public ParsowanaJednostka()
        {
            Poczatek = new PozycjaWPliku();
            Koniec = new PozycjaWPliku();
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Poczatek, Koniec);
        }
    }
}