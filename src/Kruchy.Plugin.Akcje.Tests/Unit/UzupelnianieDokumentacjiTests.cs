using Kruchy.Plugin.Akcje.Akcje;
using FluentAssertions;
using NUnit.Framework;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnianieDokumentacjiTests
    {
        WczytywaczZawartosciPrzykladow wczytywacz;

        [SetUp]
        public void SetUpEachTest()
        {
            wczytywacz = new WczytywaczZawartosciPrzykladow();
        }

        [Test]
        public void DodajeDokumentacjeJesliNieByloZadnej()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacji.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 1);

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasyDoDokumentacji.cs"));
        }

        [Test]
        public void DodajeOpisParametrowGenerycznych()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaGenertycznaZMetodaGeneryczna.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 1);

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasaGenertycznaZMetodaGeneryczna.cs"));
        }

        private void PrzygotujKonfiguracjeWgSolutionISzablonu(
            SolutionWrapper solution,
            int jezyk)
        {

            var mockKonfiguracja = new Mock<Konfiguracja>();
            mockKonfiguracja.Setup(o => o.Dokumentacja())
                .Returns(new Dokumentacja
                {
                    Jezyk = 1
                });

            mockKonfiguracja.SetupGet(o => o.Solution).Returns(solution);

            Konfiguracja.SetInstance(mockKonfiguracja.Object);
        }
    }
}