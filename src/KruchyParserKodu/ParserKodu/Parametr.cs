
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Parametr
    {
        public string NazwaTypu { get; set; }
        public string NazwaParametru { get; set; }

        public bool ZThisem { get; set; }
        public string WartoscDomyslna { get; set; }

        public bool ZRef { get; set; }
        public bool ZOut { get; set; }
        public bool ZParams { get; set; }

        public string Modyfikator { get; set; }

        public List<Atrybut> Atrybuty { get; private set; }

        public Parametr()
        {
            Atrybuty = new List<Atrybut>();
        }
    }
}