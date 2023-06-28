using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Utils.Wrappers;
using Moq;
using NUnit.Framework;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class GenerowaniePlikuZSzablonuTests
    {
        private WczytywaczZawartosciPrzykladow wczytywacz;

        Mock<IGeneratingFromTemplateParamsWindow> _uiDialogMock;

        [SetUp]
        public void SetUpEachTest()
        {
            wczytywacz = new WczytywaczZawartosciPrzykladow();

             _uiDialogMock = new Mock<IGeneratingFromTemplateParamsWindow>();

            _uiDialogMock
                .Setup(o => o.VariablesValues)
                .Returns(new Dictionary<string, object>());

            UnitModuleInitialization.UiFactoryMock
                .Setup(o => o.Get<IGeneratingFromTemplateParamsWindow>())
                .Returns(_uiDialogMock.Object);
        }

        [TearDown]
        public void TeardownEachTest()
        {
            UnitModuleInitialization.UiFactoryMock.Reset();
        }

        [Test]
        public void GenerujeKlaseBezZmiennych()
        {
            var szablon = new SchematGenerowania();
            szablon.TytulSchematu = "test1";

            var schematKlasy = new SchematKlasy();
            schematKlasy.Tresc = "aaa\nbbb";
            schematKlasy.NazwaPliku = "Klasa1.cs";
            szablon.SchematyKlas.Add(schematKlasy);

            UruchomTest(
                "PustaKlasa.cs",
                szablon,
                projekt =>
                {
                    var sciezkaDoPliku =
                        Path.Combine(projekt.DirectoryPath, schematKlasy.NazwaPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be(schematKlasy.Tresc);
                },
                akcjaDopasowaniaArrange: (solution, project) =>
                {
                    _uiDialogMock
                        .Setup(o => o.SelectedProject)
                        .Returns(project);
                    _uiDialogMock
                        .Setup(o => o.Directory)
                        .Returns($"{project.DirectoryPath}/abc");
                });
        }

        [Test]
        public void GenerujeKlaseZeZmiennymiWbudowanymi()
        {
            var szablon = new SchematGenerowania();
            szablon.TytulSchematu = "test1";

            var schematKlasy = new SchematKlasy();
            schematKlasy.Tresc = "a %NAZWA_KLASY% b %NAMESPACE_KLASY% c %NAZWA_PLIKU% d %NAZWA_PLIKU_BEZ_ROZSZERZENIA%";
            schematKlasy.NazwaPliku = "Klasa1.cs";
            szablon.SchematyKlas.Add(schematKlasy);

            UruchomTest(
                "PustaKlasa.cs",
                szablon,
                projekt =>
                {
                    var sciezkaDoPliku =
                        Path.Combine(projekt.DirectoryPath, schematKlasy.NazwaPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be(
                        "a PustaKlasa b Kruchy.Plugin.Akcje.Tests.Samples c PustaKlasa.cs d PustaKlasa");
                },
                akcjaDopasowaniaArrange: (solution, project) =>
                {
                    _uiDialogMock
                        .Setup(o => o.SelectedProject)
                        .Returns(project);
                    _uiDialogMock
                        .Setup(o => o.Directory)
                        .Returns($"{project.DirectoryPath}/abc");
                });
        }

        [Test]
        public void GnerujeKlaseZeZmiennymiWNazwiePliku()
        {
            var szablon = new SchematGenerowania();
            szablon.TytulSchematu = "test1";

            var schematKlasy = new SchematKlasy();
            schematKlasy.Tresc = "a";
            schematKlasy.NazwaPliku = "%NAZWA_KLASY%Dao.cs";
            szablon.SchematyKlas.Add(schematKlasy);

            UruchomTest(
                "PustaKlasa.cs",
                szablon,
                projekt =>
                {
                    var sciezkaDoPliku =
                        Path.Combine(projekt.DirectoryPath, "PustaKlasaDao.cs");
                    projekt.Files.Single(o => o.FullPath == sciezkaDoPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be("a");
                },
                akcjaDopasowaniaArrange: (solution, project) =>
                {
                    _uiDialogMock
                        .Setup(o => o.SelectedProject)
                        .Returns(project);
                    _uiDialogMock
                        .Setup(o => o.Directory)
                        .Returns($"{project.DirectoryPath}/abc");
                });
        }

        [Test]
        [Ignore("To verify")]
        public void GenerujeTrescZDynamicznaZmienna()
        {
            var szablon = new SchematGenerowania();
            szablon.TytulSchematu = "test1";

            var schematKlasy = new SchematKlasy();
            schematKlasy.Tresc = "a %KlasaContext%";
            schematKlasy.NazwaPliku = "ADao.cs";
            //schematKlasy.Zmienne.Add(
            //    new Zmienna
            //    {
            //        BezRozszerzenia = true,
            //        DopasowaniePliku = ".Context.cs",
            //        Symbol = "KlasaContext"
            //    });
            szablon.SchematyKlas.Add(schematKlasy);

            var konf = new KonfiguracjaPlugina.Xml.KruchyPlugin();
            konf.Schematy.Add(szablon);

            var s = new XmlSerializer(typeof(KruchyPlugin));
            var tw = new StringWriter();
            s.Serialize(tw, konf);
            var a = tw.ToString();

            UruchomTest(
                "PustaKlasa.cs",
                szablon,
                projekt =>
                {
                    var sciezkaDoPliku =
                        Path.Combine(projekt.DirectoryPath, "ADao.cs");
                    projekt.Files.Single(o => o.FullPath == sciezkaDoPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be("a ModulContext");
                },
                (solution, projekt) =>
                {
                    _uiDialogMock
                        .Setup(o => o.SelectedProject)
                        .Returns(projekt);
                    _uiDialogMock
                        .Setup(o => o.Directory)
                        .Returns($"{projekt.DirectoryPath}/abc");

                    projekt.DodajPustyPlik("ModulContext.cs");
                    projekt.DodajPustyPlik("ModulContextScope.cs");
                });
        }

        private void UruchomTest(
            string nazwaZasobuZawartosciAktulnegoPliku,
            SchematGenerowania schematGenerowania,
            Action<IProjectWrapper> akcjaAssert,
            Action<SolutionWrapper, ProjektWrapper> akcjaDopasowaniaArrange = null)
        {
            using (var projekt = new ProjektWrapper("kruchy.projekt1"))
            {
                //arrange
                var zawartosc =
                    wczytywacz
                        .DajZawartoscPrzykladu(nazwaZasobuZawartosciAktulnegoPliku);

                var solution = new SolutionWrapper(projekt, zawartosc);
                var plik = new PlikWrapper(nazwaZasobuZawartosciAktulnegoPliku, projekt);
                solution.OtworzPlik(plik.FullPath);

                PrzygotujKonfiguracjeWgSolutionISzablonu(solution, schematGenerowania);

                if (akcjaDopasowaniaArrange != null)
                    akcjaDopasowaniaArrange(solution, projekt);

                //act
                new GenerowaniePlikuZSzablonu(new SolutionExlorerWrapper(solution), solution).Generuj("test1");

                //assert
                akcjaAssert(projekt);
            }
        }

        private void PrzygotujKonfiguracjeWgSolutionISzablonu(
            SolutionWrapper solution,
            SchematGenerowania szablon)
        {
            var listaSchematow = new List<SchematGenerowania>();
            listaSchematow.Add(szablon);

            var mockKonfiguracja = new Mock<Konfiguracja>();
            mockKonfiguracja.Setup(o => o.SchematyGenerowania())
                .Returns(listaSchematow);
            mockKonfiguracja.SetupGet(o => o.Solution).Returns(solution);

            Konfiguracja.SetInstance(mockKonfiguracja.Object);
        }
    }
}