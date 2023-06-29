using System;
using System.Collections.Generic;
using System.IO;
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
            if (instance == null || instance.Solution.PelnaNazwa != solution.PelnaNazwa)
                instance = new Konfiguracja(solution);
            return instance;
        }

        public static Konfiguracja SetInstance(Konfiguracja instance1)
        {
            instance = instance1;
            return instance;
        }

        public static void Modify(ISolutionWrapper solution, Action<KruchyPlugin> modifyAction)
        {
            var configurationFilePath = DajSciezkePlikuKonfiguracji(solution);

            var backupDirectoryName = ".kruchy-plugin-configuration-backups";

            var fullBackupDirectoryPath = Path.Combine(solution.Katalog, backupDirectoryName);

            if (!Directory.Exists(fullBackupDirectoryPath))
                Directory.CreateDirectory(fullBackupDirectoryPath);

            if (File.Exists(configurationFilePath))
            {
                var newPath = Path.Combine(fullBackupDirectoryPath, solution.Nazwa + $"{DateTime.Now.ToFileTime()}-backup");
                File.Copy(configurationFilePath, newPath);
            }

            modifyAction(instance.konfiguracjaXml);

            var serializer = new XmlSerializer(typeof(KruchyPlugin));

            using (var stream = new FileStream(configurationFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, instance.konfiguracjaXml);
            }

        }
        #endregion

        public virtual ISolutionWrapper Solution { get; set; }

        private KonfiguracjaUsingow Usingi { get; set; }
        internal KruchyPlugin konfiguracjaXml;

        public Konfiguracja()
        {
            UstawDefaultoweDlaPincasso();
        }

        private Konfiguracja(ISolutionWrapper solution)
        {
            this.Solution = solution;
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

        private static string DajSciezkePlikuKonfiguracji(ISolutionWrapper solution)
        {
            var pelnaSciezkaSolution = solution.PelnaNazwa;
            return pelnaSciezkaSolution + ".kruchy.xml";
        }

        public virtual KonfiguracjaUsingow DajKonfiguracjeUsingow(ISolutionWrapper solution)
        {
            return Usingi;
        }

        public virtual bool SortowacZaleznosciSerwisu()
        {
            return konfiguracjaXml.SortowanieZaleznosciSerwisow;
        }

        public virtual IEnumerable<PrzejdzDo> PrzejdzDo()
        {
            return konfiguracjaXml.PrzejdzDo;
        }

        public virtual IEnumerable<KlasaTestowa> KlasyTestowe()
        {
            return konfiguracjaXml.KlasyTestowe;
        }

        public virtual IEnumerable<MapowanieTypuXsd> MapowaniaTypowXsd()
        {
            return konfiguracjaXml.MapowaniaTypowXsd;
        }

        public virtual IEnumerable<SchematGenerowania> SchematyGenerowania()
        {
            return konfiguracjaXml.Schematy;
        }

        public virtual Testy Testy()
        {
            return konfiguracjaXml.Testy;
        }

        public virtual IEnumerable<ProjektTestowy> PowiazaniaProjektowTestowych => konfiguracjaXml.PowiazaniaProjektowTestowych;

        public virtual Dokumentacja Dokumentacja()
        {
            return konfiguracjaXml.Dokumentacja;
        }
    }
}
