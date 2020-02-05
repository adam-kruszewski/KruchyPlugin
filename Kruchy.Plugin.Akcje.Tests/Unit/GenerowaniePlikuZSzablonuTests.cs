using Kruchy.Plugin.Akcje.Akcje;
using FluentAssertions;
using NUnit.Framework;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Moq;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using System.Collections.Generic;
using System.IO;
using Kruchy.Plugin.Utils.Wrappers;
using System;

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