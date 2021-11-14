using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
    public class SchematGenerowania
    {
        public string TytulSchematu { get; set; }

        public List<SchematKlasy> SchematyKlas { get; set; }

        public bool WyborSciezki { get; set; }

        public SchematGenerowania()
        {
            SchematyKlas = new List<SchematKlasy>();
        }
    }

    public class SchematKlasy
    {
        public List<Zmienna> Zmienne { get; set; }

        public string NazwaPliku { get; set; }

        public string Tresc { get; set; }

        public SchematKlasy()
        {
            Zmienne = new List<Zmienna>();
        }
    }

    public class Zmienna
    {
        public string Symbol { get; set; }

        public string DopasowaniePliku { get; set; }

        public bool BezRozszerzenia { get; set; }
    }
}