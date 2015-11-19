using System;
using System.IO;
using System.Text;
using KruchyCompany.KruchyPlugin1.CodeBuilders;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;

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
            string nazwaKlasyService)
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
            var pelnaSciezkaDoInterfejsu =
                Path.Combine(
                    projekt.SciezkaDoService(), nazwaPlikuInterfejsu);

            File.WriteAllText(
                pelnaSciezkaDoImplementacji,
                GenerujPlikImplementacji(nazwaKlasyService, projekt),
                Encoding.UTF8);
            File.WriteAllText(
                pelnaSciezkaDoInterfejsu,
                GenerujPlikInterfejsu(nazwaKlasyService, projekt),
                Encoding.UTF8);

            projekt.DodajPlik(pelnaSciezkaDoImplementacji);
            projekt.DodajPlik(pelnaSciezkaDoInterfejsu);

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
            ProjektWrapper projekt)
        {
            var interfaceBuilder =
                new InterfejsBuilder()
                    .ZNazwa("I" + nazwaKlasyService)
                    .ZModyfikatorem("public");

            var plikClassBuilder =
                new PlikClassBuilder()
                    .ZObiektem(interfaceBuilder)
                    .WNamespace(GenerujNamespace(projekt));

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
