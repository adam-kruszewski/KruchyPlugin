using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public class Pole : ParsedUnit, IWithDocumentation, IWithOwner
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public bool Generyczny { get; set; }
        public IList<Modifier> Modyfikatory { get; private set; }

        public Documentation Documentation { get; set; }
        public DefinedItem Owner { get; set; }

        public Pole()
        {
            Modyfikatory = new List<Modifier>();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} [{2}]", Nazwa, NazwaTypu, ScalModyfikatory());
        }

        private string ScalModyfikatory()
        {
            return string.Join(", ", Modyfikatory.Select(o => o.Name));
        }
    }
}