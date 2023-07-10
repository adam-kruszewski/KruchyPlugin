using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnianieKonstruktoraZDziedziczeniemTests
    {
        [Test]
        public void UzupelniaKonstruktorGdyKlasaDziedziczy()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraPrzyNadklasie.cs"));

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var zawartoscPoZmianie = solution.CurenctDocument.GetContent();

            zawartoscPoZmianie.Should().Be(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class UzupelnianieKonstruktoraPrzyNadklasie : Klasa1
    {
        private readonly string serwis1;
        private readonly string serwis2;

        public UzupelnianieKonstruktoraPrzyNadklasie(
            string serwis1,
            string serwis2,
            string serwis0) : base(serwis0)
        {
            this.serwis1 = serwis1;
            this.serwis2 = serwis2;
        }

    }
}
");
        }

        [Test]
        public void UzupełniaKonstruktorGdyJestZdefiniowaStalaNiePusjacKodu()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraPrzyStalej.cs"));
            //act
            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var zawartoscPoZmianie = solution.CurenctDocument.GetContent();
            //TODO trzeba poprawić mocki zawartości dokumentu - bo na żywo działa chyba dobrze
            zawartoscPoZmianie.Should().Be(
@"using System;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    class UzupelnianieKonstruktoraPrzyStalej
    {
        private const int DefaultTimeout = 10000;

        private readonly Class1 appSettingsService;

        public UzupelnianieKonstruktoraPrzyStalej(
            Class1 appSettingsService)
        {
            this.appSettingsService = appSettingsService;
        }

        public int TimeoutInMilliseconds()
        {
            return 1;
        }
    }
}");
        }
    }
}
