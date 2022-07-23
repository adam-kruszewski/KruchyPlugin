using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnienieKonstruktoraPolaZPodkresleniemTests
    {
        [Test]
        public void GenerujeNowyKonstruktor()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraGdyPolaZPodkresleniem.cs"));

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var zawartoscPoZmianie = solution.AktualnyDokument.GetContent();

            zawartoscPoZmianie.Should().Be(
@"namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class UzupelnianieKonstruktoraGdyPolaZPodkresleniem
    {
        private readonly string _pole1;
        private readonly string _pole2;

        public UzupelnianieKonstruktoraGdyPolaZPodkresleniem(
            string pole1,
            string pole2)
        {
            _pole1 = pole1;
            _pole2 = pole2;
        }
    }
}");
        }

        [Test]
        public void DodajeNowePoleDoKonstruktora()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraGdyPolaZPodkresleniemPrzegenerowanie.cs"));

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var zawartoscPoZmianie = solution.AktualnyDokument.GetContent();

            zawartoscPoZmianie.Should().Be(
@"namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class UzupelnianieKonstruktoraGdyPolaZPodkresleniemPrzegenerowanie
    {
        private readonly string _pole1;
        private readonly string _pole2;

        public UzupelnianieKonstruktoraGdyPolaZPodkresleniemPrzegenerowanie(
            string pole1,
            string pole2)
        {
            _pole1 = pole1;
            _pole2 = pole2;
        }

    }
}");
        }
    }
}
