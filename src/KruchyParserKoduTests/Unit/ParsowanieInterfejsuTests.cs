using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieInterfejsuTests
    {
        [Test]
        public void ParsujeInterfejs()
        {
            //act
            var sparsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("InterfejsDoParsowania.cs"));

            //assert
            var interfejs = sparsowane.DefiniowaneObiekty.Single();

            interfejs.Rodzaj = RodzajObiektu.Interfejs;
            interfejs.Nazwa = "InterfejsDoParsowania";
            interfejs.Owner.Should().BeNull();

            interfejs.Konstruktory.Should().BeEmpty();
            interfejs.Pola.Should().BeEmpty();

            interfejs.Propertiesy.Should().HaveCount(2);
            interfejs.Metody.Should().HaveCount(1);

            interfejs.Atrybuty.Should().BeEmpty();
            interfejs.NadklasaIInterfejsy.Should().BeEmpty();
            interfejs.ObiektyWewnetrzne.Should().BeEmpty();

            interfejs.PoczatkowaKlamerka.Sprawdz(7, 5);
            interfejs.KoncowaKlamerka.Sprawdz(12, 5);

            interfejs.Poczatek.Sprawdz(6, 5);
            interfejs.Koniec.Sprawdz(12, 6);
        }

        [Test]
        public void ParsujeInterfejsZDziedziczeniemIAtrybutami()
        {
            //act
            var sparsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("InterfejsZDziedziczeniemIAtrybutami.cs"));

            //assert
            var interfejs = sparsowane.DefiniowaneObiekty.Single();

            interfejs.NadklasaIInterfejsy.Should().HaveCount(2);

            interfejs.NadklasaIInterfejsy[0].Nazwa.Should().Be("InterfejsDoParsowania");
            interfejs.NadklasaIInterfejsy[0].NazwyTypowParametrow.Should().BeEmpty();
            interfejs.NadklasaIInterfejsy[0].Poczatek.Sprawdz(5, 53);
            interfejs.NadklasaIInterfejsy[0].Koniec.Sprawdz(5, 74);

            interfejs.NadklasaIInterfejsy[1].Nazwa.Should().Be("Interfejs2");
            interfejs.NadklasaIInterfejsy[1].NazwyTypowParametrow.Should().HaveCount(2);
            var parametryTypu = interfejs.NadklasaIInterfejsy[1].NazwyTypowParametrow;
            parametryTypu[0].Should().Be("Klasa1");
            parametryTypu[1].Should().Be("int?");
            interfejs.NadklasaIInterfejsy[1].Poczatek.Sprawdz(5, 76);
            interfejs.NadklasaIInterfejsy[1].Koniec.Sprawdz(5, 100);
        }
    }
}
