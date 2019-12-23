using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.Akcje.DodawanieMapowanElementy
{
    public class MapowanyProperty
    {
        public string Prefix { get; set; }
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public List<MapowanyProperty> Podobiekty { get; private set; }

        public MapowanyProperty(
            string nazwa,
            string nazwaTypu,
            string prefix)
        {
            Nazwa = nazwa;
            NazwaTypu = nazwaTypu;
            Prefix = prefix;
            Podobiekty = new List<MapowanyProperty>();
        }

        public override string ToString()
        {
            return "[" + NazwaTypu + "]" +  Prefix + Nazwa;
        }
    }
}