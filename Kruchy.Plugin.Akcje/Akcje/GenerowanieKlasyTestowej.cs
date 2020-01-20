using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class GenerowanieKlasyTestowej
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        private IProjektWrapper AktualnyProjekt
        {
            get { return solution.AktualnyPlik.Projekt; }
        }

        private IProjektWrapper ProjektTestowy
        {
            get
            {
                var nazwaProjektuTestowego =
                    AktualnyProjekt.Nazwa + ".Tests";

                return solution.ZnajdzProjekt(nazwaProjektuTestowego);
            }
        }

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
            bool integracyjny)
        {
            var aktualnyProjekt = solution.AktualnyPlik.Projekt;
            var nazwaProjektuTestowego =
                aktualnyProjekt.Nazwa + ".Tests";

            var projektTestowy = solution.ZnajdzProjekt(nazwaProjektuTestowego);

            if (projektTestowy == null)
                throw new ApplicationException(
                    "Nie ma projektu testowego dla projektu " + aktualnyProjekt.Nazwa);

            var nazwaPlikuTestow = nazwaKlasy + ".cs";
            var pelnaSciezka = Path.Combine(
                DajSciezkeDoKataloguTestow(integracyjny),
                nazwaPlikuTestow);

            string zawartosc =
                GenerujZawartosc(
                    nazwaKlasy,
                    rodzaj,
                    interfejsTestowany,
                    integracyjny);
            if (File.Exists(pelnaSciezka))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezka);
                return;
            }

            File.WriteAllText(pelnaSciezka, zawartosc, Encoding.UTF8);
            var plik = ProjektTestowy.DodajPlik(pelnaSciezka);

            solutionExplorer.OtworzPlik(plik);
        }

        private string DajSciezkeDoKataloguTestow(bool integracyjne)
        {
            if (integracyjne)
                return ProjektTestowy.SciezkaDoIntegrationTests();
            else
                return ProjektTestowy.SciezkaDoUnitTests();
        }

        private string GenerujZawartosc(
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            bool integracyjny)
        {
            var konfiguracja = Konfiguracja.GetInstance(solution);

            return GenerujZawartoscDynamiczna(
                konfiguracja,
                nazwaKlasy,
                rodzaj,
                interfejsTestowany,
                integracyjny);
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

        private string DajNamespaceKlastyTestowej(bool integracyjny)
        {
            return ProjektTestowy.Nazwa +
                DajFragmentNamespaceDotyczacyRodzajuTestow(integracyjny);
        }

        private string GenerujZawartoscDynamiczna(
            Konfiguracja konfiguracja,
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            bool integracyjny)
        {
            var szablon = konfiguracja.KlasyTestowe().Single(o => o.Nazwa == rodzaj);

            return WypelnijZnaczniki(
                szablon,
                nazwaKlasy,
                rodzaj,
                interfejsTestowany,
                integracyjny);
        }

        private string WypelnijZnaczniki(
            KlasaTestowa szablon,
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            bool integracyjny)
        {
            var slownikWartosci = PrzygotujWartosciZnacznikow(
                nazwaKlasy,
                rodzaj,
                interfejsTestowany,
                integracyjny);

            var wynik = szablon.Zawartosc;

            foreach (var wartosc in slownikWartosci)
                wynik = wynik.Replace(wartosc.Key, wartosc.Value);

            return wynik;
        }

        private IDictionary<string, string> PrzygotujWartosciZnacznikow(
            string nazwaKlasy,
            string rodzaj,
            string interfejsTestowany,
            bool integracyjny)
        {
            var wynik = new Dictionary<string, string>();

            wynik.Add("%NAMESPACE%", DajNamespaceKlastyTestowej(integracyjny));
            wynik.Add("%NAZWA_KLASY%", nazwaKlasy);
            wynik.Add("%NAMESPACE_INTERFEJSU_TESTOWANEGO%", DajNamespaceInterfejsuTestowanego());
            wynik.Add("%INTERFEJS_TESTOWANY%", interfejsTestowany);

            var kategoria = DajKategorie(integracyjny);
            wynik.Add("%KATEGORIA%", kategoria);

            return wynik;
        }

        private bool RodzajZKonfiguracjiDynamicznej(
            Konfiguracja konfiguracja,
            string rodzaj)
        {
            return konfiguracja.KlasyTestowe().Any(o => o.Nazwa == rodzaj);
        }

        private static string DajFragmentNamespaceDotyczacyRodzajuTestow(
            bool integracyjny)
        {
            if (integracyjny)
                return ".Integration";
            else
                return ".Unit";
        }

        private string DajKategorie(bool integracyjny)
        {
            if (integracyjny)
                return "TestCategories.Integration";
            else
                return "TestCategories.Unit";
        }
    }
}
