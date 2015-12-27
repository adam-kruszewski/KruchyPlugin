using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Konstruktor : ParsowanaJednostka
    {
        public IList<Parametr> Parametry { get; private set; }
        public string Modyfikator { get; set; }

        public PozycjaWPliku PoczatekParametrow { get; private set; }
        public PozycjaWPliku KoniecParametrow { get; private set; }

        public Konstruktor()
        {
            Parametry = new List<Parametr>();
        }
    }
}