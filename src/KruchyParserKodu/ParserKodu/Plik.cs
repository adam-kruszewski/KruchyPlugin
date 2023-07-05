using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Plik
    {
        public string Namespace { get; set; }
        public PozycjaWPliku PoczatekNamespace { get; set; }
        public PozycjaWPliku KoniecNamespace { get; set; }
        public IList<UsingNamespace> Usingi { get; private set; }

        public IList<Obiekt> DefiniowaneObiekty { get; private set; }

        public IList<Enumeration> DefiniowaneEnumeracje { get; private set; }

        public Plik()
        {
            DefiniowaneObiekty = new List<Obiekt>();
            Usingi = new List<UsingNamespace>();
            DefiniowaneEnumeracje = new List<Enumeration>();
        }
    }
}