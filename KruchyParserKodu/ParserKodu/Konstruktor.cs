using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Konstruktor
        : ParsowanaJednostka
            , IZNawiasamiOtwierajacymiZamykajacymiParametry
                , IZPoczatkowaIKoncowaKlamerka
                    , IZWlascicielem
                        , IZDokumentacja
    {
        public Obiekt Wlasciciel { get; set; }

        public IList<Parametr> Parametry { get; private set; }
        public string Modyfikator { get; set; }

        public PozycjaWPliku PoczatekParametrow { get; private set; }
        public PozycjaWPliku KoniecParametrow { get; private set; }
        public PozycjaWPliku NawiasOtwierajacyParametry { get; set; }
        public PozycjaWPliku NawiasZamykajacyParametry { get; set; }
        public PozycjaWPliku PoczatkowaKlamerka { get; set; }
        public PozycjaWPliku KoncowaKlamerka { get; set; }

        //jeśli nie ma wołania kontruktora z nadklasy, to pole jest nullem
        public IList<string> ParametryKonstruktoraZNadKlasy;
        public string SlowoKluczoweInicjalizacji { get; set; }
        public Dokumentacja Dokumentacja { get; set; }

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