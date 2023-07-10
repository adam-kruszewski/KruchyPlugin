using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class DodawanieUsingaTests
    {
        [Test]
        public void DodajeUsingaJesliNieByloZadnegoUsinga()
        {
            //arrange
            var startowe =
@"namespace X{
    class a
    {
    }
}";

            var solution = new SolutionWrapper(startowe);

            //act
            new DodawanieUsinga(solution).Dodaj("a.b.c");

            //assert
            solution.CurentDocument.GetContent().Should().Be(
@"using a.b.c;namespace X{
    class a
    {
    }
}");
        }

        [Test]
        [Ignore("To verify")]
        public void DodajeUsingaJesliNieByloZadnegoUsingaIPustaLiniaPrzedNamespace()
        {
            //arrange
            var startowe =
@"
namespace X{
    class a
    {
    }
}";

            var solution = new SolutionWrapper(startowe);

            //act
            new DodawanieUsinga(solution).Dodaj("a.b.c");

            //assert
            solution.CurentDocument.GetContent().Should().Be(
@"using a.b.c;

namespace X{
    class a
    {
    }
}");
        }

        [Test]
        public void DodajeUsingaJesliJuzBylJakisInny()
        {
            //arrange
            var startowe =
@"using a.b.c;

namespace X{
    class a
    {
    }
}";

            var solution = new SolutionWrapper(startowe);

            //act
            new DodawanieUsinga(solution).Dodaj("b.c.d");

            //assert
            solution.CurentDocument.GetContent().Should().Be(
@"using a.b.c;
using b.c.d;

namespace X{
    class a
    {
    }
}");
        }

        [Test]
        public void DodajeDwaRazyUsinga()
        {
            //arrange
            var startowe =
@"using a.b.c;

namespace X{
    class a
    {
    }
}";

            var solution = new SolutionWrapper(startowe);

            //act
            new DodawanieUsinga(solution).Dodaj("b.c.d");
            new DodawanieUsinga(solution).Dodaj("c.d.e");

            //assert
            solution.CurentDocument.GetContent().Should().Be(
@"using a.b.c;
using b.c.d;
using c.d.e;

namespace X{
    class a
    {
    }
}");
        }


        [Test]
        public void DodajeUsingaJesliDotychczasZdefiniowaneWiecejNizJeden()
        {
            //arrange
            var startowe =
    @"using a.b.c
using b.c.d;

namespace X{
    class a
    {
    }
}";

            var solution = new SolutionWrapper(startowe);

            //act
            new DodawanieUsinga(solution).Dodaj("c.d.e");

            //assert
            solution.CurentDocument.GetContent().Should().Be(
    @"using a.b.c;
using b.c.d;
using c.d.e;

namespace X{
    class a
    {
    }
}");
        }

        [Test]
        public void Test1()
        {
            //arrange
            var startowe =
    @"using System.Linq;
using ClassLibrary1.Domain;
using ClassLibrary1.Validation;
using FluentAssertions;

namespace ClassLibrary1.Tests.Unit
{
    [TestFixture, Category(TestCategories.Unit)]
    public class Nadklasa1Tests : ServiceTests<INadklasa1>
    {
    }
}";
            string[] usingiDoDodania =
            {
"Piatka.Infrastructure.Mappings",
"FluentAssertions",
"Piatka.Infrastructure.Tests.Builders",
"System.Linq",
"Piatka.Infrastructure.QueryBuilder",
"Pincasso.Core.Dao",
"Piatka.Infrastructure.Tests.Assertions",
"ClassLibrary1.Validation",
"ClassLibrary1.Domain"
            };

            var solution = new SolutionWrapper(startowe);

            //act
            var du = new DodawanieUsinga(solution);

            foreach (var u in usingiDoDodania)
                du.Dodaj(u);

            //assert
            var wynik = solution.CurentDocument.GetContent();
        }
    }
}