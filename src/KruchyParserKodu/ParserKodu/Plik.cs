using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Plik
    {
        public string Namespace { get; set; }
        public PlaceInFile PoczatekNamespace { get; set; }
        public PlaceInFile KoniecNamespace { get; set; }
        public IList<UsingNamespace> Usingi { get; private set; }

        public IList<DefinedItem> DefiniowaneObiekty { get; private set; }

        public IList<Enumeration> DefiniowaneEnumeracje { get; private set; }

        public Plik()
        {
            DefiniowaneObiekty = new List<DefinedItem>();
            Usingi = new List<UsingNamespace>();
            DefiniowaneEnumeracje = new List<Enumeration>();
        }
    }
}