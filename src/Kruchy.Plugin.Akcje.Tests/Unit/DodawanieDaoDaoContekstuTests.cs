using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Pincasso.Akcje.Akcje;
using Kruchy.Plugin.Utils.Wrappers;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class DodawanieDaoDaoContekstuTests
    {
        [Test]
        public void DodajeDaoDoPustegoContekstu()
        {
            UruchamianieTestowExtensions.UruchomTest(
                "SamochodDomain.cs",
                (solution, projekt, solutionExplorer) =>
                {
                    new DodawanieDaoDaoContekstu(solution, solutionExplorer)
                        .Dodaj();
                },
                (solution, projekt, solutionExplorer) =>
                {
                    solution.AktualnyPlik.Nazwa.Should().Be("KruchyContext.cs");

                    var sciezkaDoContext =
                    Path.Combine(projekt.SciezkaDoKatalogu, "Base", "KruchyContext.cs");

                    var sciezkaDoIContext =
                    Path.Combine(projekt.SciezkaDoKatalogu, "Base", "IKruchyContext.cs");

                    solution.AktualnyDokument.DajZawartosc().Should().Be(
@"using Kruchy.Projekt1.Dao;
using Kruchy.Projekt1.Dao.Impl;
using Pincasso.Core.Base;

namespace KruchyProjekt.Base
{
    class KruchyContext : PincassoBaseContext, IKruchyContext
    {
        public ISamochodDomainDao SamochodDomainDao { get { return GetDao<SamochodDomainDao>(); } }
    }
}");

                    ((SolutionExlorerWrapper)solutionExplorer)
                        .PoprzednieZawartosciDokumentow.Last()
                            .Should().Be(
@"using Kruchy.Projekt1.Dao;
using Piatka.Core.Base;

namespace KruchyProjekt.Base
{
    public interface IKruchyContext : IPiatkaContext
    {
        ISamochodDomainDao SamochodDomainDao { get; }
    }
}");

                },
                (soltuion, projekt, solutionExplorer) =>
                {
                    DodajPlikiContekstu(projekt);
                    DodajPlikiDao(projekt);
                    solutionExplorer.OpenFile(
                        projekt.Pliki.First(o => o.SciezkaPelna.EndsWith("Dao.cs")));
                },
                null);

            //arrange

            //act

            //assert
        }

        private void DodajPlikiDao(ProjektWrapper projekt)
        {
            var katalogDao = Path.Combine(projekt.SciezkaDoKatalogu, "Dao");
            var katalogDaoImpl = Path.Combine(katalogDao, "Impl");

            Directory.CreateDirectory(katalogDaoImpl);

            var plikBuilder =
                new PlikClassBuilder()
                    .WNamespace("Kruchy.Projekt1.Dao")
                    .ZNazwa("ISamochodDomainDao")
                    .ZNazwaRodzajuObiektu("interface")
                    .ZObiektem(new InterfejsBuilder().ZNazwa("ISamochodDomainDao"));
            var zawartoscIDao = plikBuilder.Build();

            var sciezkDoIDao = Path.Combine(katalogDao, "ISamochodDomainDao.cs");
            File.WriteAllText(sciezkDoIDao, zawartoscIDao);
            projekt.DodajPlik(sciezkDoIDao);

            var plikDaoImplBuilder =
                new PlikClassBuilder()
                    .WNamespace("Kruchy.Projekt1.Dao.Impl")
                    .ZNazwa("SamochodDomainDao")
                    .ZNazwaRodzajuObiektu("class")
                    .ZObiektem(new ClassBuilder().ZNazwa("SamochodDomainDao"));
            var zawartoscDao = plikDaoImplBuilder.Build();

            var sciezkaDoDaoImpl = Path.Combine(katalogDaoImpl, "SamochodDomainDao.cs");
            File.WriteAllText(sciezkaDoDaoImpl, zawartoscDao);
            projekt.DodajPlik(sciezkaDoDaoImpl);

        }

        private void DodajPlikiContekstu(ProjektWrapper projekt, bool pusty = true)
        {
            var builderContekstu = new StringBuilder();
            builderContekstu.Append(
@"using Pincasso.Core.Base;

namespace KruchyProjekt.Base
{
    class KruchyContext : PincassoBaseContext, IKruchyContext
    {
");

            builderContekstu.Append(
@"    }
}");
            var sciezkaDoKataloguZKontekstami = Path.Combine(projekt.SciezkaDoKatalogu, "Base");
            var sciezkaDoContext = Path.Combine(sciezkaDoKataloguZKontekstami, "KruchyContext.cs");
            var sciezkaDoIContext = Path.Combine(sciezkaDoKataloguZKontekstami, "IKruchyContext.cs");
            Directory.CreateDirectory(sciezkaDoKataloguZKontekstami);
            File.WriteAllText(sciezkaDoContext, builderContekstu.ToString(), Encoding.UTF8);
            projekt.DodajPlik(sciezkaDoContext);

            var builderIContekstu = new StringBuilder();

            builderIContekstu.Append(
@"using Piatka.Core.Base;

namespace KruchyProjekt.Base
{
    public interface IKruchyContext : IPiatkaContext
    {
");

            builderIContekstu.Append(
@"    }
}");

            File.WriteAllText(sciezkaDoIContext, builderIContekstu.ToString());
            projekt.DodajPlik(sciezkaDoIContext);
        }

        private void SprawdzZawartosc(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer,
            string sciezka,
            string oczekiwanaZawartosc)
        {
            solutionExplorer.OpenFile(sciezka);
            var zawartosc = solution.AktualnyDokument.DajZawartosc();

            zawartosc.Should().Be(oczekiwanaZawartosc);
        }
    }
}