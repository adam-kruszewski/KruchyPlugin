
namespace KruchyParserKodu.ParserKodu
{
    public class PozycjaWPliku
    {
        public int Wiersz { get; set; }
        public int Kolumna { get; set; }

        public PozycjaWPliku() { }

        public PozycjaWPliku(int wiersz, int kolumna)
        {
            Wiersz = wiersz;
            Kolumna = kolumna;
        }
    }
}