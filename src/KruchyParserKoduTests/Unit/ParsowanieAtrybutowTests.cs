using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieAtrybutowTests
    {
        Plik parsowane;
        IList<Metoda> metody;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowaniaAtrybutow.cs"));
            metody = parsowane.DefiniowaneObiekty.First().Metody;
        }

        [Test]
        public void ZnajdujeAtrybutMapowan()
        {
            //arrange
            var klasa = parsowane.DefiniowaneObiekty.First();
            //assert
            klasa.Atrybuty.Count().Should().Be(2);
            var atrybutMapowan =
                klasa.Atrybuty.Where(o => o.Name == "Map").FirstOrDefault();
            atrybutMapowan.Should().NotBeNull();
            atrybutMapowan.Parameters.Count.Should().Be(1);
            atrybutMapowan.Parameters.First().Value
                .Should().Be("typeof(KlasaDoParsowaniaAtrybutow)");
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWJednejLinii()
        {
            //arrange
            var metoda1 = metody.First();
            //assert
            metoda1.Atrybuty.Count().Should().Be(2);
            metoda1.Atrybuty[0].Name.Should().Be("Testowo");
            metoda1.Atrybuty[0].Poczatek.Sprawdz(8, 10);
            metoda1.Atrybuty[0].Koniec.Sprawdz(8, 17);
            metoda1.Atrybuty[1].Name.Should().Be("Testowo3");
            metoda1.Atrybuty[1].Poczatek.Sprawdz(8, 19);
            metoda1.Atrybuty[1].Koniec.Sprawdz(8, 27);
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWOddzielnychLiniachWTymJedenZParametrem()
        {
            //arrange
            var metoda2 = metody[1];

            //assert
            metoda2.Atrybuty.Count().Should().Be(2);
            var atrybutTestowo2 = metoda2.Atrybuty[0];
            atrybutTestowo2.Name.Should().Be("Testowo2");
            atrybutTestowo2.Poczatek.Sprawdz(14, 10);
            atrybutTestowo2.Koniec.Sprawdz(14, 32);
            atrybutTestowo2.Parameters.Count().Should().Be(1);
            var parametr = atrybutTestowo2.Parameters.First();
            parametr.Name.Should().Be("Param");
            parametr.Value.Should().Be("\"aa\"");

            metoda2.Atrybuty[1].Name.Should().Be("Testowo3");
            metoda2.Atrybuty[1].Poczatek.Sprawdz(15, 10);
            metoda2.Atrybuty[1].Koniec.Sprawdz(15, 18);
        }

        [Test]
        public void ZnajdujeDwaAtrybutyWJednejLiniiWTymJedenZParametrem()
        {
            //arrange
            var metoda3 = metody[2];
            //assert
            metoda3.Atrybuty.Count().Should().Be(2);
            metoda3.Atrybuty[0].Name.Should().Be("Testowo2");
            metoda3.Atrybuty[0].Poczatek.Sprawdz(21, 10);
            metoda3.Atrybuty[0].Koniec.Sprawdz(21, 32);
            metoda3.Atrybuty[1].Name.Should().Be("Testowo");
            metoda3.Atrybuty[1].Poczatek.Sprawdz(21, 34);
            metoda3.Atrybuty[1].Koniec.Sprawdz(21, 41);
        }

        [Test]
        public void ZnajdujeJedenAtrybutZParametremPrzezKontruktor()
        {
            //arrange
            var metoda4 = metody[3];
            //assert
            metoda4.Atrybuty.Count().Should().Be(1);
            metoda4.Atrybuty[0].Name.Should().Be("Testowo4");
            metoda4.Atrybuty[0].Poczatek.Sprawdz(27, 10);
            metoda4.Atrybuty[0].Koniec.Sprawdz(27, 21);
            metoda4.Atrybuty.Count().Should().Be(1);
            var p = metoda4.Atrybuty.First().Parameters.First();
            p.Name.Should().Be("");
            p.Value.Should().Be("1");
        }
    }
}
