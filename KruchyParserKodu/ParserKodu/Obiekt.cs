using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Obiekt : ParsowanaJednostka, IZPoczatkowaIKoncowaKlamerka
    {
        public RodzajObiektu Rodzaj { get; set; }

        public string Nazwa { get; set; }
        public IList<Konstruktor> Konstruktory { get; private set; }
        public IList<Pole> Pola { get; private set; }
        public IList<Property> Propertiesy { get; private set; }
        public IList<Metoda> Metody { get; private set; }
        public List<Atrybut> Atrybuty { get; private set; }
        public IList<ObiektDziedziczony> NadklasaIInterfejsy { get; private set; }

        public PozycjaWPliku PoczatkowaKlamerka { get; private set; }
        public PozycjaWPliku KoncowaKlamerka { get; private set; }

        public Obiekt() : base()
        {
            Konstruktory = new List<Konstruktor>();
            Pola = new List<Pole>();
            Propertiesy = new List<Property>();
            Metody = new List<Metoda>();
            Atrybuty = new List<Atrybut>();
            NadklasaIInterfejsy = new List<ObiektDziedziczony>();

            PoczatkowaKlamerka = new PozycjaWPliku();
            KoncowaKlamerka = new PozycjaWPliku();
        }
    }
}
