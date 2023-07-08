using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKlasyNadklasIInterfejsowTests
    {
        [Test]
        public void PoprawnieParsuje()
        {
            //arrange
            var zawartosc =
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowaniaNadklasIInterfejsow.cs");

            //act
            var plik = Parser.Parsuj(zawartosc);

            //assert
            var klasa = plik.DefiniowaneObiekty.First();

            klasa.SuperClassAndInterfaces.Count().Should().Be(3);
            klasa.SuperClassAndInterfaces.First().Nazwa.Should().Be("Test3");
            klasa.SuperClassAndInterfaces[1].Nazwa.Should().Be("ITest1");
            klasa.SuperClassAndInterfaces[2].Nazwa.Should().Be("ITest2");
            klasa.SuperClassAndInterfaces[2].Poczatek.Sprawdz(9, 65);
            klasa.SuperClassAndInterfaces[2].Koniec.Sprawdz(9, 71);
        }
    }
}