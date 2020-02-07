using System;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Utils.Wrappers;
using Moq;

namespace Kruchy.Plugin.Akcje.Tests.Utils
{
    public static class UruchamianieTestowExtensions
    {
        internal static void UruchomTest(
            string nazwaZasobuZawartosciAktulnegoPliku,
            Action<SolutionWrapper, ProjektWrapper, SolutionExlorerWrapper> akcjaWykonania,
            Action<ISolutionWrapper, IProjektWrapper, ISolutionExplorerWrapper> akcjaAssert,
            Action<SolutionWrapper, ProjektWrapper> akcjaDopasowaniaArrange = null,
            Action<Mock<Konfiguracja>> akcjaDopasowaniaKonfiguracji = null)
        {
            using (var projekt = new ProjektWrapper("Kruchy.Projekt1"))
            {
                //arrange
                var zawartosc =
                    new WczytywaczZawartosciPrzykladow()
                        .DajZawartoscPrzykladu(nazwaZasobuZawartosciAktulnegoPliku);

                var solution = new SolutionWrapper(projekt, zawartosc);
                var plik = new PlikWrapper(nazwaZasobuZawartosciAktulnegoPliku, projekt);
                solution.OtworzPlik(plik.SciezkaPelna);

                PrzygotujKonfiguracjeWgSolutionISzablonu(solution, akcjaDopasowaniaKonfiguracji);

                if (akcjaDopasowaniaArrange != null)
                    akcjaDopasowaniaArrange(solution, projekt);

                var solutionExplorer = new SolutionExlorerWrapper(solution);

                //act
                akcjaWykonania(solution, projekt, solutionExplorer);

                //assert
                akcjaAssert(solution, projekt, solutionExplorer);
            }
        }

        private static void PrzygotujKonfiguracjeWgSolutionISzablonu(
            SolutionWrapper solution,
            Action<Mock<Konfiguracja>> akcjaDopasowaniaKonfiguracji)
        {
            var mockKonfiguracja = new Mock<Konfiguracja>();

            if (akcjaDopasowaniaKonfiguracji != null)
                akcjaDopasowaniaKonfiguracji(mockKonfiguracja);

            Konfiguracja.SetInstance(mockKonfiguracja.Object);
        }
    }
}