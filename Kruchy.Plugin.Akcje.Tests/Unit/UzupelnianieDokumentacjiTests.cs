using Kruchy.Plugin.Akcje.Akcje;
using FluentAssertions;
using NUnit.Framework;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;

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

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasyDoDokumentacji.cs"));
        }
    }
}