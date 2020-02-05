using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Kruchy.Plugin.Akcje.Akcje;
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

        [SetUp]
        public void SetUpEachTest()
        {
            wczytywacz = new WczytywaczZawartosciPrzykladow();
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
                        Path.Combine(projekt.SciezkaDoKatalogu, schematKlasy.NazwaPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be(schematKlasy.Tresc);
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
                        Path.Combine(projekt.SciezkaDoKatalogu, schematKlasy.NazwaPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be(
                        "a PustaKlasa b Kruchy.Plugin.Akcje.Tests.Samples c PustaKlasa.cs d PustaKlasa");
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
                        Path.Combine(projekt.SciezkaDoKatalogu, "PustaKlasaDao.cs");
                    projekt.Pliki.Single(o => o.SciezkaPelna == sciezkaDoPliku);

                    File.ReadAllText(sciezkaDoPliku).Should().Be("a");
                });
        }

        private void UruchomTest(
            string nazwaZasobuZawartosciAktulnegoPliku,
            SchematGenerowania schematGenerowania,
            Action<IProjektWrapper> akcjaAssert)
        {
            using (var projekt = new ProjektWrapper("kruchy.projekt1"))
            {
                //arrange
                var zawartosc =
                    wczytywacz
                        .DajZawartoscPrzykladu(nazwaZasobuZawartosciAktulnegoPliku);

                var solution = new SolutionWrapper(projekt, zawartosc);
                var plik = new PlikWrapper(nazwaZasobuZawartosciAktulnegoPliku, projekt);
                solution.OtworzPlik(plik.SciezkaPelna);

                PrzygotujKonfiguracjeWgSolutionISzablonu(solution, schematGenerowania);

                //act
                new GenerowaniePlikuZSzablonu(null, solution).Generuj("test1");

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