using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyPlugin1Tests.Utils;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    [TestFixture]
    public class ParsowanieKlasyNadklasIInterfejsowTests
    {
        [Test]
        public void PoprawnieParsuje()
        {
            //arrange
            var zawartosc = File.ReadAllText(DajSciezkeTestu());
            //act
            var plik = Parser.Parsuj(zawartosc);

            //assert
            var klasa = plik.DefiniowaneObiekty.First();

            klasa.NadklasaIInterfejsy.Count().Should().Be(3);
            klasa.NadklasaIInterfejsy.First().Nazwa.Should().Be("Test3");
            klasa.NadklasaIInterfejsy[1].Nazwa.Should().Be("ITest1");
            klasa.NadklasaIInterfejsy[2].Nazwa.Should().Be("ITest2");
            klasa.NadklasaIInterfejsy[2].Poczatek.Sprawdz(9, 65);
            klasa.NadklasaIInterfejsy[2].Koniec.Sprawdz(9, 71);
        }

        private string DajSciezkeTestu()
        {
            var wynik = Path.Combine("..", "..", "ParserTests", "KlasaDoParsowaniaNadklasIInterfejsow.cs");
            var fi = new FileInfo(wynik);
            return wynik;
        }
    }
}
