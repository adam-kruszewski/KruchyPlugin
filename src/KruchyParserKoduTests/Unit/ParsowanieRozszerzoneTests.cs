using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieRozszerzoneTests
    {
        Plik parsowane;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowaniaRozszerzone.cs"));
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

        [Test]
        public void RozpoznajeParam()
        {
            //arrange
            var metoda = parsowane.DefiniowaneObiekty.First().Metody[1];

            //assert
            metoda.Parametry.Should().HaveCount(2);

            var parametr = metoda.Parametry[1];
            parametr.ZParams.Should().BeTrue();
        }
    }
}