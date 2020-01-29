using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Wrappers;


namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina
{
    public class Konfiguracja
    {
        #region[STATIC]
        private static Konfiguracja instance;
        public static Konfiguracja GetInstance(ISolutionWrapper solution)
        {
            if (instance == null || instance.solution.PelnaNazwa != solution.PelnaNazwa)
                instance = new Konfiguracja(solution);
            return instance;
        }
        #endregion

        private ISolutionWrapper solution;

        private KonfiguracjaUsingow Usingi { get; set; }
        private KruchyPlugin konfiguracjaXml;

        private Konfiguracja(ISolutionWrapper solution)
        {
            this.solution = solution;
            var sciezkaPlikuKonfiguracji = DajSciezkePlikuKonfiguracji(solution);

            if (!string.IsNullOrEmpty(sciezkaPlikuKonfiguracji) &&
                File.Exists(sciezkaPlikuKonfiguracji))
            {
                konfiguracjaXml = WczytajPlik(sciezkaPlikuKonfiguracji);
                Usingi = new KonfiguracjaUsingow(konfiguracjaXml.Usingi);
            }
            else
                UstawDefaultoweDlaPincasso();
        }

        private void UstawDefaultoweDlaPincasso()
        {
            konfiguracjaXml = new KruchyPlugin();
            Usingi = new KonfiguracjaUsingow();
        }

        private KruchyPlugin WczytajPlik(string sciezkaPlikuKonfiguracji)
        {
            var s = new XmlSerializer(typeof(KruchyPlugin));
            var obj =
                s.Deserialize(
                    new FileStream(sciezkaPlikuKonfiguracji, FileMode.Open));

            return obj as KruchyPlugin;
        }

        private string DajSciezkePlikuKonfiguracji(ISolutionWrapper solution)
        {
            var pelnaSciezkaSolution = solution.PelnaNazwa;
            return pelnaSciezkaSolution + ".kruchy.xml";
        }

        public KonfiguracjaUsingow DajKonfiguracjeUsingow(ISolutionWrapper solution)
        {
            return Usingi;
        }

        public bool SortowacZaleznosciSerwisu()
        {
            return konfiguracjaXml.SortowanieZaleznosciSerwisow;
        }

        public IEnumerable<PrzejdzDo> PrzejdzDo()
        {
            return konfiguracjaXml.PrzejdzDo;
        }

        public IEnumerable<KlasaTestowa> KlasyTestowe()
        {
            return konfiguracjaXml.KlasyTestowe;
        }

        public IEnumerable<MapowanieTypuXsd> MapowaniaTypowXsd()
        {
            return konfiguracjaXml.MapowaniaTypowXsd;
        }

        public IEnumerable<SchematGenerowania> SchematyGenerowania()
        {
            return konfiguracjaXml.Schematy;
        }
    }
}
