using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Metoda : ParsowanaJednostka
    {
        public IList<Parametr> Parametry { get; private set; }
        public IList<string> Modyfikatory { get; set; }
        public string TypZwracany { get; set; }
        public string Nazwa { get; set; }
        public PozycjaWPliku NawiasOtwierajacyParametry { get; set; }
        public PozycjaWPliku NawiasZamykajacyParametry { get; set; }

        public Metoda()
        {
            Parametry = new List<Parametr>();
            Modyfikatory = new List<string>();
            NawiasOtwierajacyParametry = new PozycjaWPliku();
            NawiasZamykajacyParametry = new PozycjaWPliku();
        }
    }
}