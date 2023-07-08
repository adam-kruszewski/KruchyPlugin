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
            var klasa = plik.DefinedItems.First();

            klasa.SuperClassAndInterfaces.Count().Should().Be(3);
            klasa.SuperClassAndInterfaces.First().Name.Should().Be("Test3");
            klasa.SuperClassAndInterfaces[1].Name.Should().Be("ITest1");
            klasa.SuperClassAndInterfaces[2].Name.Should().Be("ITest2");
            klasa.SuperClassAndInterfaces[2].StartPosition.Sprawdz(9, 65);
            klasa.SuperClassAndInterfaces[2].EndPosition.Sprawdz(9, 71);
        }
    }
}