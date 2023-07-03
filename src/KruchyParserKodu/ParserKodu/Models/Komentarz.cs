using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Komentarz : ParsowanaJednostka
    {
        public List<string> Linie { get; set; }

        public Komentarz()
        {
            Linie = new List<string>();
        }

        public void DodajLinie(string linia)
        {
            Linie.Add(linia);
        }

        public void DodajCalyKomentarz(string komentarz)
        {

        }
    }
}
