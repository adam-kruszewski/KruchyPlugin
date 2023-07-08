using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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
            var metoda = parsowane.DefiniowaneObiekty.First().Methods.First();

            //assert
            metoda.Parametry.Count.Should().Be(3);
            var pierwszyParametr = metoda.Parametry[0];
            pierwszyParametr.WithThis.Should().BeTrue();
            pierwszyParametr.ParameterName.Should().Be("liczba");
            pierwszyParametr.TypeName.Should().Be("int");
            pierwszyParametr.DefaultValue.Should().BeNullOrEmpty();
            var drugiParametr = metoda.Parametry[1];
            drugiParametr.WithThis.Should().BeFalse();
            drugiParametr.DefaultValue.Should().Be("2");
            drugiParametr.ParameterName.Should().Be("liczba2");
            drugiParametr.TypeName.Should().Be("int");
            var trzecieParametr = metoda.Parametry[2];
            trzecieParametr.DefaultValue.Should().Be("\"aa\"");
        }

        [Test]
        public void RozpoznajeRefIOut()
        {
            //arrange
            var metoda = parsowane.DefiniowaneObiekty.First().Methods[2];

            //assert
            metoda.Parametry.Should().HaveCount(2);

            var parametr1 = metoda.Parametry.First();
            parametr1.WithRef.Should().BeTrue();
            parametr1.WithOut.Should().BeFalse();
            parametr1.ParameterName.Should().Be("obiekt");

            var parametr2 = metoda.Parametry[1];
            parametr2.WithOut.Should().BeTrue();
            parametr2.WithRef.Should().BeFalse();
        }

        [Test]
        public void RozpoznajeParam()
        {
            //arrange
            var metoda = parsowane.DefiniowaneObiekty.First().Methods[1];

            //assert
            metoda.Parametry.Should().HaveCount(2);

            var parametr = metoda.Parametry[1];
            parametr.WithParams.Should().BeTrue();
        }
    }
}