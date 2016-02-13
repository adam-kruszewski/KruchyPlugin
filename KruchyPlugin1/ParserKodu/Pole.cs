using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Pole : ParsowanaJednostka
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public IList<Modyfikator> Modyfikatory { get; private set; }

        public Pole()
        {
            Modyfikatory = new List<Modyfikator>();
        }
    }
}