using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KrucheBuilderyKodu.Builders;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieKlasyTestowej
    {
        private readonly SolutionWrapper solution;

        private ProjektWrapper AktualnyProjekt
        {
            get { return solution.AktualnyPlik.Projekt; }
        }

        private ProjektWrapper ProjektTestowy
        {
            get
            {
                var nazwaProjektuTestowego =
                    AktualnyProjekt.Nazwa + ".Tests";

                return solution.ZnajdzProjekt(nazwaProjektuTestowego);
            }
        }

        public GenerowanieKlasyTestowej(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj(
            string nazwaKlasy,
            RodzajKlasyTestowej rodzaj,
            string interfejsTestowany)
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
                ProjektTestowy.SciezkaDoUnitTests(),
                nazwaPlikuTestow);

            string zawartosc =
                GenerujZawartosc(
                    nazwaKlasy,
                    rodzaj,
                    interfejsTestowany);
            if (File.Exists(pelnaSciezka))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezka);
                return;
            }

            File.WriteAllText(pelnaSciezka, zawartosc, Encoding.UTF8);
            var plik = ProjektTestowy.DodajPlik(pelnaSciezka);

            new SolutionExplorerWrapper(solution).OtworzPlik(plik);
        }

        private string GenerujZawartosc(
            string nazwaKlasy,
            RodzajKlasyTestowej rodzaj,
            string interfejsTestowany)
        {
            var atrybutCategory =
                new AtrybutBuilder()
                .ZNazwa("Category")
                .DodajWartoscParametruNieStringowa("TestCategories.Unit");
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
                .WNamespace(ProjektTestowy.Nazwa + ".Unit")
                .DodajUsing("FluentAssertions")
                .DodajUsing("NUnit.Framework")
                .DodajUsing("Pincasso.Core.Tests.Fixtures")
                .DodajUsing("Piatka.Infrastructure.Tests")
                .DodajUsing(namespaceTestowanejKlasy);
            return plikBuilder.Build();
        }
    }

}
