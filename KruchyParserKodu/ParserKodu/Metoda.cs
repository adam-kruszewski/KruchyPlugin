using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public class Metoda
        : ParsowanaJednostka
            , IZNawiasamiOtwierajacymiZamykajacymiParametry
    {
        public Obiekt Wlasciciel { get; set; }

        public IList<Parametr> Parametry { get; private set; }
        public IList<Modyfikator> Modyfikatory { get; set; }
        public List<Atrybut> Atrybuty { get; private set; }

        public string TypZwracany { get; set; }
        public string Nazwa { get; set; }
        public PozycjaWPliku NawiasOtwierajacyParametry { get; set; }
        public PozycjaWPliku NawiasZamykajacyParametry { get; set; }

        public bool Prywatna
        {
            get
            {
                return Modyfikatory.Any(o => o.Nazwa == "private");
            }
        }

        public bool Publiczna
        {
            get
            {
                return Modyfikatory.Any(o => o.Nazwa == "public");
            }
        }

        public Metoda()
        {
            Parametry = new List<Parametr>();
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
            NawiasOtwierajacyParametry = new PozycjaWPliku();
            NawiasZamykajacyParametry = new PozycjaWPliku();
        }

        public bool ZawieraModyfikator(string nazwa)
        {
            return Modyfikatory.Any(o => o.Nazwa == nazwa);
        }
    }
}