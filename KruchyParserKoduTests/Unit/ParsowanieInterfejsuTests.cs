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
                new WczytywaczZawartosciPrzykladow().DajZawartoscPrzykladu("InterfejsDoParsowania.cs"));

            //assert
            var interfejs = sparsowane.DefiniowaneObiekty.Single();

            interfejs.Rodzaj = RodzajObiektu.Interfejs;
            interfejs.Nazwa = "InterfejsDoParsowania";
            interfejs.Wlasciciel.Should().BeNull();

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
    }
}
