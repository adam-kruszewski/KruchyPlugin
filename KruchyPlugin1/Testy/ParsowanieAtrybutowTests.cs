using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    [TestFixture]
    class ParsowanieAtrybutowTests
    {
        Plik parsowane;
        IList<Metoda> metody;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(File.ReadAllText(DajSciezkeTestu()));
            metody = parsowane.DefiniowaneObiekty.First().Metody;
        }

        private string DajSciezkeTestu()
        {
            return Path.Combine("..", "..", "Testy", "KlasaDoParsowaniaAtrybutow.cs");
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWJednejLinii()
        {
            //arrange
            var metoda1 = metody.First();
            //assert
            metoda1.Atrybuty.Count().Should().Be(2);
            metoda1.Atrybuty[0].Nazwa.Should().Be("Testowo");
            metoda1.Atrybuty[0].Poczatek.Sprawdz(7, 10);
            metoda1.Atrybuty[0].Koniec.Sprawdz(7, 17);
            metoda1.Atrybuty[1].Nazwa.Should().Be("Testowo3");
            metoda1.Atrybuty[1].Poczatek.Sprawdz(7, 19);
            metoda1.Atrybuty[1].Koniec.Sprawdz(7, 27);
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWOddzielnychLiniachWTymJedenZParametrem()
        {
            //arrange
            var metoda2 = metody[1];

            //assert
            metoda2.Atrybuty.Count().Should().Be(2);
            var atrybutTestowo2 = metoda2.Atrybuty[0];
            atrybutTestowo2.Nazwa.Should().Be("Testowo2");
            atrybutTestowo2.Poczatek.Sprawdz(13, 10);
            atrybutTestowo2.Koniec.Sprawdz(13, 32);
            atrybutTestowo2.Parametry.Count().Should().Be(1);
            var parametr = atrybutTestowo2.Parametry.First();
            parametr.Nazwa.Should().Be("Param");
            parametr.Wartosc.Should().Be("aa");

            metoda2.Atrybuty[1].Nazwa.Should().Be("Testowo3");
            metoda2.Atrybuty[1].Poczatek.Sprawdz(14, 10);
            metoda2.Atrybuty[1].Koniec.Sprawdz(14, 18);
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWJednejLiniiWTymJedenZParametrem()
        {
            //arrange
            var metoda3 = metody[2];
            //assert
            metoda3.Atrybuty.Count().Should().Be(2);
            metoda3.Atrybuty[0].Nazwa.Should().Be("Testowo2");
            metoda3.Atrybuty[0].Poczatek.Sprawdz(20, 10);
            metoda3.Atrybuty[0].Koniec.Sprawdz(20, 32);
            metoda3.Atrybuty[1].Nazwa.Should().Be("Testowo");
            metoda3.Atrybuty[1].Poczatek.Sprawdz(20, 34);
            metoda3.Atrybuty[1].Koniec.Sprawdz(20, 41);
        }

        [Test]
        public void ZnajdujeJedenAtrybutZParametremPrzezKontruktor()
        {
            //arrange
            var metoda4 = metody[3];
            //assert
            metoda4.Atrybuty.Count().Should().Be(1);
            metoda4.Atrybuty[0].Nazwa.Should().Be("Testowo4");
            metoda4.Atrybuty[0].Poczatek.Sprawdz(26, 10);
            metoda4.Atrybuty[0].Koniec.Sprawdz(26, 21);
            metoda4.Atrybuty.Count().Should().Be(1);
            var p = metoda4.Atrybuty.First().Parametry.First();
            p.Nazwa.Should().Be("");
            p.Wartosc.Should().Be("1");
        }
    }
}
