using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Konstruktor
        : ParsowanaJednostka
            , IZNawiasamiOtwierajacymiZamykajacymiParametry
                , IZPoczatkowaIKoncowaKlamerka
    {
        public IList<Parametr> Parametry { get; private set; }
        public string Modyfikator { get; set; }

        public PozycjaWPliku PoczatekParametrow { get; private set; }
        public PozycjaWPliku KoniecParametrow { get; private set; }
        public PozycjaWPliku NawiasOtwierajacyParametry { get; set; }
        public PozycjaWPliku NawiasZamykajacyParametry { get; set; }
        public PozycjaWPliku PoczatkowaKlamerka { get; set; }
        public PozycjaWPliku KoncowaKlamerka { get; set; }

        public Konstruktor()
        {
            Parametry = new List<Parametr>();
            NawiasOtwierajacyParametry = new PozycjaWPliku();
            NawiasZamykajacyParametry = new PozycjaWPliku();
            PoczatkowaKlamerka = new PozycjaWPliku();
            KoncowaKlamerka = new PozycjaWPliku();
        }
    }
}