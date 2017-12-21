using System;
using System.IO;
using System.Text;
using System.Windows;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Extensions;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieKlasService
    {
        private readonly SolutionWrapper solution;

        public GenerowanieKlasService(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public WynikGenerowaniaKlasService Generuj(
            PlikWrapper aktualnyPlik,
            string nazwaKlasyService,
            bool obaWKataloguImpl)
        {
            var aktualny = solution.AktualnyPlik;
            var projekt = aktualny.Projekt;

            if (aktualny == null)
                throw new ApplicationException("Nie ma otwartego pliku");

            var nazwaPlikuImplementacji = nazwaKlasyService + ".cs"; ;
            var nazwaPlikuInterfejsu = "I" + nazwaKlasyService + ".cs";

            var pelnaSciezkaDoImplementacji =
                Path.Combine(
                    projekt.SciezkaDoServiceImpl(), nazwaPlikuImplementacji);

            string pelnaSciezkaDoInterfejsu;
            if (!obaWKataloguImpl)
                pelnaSciezkaDoInterfejsu =
                    Path.Combine(
                        projekt.SciezkaDoService(), nazwaPlikuInterfejsu);
            else
                pelnaSciezkaDoInterfejsu =
                    Path.Combine(
                        projekt.SciezkaDoServiceImpl(), nazwaPlikuInterfejsu);

            if (File.Exists(pelnaSciezkaDoImplementacji))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoImplementacji);
                return null;
            }
            if (File.Exists(pelnaSciezkaDoInterfejsu))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoInterfejsu);
                return null;
            }


            File.WriteAllText(
                pelnaSciezkaDoImplementacji,
                GenerujPlikImplementacji(nazwaKlasyService, projekt),
                Encoding.UTF8);
            File.WriteAllText(
                pelnaSciezkaDoInterfejsu,
                GenerujPlikInterfejsu(nazwaKlasyService, projekt, obaWKataloguImpl),
                Encoding.UTF8);

            var plikImpl = projekt.DodajPlik(pelnaSciezkaDoImplementacji);
            var plikInt = projekt.DodajPlik(pelnaSciezkaDoInterfejsu);

            var solutionExplorer = new SolutionExplorerWrapper(solution);
            solutionExplorer.OtworzPlik(plikInt.SciezkaPelna);
            solutionExplorer.OtworzPlik(plikImpl.SciezkaPelna);

            return new WynikGenerowaniaKlasService();
        }

        private string GenerujPlikImplementacji(
            string nazwaKlasyService,
            ProjektWrapper projekt)
        {
            var klasaBuilder =
                new ClassBuilder()
                    .ZModyfikatorem("")
                    .ZNazwa(nazwaKlasyService)
                    .DodajInterfejs("I" + nazwaKlasyService);

            var plikBuilder =
                new PlikClassBuilder()
                    .ZObiektem(klasaBuilder)
                    .WNamespace(GenerujNamespaceImpl(projekt));

            var zawartosc = plikBuilder.Build();
            return zawartosc;
        }

        private string GenerujPlikInterfejsu(
            string nazwaKlasyService,
            ProjektWrapper projekt,
            bool obaWImpl)
        {
            var interfaceBuilder =
                new InterfejsBuilder()
                    .ZNazwa("I" + nazwaKlasyService)
                    .ZModyfikatorem("public");

            var plikClassBuilder =
                new PlikClassBuilder()
                    .ZObiektem(interfaceBuilder);
            if (!obaWImpl)
                plikClassBuilder.WNamespace(GenerujNamespace(projekt));
            else
                plikClassBuilder.WNamespace(GenerujNamespaceImpl(projekt));

            var zawartosc = plikClassBuilder.Build();
            return zawartosc;
        }

        private string GenerujNamespace(ProjektWrapper projekt)
        {
            return projekt.Nazwa + ".Services";
        }

        private string GenerujNamespaceImpl(ProjektWrapper projekt)
        {
            return GenerujNamespace(projekt) + ".Impl";
        }
    }
}
