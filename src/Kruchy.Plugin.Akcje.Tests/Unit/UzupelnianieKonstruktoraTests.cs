using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnianieKonstruktoraTests
    {
        [Test]
        public void UzupelniaGdySaPropertiesy()
        {
            //arrange
            var solution = new SolutionWrapper(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("UzupelnianieKonstruktoraGdyPropertiesy.cs"));

            UnitModuleInitialization.KonfiguracjaMock
                .Setup(o => o.SortowacZaleznosciSerwisu())
                .Returns(false);

            UnitModuleInitialization.SetSolutionToKonfiguracjaMock(solution);

            //act
            new UzupelnianieKonstruktora(solution).Uzupelnij();

            //assert
            var wynik = solution.AktualnyDokument.GetContent();

            wynik.Should().Be(
@"using System.Configuration;
using Piatka.Infrastructure.AppSettings;
using Pincasso.Administracja.Core.Services;
using Pincasso.Core.Utils;

namespace Pincasso.MvcApp.WebServices.Logging
{
    public class KonfiguracjaLogowaniaKomunikatowWebowych
         : IKonfiguracjaLogowaniaKomunikatowWebowych
    {
        private const string Prefix_Kluczy_Wartosci = ""KomunikatyWebSerwis:"";

        private const string Domyslny_Katalog_Komunikatow = ""web-service-messages"";

        private readonly IAppSettingsService appSettingsService;

        public KonfiguracjaLogowaniaKomunikatowWebowych(
            IAppSettingsService appSettingsService)
        {
            this.appSettingsService = appSettingsService;
        }

        public string KatalogKomunikatow
        {
            get
            {
                return SciezkiUtils.DajPelnaSciezke(wartosc);
            }
        }

        public int GlebokoscPodzialuNaKatalogi
        {
            get
            {
                return 2;
            }
        }

        public int? LiczbaGodzinPrzechowywania
        {
            get
            {
                return 1;
            }
        }
    }
}");
        }

        [TearDown]
        public void CleanUp()
        {
            UnitModuleInitialization.KonfiguracjaMock.Reset();
        }
    }
}