using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public class Pole : ParsowanaJednostka, IWithDocumentation, IZWlascicielem
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public bool Generyczny { get; set; }
        public IList<Modyfikator> Modyfikatory { get; private set; }

        public Dokumentacja Dokumentacja { get; set; }
        public Obiekt Wlasciciel { get; set; }

        public Pole()
        {
            Modyfikatory = new List<Modyfikator>();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} [{2}]", Nazwa, NazwaTypu, ScalModyfikatory());
        }

        private string ScalModyfikatory()
        {
            return string.Join(", ", Modyfikatory.Select(o => o.Nazwa));
        }
    }
}