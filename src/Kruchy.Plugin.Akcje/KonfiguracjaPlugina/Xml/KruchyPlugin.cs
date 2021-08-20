using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
    public class KruchyPlugin
    {
        public List<Namespace> Usingi { get; set; }
        public bool SortowanieZaleznosciSerwisow { get; set; }

        public List<PrzejdzDo> PrzejdzDo { get; set; }

        public List<KlasaTestowa> KlasyTestowe { get; set; }

        public List<MapowanieTypuXsd> MapowaniaTypowXsd { get; set; }

        public List<SchematGenerowania> Schematy { get; set; }

        public Testy Testy { get; set; }

        public List<ProjektTestowy> PowiazaniaProjektowTestowych { get; set; }

        public Dokumentacja Dokumentacja { get; set; }

        public KruchyPlugin()
        {
            Usingi = new List<Namespace>();
            PrzejdzDo = new List<PrzejdzDo>();
            KlasyTestowe = new List<KlasaTestowa>();
            MapowaniaTypowXsd = new List<MapowanieTypuXsd>();
            Schematy = new List<SchematGenerowania>();
            Testy = new Testy();
            PowiazaniaProjektowTestowych = new List<ProjektTestowy>();
            Dokumentacja = new Dokumentacja();
        }
    }
}