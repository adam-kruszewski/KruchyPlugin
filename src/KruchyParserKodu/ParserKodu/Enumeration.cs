using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Enumeration
        : ParsowanaJednostka
            , IZPoczatkowaIKoncowaKlamerka
                , IZWlascicielem
                    , IZKomentarzem
                        , IZDokumentacja
    {
        public Obiekt Wlasciciel { get; set; }

        public string Nazwa { get; set; }

        public IList<Pole> Pola { get; private set; }

        public IList<Modyfikator> Modyfikatory { get; set; }

        public List<Atrybut> Atrybuty { get; private set; }

        public PozycjaWPliku PoczatkowaKlamerka { get; set; }

        public PozycjaWPliku KoncowaKlamerka { get; set; }

        public Komentarz Komentarz { get; set; }

        public Dokumentacja Dokumentacja { get; set; }

        public Enumeration()
        {
            Pola = new List<Pole>();
            PoczatkowaKlamerka = new PozycjaWPliku();
            KoncowaKlamerka = new PozycjaWPliku();
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
        }

    }
}
