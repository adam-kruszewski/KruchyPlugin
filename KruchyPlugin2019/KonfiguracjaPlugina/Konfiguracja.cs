using System.IO;
using System.Xml.Serialization;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina
{
    class Konfiguracja
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
    }
}