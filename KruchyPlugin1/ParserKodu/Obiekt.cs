using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Obiekt : ParsowanaJednostka
    {
        public RodzajObiektu Rodzaj { get; set; }

        public string Nazwa { get; set; }
        public IList<Konstruktor> Konstruktory { get; private set; }
        public IList<Pole> Pola { get; private set; }
        public IList<Property> Propertiesy { get; private set; }
        public IList<Metoda> Metody { get; private set; }

        public Obiekt() : base()
        {
            Konstruktory = new List<Konstruktor>();
            Pola = new List<Pole>();
            Propertiesy = new List<Property>();
            Metody = new List<Metoda>();
        }
    }
}
