using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Plik
    {
        public string Namespace { get; set; }
        public PozycjaWPliku PoczatekNamespace { get; set; }
        public PozycjaWPliku KoniecNamespace { get; set; }
        public IList<string> Usingi { get; private set; }

        public IList<Obiekt> DefiniowaneObiekty { get; private set; }

        public Plik()
        {
            DefiniowaneObiekty = new List<Obiekt>();
            Usingi = new List<string>();
        }
    }
}