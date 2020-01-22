using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Buildera.Komponenty;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class GenerowanieBuilderaTests
    {
        [Test]
        public void GenerujeBuilderGdyNieIstnieje()
        {
            //arrange
            var mockParametrow = new Mock<IParametryGenerowaniaBuildera>();
            mockParametrow.Setup(o => o.NazwaInterfejsuService).Returns("IDomainService");

            using (var projektZDomainObjectem = new ProjektWrapper("a1"))
            {
                var zawartoscDomain =
                    new WczytywaczZawartosciPrzykladow()
                        .DajZawartoscPrzykladu("DomainObject.cs");

                var plikZDomainObjectem =
                    new PlikWrapper(
                        "DomainObject.cs",
                        "Domain",
                        projektZDomainObjectem,
                        zawartoscDomain);

                using (var projektTestow = new ProjektWrapper("a1.tests"))
                {
                    var solution = new SolutionWrapper(projektZDomainObjectem, zawartoscDomain);
                    solution.DodajProjekt(projektZDomainObjectem);
                    solution.DodajProjekt(projektTestow);

                    var solutionExplorer = new SolutionExlorerWrapper(solution);

                    //act
                    new GenerowanieBuildera(solution, solutionExplorer)
                        .Generuj(mockParametrow.Object);

                    //assert
                    var sciezkaDoBuildera =
                        Path.Combine(
                            projektTestow.SciezkaDoKatalogu,
                            "Builders",
                            "DomainObjectBuilder.cs");

                    solutionExplorer.OtwartyPlik.Should().Be(sciezkaDoBuildera);

                    projektTestow.Pliki
                        .Where(o => o.Nazwa == "DomainObjectBuilder.cs")
                            .Should().ContainSingle();

                    solution.AktualnyDokument.DajZawartosc()
                        .Should().Be(File.ReadAllText(sciezkaDoBuildera, Encoding.UTF8));
                }
            }
        }
    }
}
