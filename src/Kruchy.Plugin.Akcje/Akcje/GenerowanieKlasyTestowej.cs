using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class GenerowanieKlasyTestowej
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieKlasyTestowej(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            string katalog)
        {
            var projektTestowy = solution.SzukajProjektuTestowego();

            if (projektTestowy == null)
                throw new ApplicationException(
                    "Nie ma projektu testowego dla projektu " + solution.CurrentProject.Name);

            var nazwaPlikuTestow = nazwaKlasy + ".cs";
            var pelnaSciezka = Path.Combine(
                DajSciezkeDoKataloguTestow(projektTestowy, katalog),
                nazwaPlikuTestow);

            string zawartosc =
                GenerujZawartosc(
                    projektTestowy,
                    nazwaKlasy,
                    rodzaj,
                    interfejsTestowany,
                    katalog);
            if (File.Exists(pelnaSciezka))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezka);
                return;
            }

            var fileInfo = new FileInfo(pelnaSciezka);

            Directory.CreateDirectory(fileInfo.Directory.FullName);

            File.WriteAllText(pelnaSciezka, zawartosc, Encoding.UTF8);
            var plik = projektTestowy.AddFile(pelnaSciezka);

            solutionExplorer.OpenFile(plik);
        }

        private string DajSciezkeDoKataloguTestow(IProjectWrapper projektTestowy, string katalog)
        {
            if (!string.IsNullOrEmpty(katalog))
                return Path.Combine(projektTestowy.DirectoryPath, katalog);

            return projektTestowy.DirectoryPath;
        }

        private string GenerujZawartosc(
            IProjectWrapper projektTestowy,
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            string katalog)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            return GenerujZawartoscDynamiczna(
                projektTestowy,
                konfiguracja,
                nazwaKlasy,
                rodzaj,
                interfejsTestowany,
                katalog);
        }

        private string DajNamespaceInterfejsuTestowanego()
        {
            var namespaceTestowanejKlasy = solution.NamespaceAktualnegoPliku();
            if (namespaceTestowanejKlasy.EndsWith(".Impl"))
            {
                namespaceTestowanejKlasy =
                    namespaceTestowanejKlasy.Substring(
                        0, namespaceTestowanejKlasy.Length - ".Impl".Length);
            }

            return namespaceTestowanejKlasy;
        }

        private string DajNamespaceKlastyTestowej(IProjectWrapper projektTestowy, string katalog)
        {
            if (!string.IsNullOrEmpty(katalog))
                return projektTestowy.Name + "." + katalog.Replace("\\", ".").Replace("/", ".");

            return projektTestowy.Name;
        }

        private string GenerujZawartoscDynamiczna(
            IProjectWrapper projektTestowy,
            Konfiguracja konfiguracja,
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            string katalog)
        {
            var szablon = konfiguracja.KlasyTestowe().Single(o => o.Nazwa == rodzaj);

            return WypelnijZnaczniki(
                projektTestowy,
                szablon,
                nazwaKlasy,
                interfejsTestowany,
                katalog);
        }

        private string WypelnijZnaczniki(
            IProjectWrapper projektTestowy,
            KlasaTestowa szablon,
            string nazwaKlasy,
            string interfejsTestowany,
            string katalog)
        {
            var slownikWartosci = PrzygotujWartosciZnacznikow(
                projektTestowy,
                nazwaKlasy,
                interfejsTestowany,
                katalog);

            var wynik = szablon.Zawartosc;

            foreach (var wartosc in slownikWartosci)
                wynik = wynik.Replace(wartosc.Key, wartosc.Value);

            return wynik;
        }

        private IDictionary<string, string> PrzygotujWartosciZnacznikow(
            IProjectWrapper projektTestowy,
            string nazwaKlasy,
            string interfejsTestowany,
            string katalog)
        {
            var wynik = new Dictionary<string, string>();

            wynik.Add("%NAMESPACE%", DajNamespaceKlastyTestowej(projektTestowy, katalog));
            wynik.Add("%NAZWA_KLASY%", nazwaKlasy);
            wynik.Add("%NAMESPACE_INTERFEJSU_TESTOWANEGO%", DajNamespaceInterfejsuTestowanego());
            wynik.Add("%INTERFEJS_TESTOWANY%", interfejsTestowany);

            return wynik;
        }
    }
}