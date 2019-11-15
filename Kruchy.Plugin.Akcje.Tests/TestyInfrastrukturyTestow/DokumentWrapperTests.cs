using FluentAssertions;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Utils.Wrappers;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.TestyInfrastrukturyTestow
{
    [TestFixture]
    public class DokumentWrapperTests
    {
        IDokumentWrapper dokument;

        [SetUp]
        public void SetUpEachTest()
        {
            dokument =
                new DokumentWrapper(
                    new WczytywaczZawartosciPrzykladow()
                        .DajZawartoscPrzykladu("PustaKlasa.cs"));
        }

        [Test]
        public void WstawiaWLinii()
        {
            //act
            dokument.WstawWMiejscu("abc", 1, 0);

            //assert
            dokument.DajZawartosc()
                .Should()
                    .Be(
@"abcnamespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class PustaKlasa
    {
    }
}");
        }
    }
}
