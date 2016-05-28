using System.IO;
using System.Xml.Serialization;
using KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina.Xml;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.KonfiguracjaPlugina
{
    class Konfiguracja
    {
        #region[STATIC]
        private static Konfiguracja instance;
        public static Konfiguracja GetInstance(SolutionWrapper solution)
        {
            if (instance == null || instance.solution.PelnaNazwa != solution.PelnaNazwa)
                instance = new Konfiguracja(solution);
            return instance;
        }
        #endregion

        private SolutionWrapper solution;

        private KonfiguracjaUsingow Usingi { get; set; }
        private KruchyPlugin konfiguracjaXml;

        private Konfiguracja(SolutionWrapper solution)
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

        private string DajSciezkePlikuKonfiguracji(SolutionWrapper solution)
        {
            var pelnaSciezkaSolution = solution.PelnaNazwa;
            return pelnaSciezkaSolution + ".kruchy.xml";
        }

        public KonfiguracjaUsingow DajKonfiguracjeUsingow(SolutionWrapper solution)
        {
            return Usingi;
        }

        public bool SortowacZaleznosciSerwisu()
        {
            return konfiguracjaXml.SortowanieZaleznosciSerwisow;
        }
    }
}