using Kruchy.Plugin.Akcje.Akcje;
using FluentAssertions;
using NUnit.Framework;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Xsd.Komponenty;
using System.IO;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class GenerowanieXsdDlaReportViewTests
    {
        WczytywaczZawartosciPrzykladow wczytywacz;

        [SetUp]
        public void SetUpEachTest()
        {
            wczytywacz = new WczytywaczZawartosciPrzykladow();
        }


        [Test]
        public void GenerujeXsdJakoNowy()
        {
            //arrange
            var zawartoscView = wczytywacz.DajZawartoscPrzykladu("ReportView.cs");

            using (var projekt = new ProjektWrapper("a1"))
            {
                var solution = new SolutionWrapper(zawartoscView);
                solution.AktualnyProjekt = projekt;

                var plikZView = new PlikWrapper(
                    "ReportView.cs",
                    "Views",
                    projekt,
                    zawartoscView);

                new PlikWrapper("NierozksiegowanaWplataReportView.cs", projekt);
                new PlikWrapper("NiezaplaconaFakturaReportView.cs", projekt);

                solution.DodajProjekt(projekt);
                var solutionExplorer = new SolutionExlorerWrapper(solution);

                var mockParametrow = new Mock<IParametryGenerowaniaXsd>();

                var sciezkaDoXsd =
                    Path.Combine(projekt.SciezkaDoKatalogu, "PotwierdzenieSaldaReportView.xsd");
                mockParametrow.Setup(o => o.SciezkaDoXsd).Returns(sciezkaDoXsd);
                //act
                new GenerowanieXsdDlaReportView(solution, solutionExplorer)
                    .Generuj(mockParametrow.Object);

                //assert
                var zawartosc = solution.AktualnyDokument.DajZawartosc();

                zawartosc.Should().Be(wczytywacz.DajZawartoscPrzykladu("ReportViewXsd.xsd"));
                solution.AktualnyPlik.SciezkaPelna.Should().Be(sciezkaDoXsd);
            }
        }
    }
}