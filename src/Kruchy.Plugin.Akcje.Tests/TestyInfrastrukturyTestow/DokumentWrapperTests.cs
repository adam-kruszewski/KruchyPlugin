using System.Text;
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
            dokument.WstawWMiejscu("abc", 1, 1);

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

        [Test]
        public void WstawiaWLiniiTeskstZEnterem()
        {
            //act
            dokument.WstawWMiejscu("abc" + new StringBuilder().AppendLine().ToString(), 1, 1);

            //assert
            dokument.DajZawartosc()
                .Should()
                    .Be(
@"abc
namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class PustaKlasa
    {
    }
}");
        }

        [Test]
        public void WstawiaWSrodkuLinii()
        {
            //act
            dokument.WstawWMiejscu("abc", 1, 3);

            //assert
            dokument.DajZawartosc()
                .Should()
                    .Be(
@"naabcmespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class PustaKlasa
    {
    }
}");
        }

        [Test]
        public void UsuwaUsingi()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using c.d.e;
using g.h.i;

namespace x.y.z{
  class A {}
}";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 1, 3, 13);

            //assert
            dokument.DajZawartosc().Should().Be(
@"

namespace x.y.z{
  class A {}
}");
        }

        [Test]
        public void UsuwaTekstZLinii()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using c.d.e;
using g.h.i;

namespace x.y.z{
  class A {}
}";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 2, 1, 4);

            //assert
            dokument.DajZawartosc().Should().Be(
@"ug a.b.c;
using c.d.e;
using g.h.i;

namespace x.y.z{
  class A {}
}");
        }

        [Test]
        public void UsuwaTekstZPoczatkuLinii()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using c.d.e;";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 1, 1, 4);

            //assert
            dokument.DajZawartosc().Should().Be(
@"g a.b.c;
using c.d.e;");
        }

        [Test]
        public void NieUsuwaKoncaLiniiJesliPodanyKoncowyIndeksSlashR()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using c.d.e;";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 1, 1, 13);

            //assert
            dokument.DajZawartosc().Should().Be(
@"
using c.d.e;");
        }

        [Test]
        public void UsuwaCalaLinieJesliPodanyKoncowyIndeksSlashN()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using c.d.e;";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 1, 1, 14);

            //assert
            dokument.DajZawartosc().Should().Be(
@"using c.d.e;");
        }

        [Test]
        public void UsuwaWielolinijkowe()
        {
            //arrange
            var zrodlo =
@"using a.b.c;
using b.c.d;

namespace X{
    class a
    {
    }
}";

            var dokument = new DokumentWrapper(zrodlo);

            //act
            dokument.Usun(1, 1, 2, 13);

            //assert
            dokument.DajZawartosc().Should().Be(
@"

namespace X{
    class a
    {
    }
}");
        }
    }
}