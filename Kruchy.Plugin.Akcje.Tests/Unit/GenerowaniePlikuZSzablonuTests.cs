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

        private void UruchomTest(
            string nazwaZasobuZawartosciAktulnegoPliku,
            SchematGenerowania schematGenerowania,
            Action<IProjektWrapper> akcjaAssert)
        {
            using (var projekt = new ProjektWrapper("kruchy.projekt1"))
            {
                //arrange
                var zawartoscPustaKlasa =
                    wczytywacz
                        .DajZawartoscPrzykladu(nazwaZasobuZawartosciAktulnegoPliku);

                var solution = new SolutionWrapper(projekt, zawartoscPustaKlasa);

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