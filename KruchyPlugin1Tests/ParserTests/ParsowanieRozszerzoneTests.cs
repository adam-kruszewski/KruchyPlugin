using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    [TestFixture]
    public class ParsowanieRozszerzoneTests
    {
        Plik parsowane;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(File.ReadAllText(DajSciezkeTestu()));
        }

        [Test]
        public void RozpoznajeThis()
        {
            //arrange
            var metoda = parsowane.DefiniowaneObiekty.First().Metody.First();

            //assert
            metoda.Parametry.Count.Should().Be(3);
            var pierwszyParametr = metoda.Parametry[0];
            pierwszyParametr.ZThisem.Should().BeTrue();
            pierwszyParametr.NazwaParametru.Should().Be("liczba");
            pierwszyParametr.NazwaTypu.Should().Be("int");
            pierwszyParametr.WartoscDomyslna.Should().BeNullOrEmpty();
            var drugiParametr = metoda.Parametry[1];
            drugiParametr.ZThisem.Should().BeFalse();
            drugiParametr.WartoscDomyslna.Should().Be("2");
            drugiParametr.NazwaParametru.Should().Be("liczba2");
            drugiParametr.NazwaTypu.Should().Be("int");
            var trzecieParametr = metoda.Parametry[2];
            trzecieParametr.WartoscDomyslna.Should().Be("\"aa\"");
        }

        private string DajSciezkeTestu()
        {
            return Path.Combine("..", "..", "ParserTests", "KlasaDoParsowaniaRozszerzone.cs");
        }
    }
}
