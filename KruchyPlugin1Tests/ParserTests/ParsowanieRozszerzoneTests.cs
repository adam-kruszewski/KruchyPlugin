using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
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

        [Test]
        public void RozpoznajeRefIOut()
        {
            //arrange
            var metoda = parsowane.DefiniowaneObiekty.First().Metody[2];

            //assert
            metoda.Parametry.Should().HaveCount(2);

            var parametr1 = metoda.Parametry.First();
            parametr1.ZRef.Should().BeTrue();
            parametr1.ZOut.Should().BeFalse();
            parametr1.NazwaParametru.Should().Be("obiekt");

            var parametr2 = metoda.Parametry[1];
            parametr2.ZOut.Should().BeTrue();
            parametr2.ZRef.Should().BeFalse();
        }
    }
}
