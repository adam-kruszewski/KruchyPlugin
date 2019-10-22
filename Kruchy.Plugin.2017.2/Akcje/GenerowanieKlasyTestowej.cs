using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieKlasyTestowej
    {
        private readonly ISolutionWrapper solution;

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

        public GenerowanieKlasyTestowej(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj(
            string nazwaKlasy,
            RodzajKlasyTestowej rodzaj,
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

            new SolutionExplorerWrapper(solution).OtworzPlik(plik);
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
            RodzajKlasyTestowej rodzaj,
            string interfejsTestowany,
            bool integracyjny)
        {
            var atrybutCategory =
                new AtrybutBuilder()
                .ZNazwa("Category")
                .DodajWartoscParametruNieStringowa(DajKategorie(integracyjny));
            var atrybut =
                new AtrybutBuilder()
                    .ZNazwa("TestFixture")
                    .DodajKolejnyAtrybut(atrybutCategory);
            var klasaBuilder =
                new ClassBuilder()
                    .ZModyfikatorem("public")
                    .ZNazwa(nazwaKlasy)
                    .DodajAtrybut(atrybut);

            switch (rodzaj)
            {
                case RodzajKlasyTestowej.ServiceTests:
                    klasaBuilder.ZNadklasa("ServiceTests<" + interfejsTestowany + ">");
                    break;
                case RodzajKlasyTestowej.TestsWithDatabase:
                    klasaBuilder.ZNadklasa("TestsWithDatabaseFixture");
                    break;
            }

            var namespaceTestowanejKlasy = solution.NamespaceAktualnegoPliku();
            if (namespaceTestowanejKlasy.EndsWith(".Impl"))
            {
                namespaceTestowanejKlasy =
                    namespaceTestowanejKlasy.Substring(
                        0, namespaceTestowanejKlasy.Length - ".Impl".Length);
            }
            var plikBuilder = new PlikClassBuilder();
            plikBuilder
                .ZObiektem(klasaBuilder)
                .WNamespace(ProjektTestowy.Nazwa +
                    DajFragmentNamespaceDotyczacyRodzajuTestow(integracyjny))
                .DodajUsing("FluentAssertions")
                .DodajUsing("NUnit.Framework")
                .DodajUsing("Pincasso.Core.Tests.Fixtures")
                .DodajUsing("Piatka.Infrastructure.Tests")
                .DodajUsing(namespaceTestowanejKlasy);
            return plikBuilder.Build();
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
