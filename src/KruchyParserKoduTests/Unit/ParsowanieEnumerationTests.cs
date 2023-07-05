using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieEnumerationTests
    {
        Plik parsowane;

        Enumeration enumeration;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("EnumerationDoParsowania.cs"));

            enumeration = parsowane.DefiniowaneEnumeracje.Single();
        }

        [Test]
        public void ParsujeNazweIPozycjeEnumeracji()
        {
            enumeration.Name.Should().Be("Enum1");
            enumeration.Poczatek.Sprawdz(6, 5);
            enumeration.Koniec.Sprawdz(12, 6);
        }

        [Test]
        public void ParsujeAtrybutyEnumeracji()
        {
            var atrybut = enumeration.Atrybuty.Single();

            atrybut.Name.Should().Be("Serializable");
            atrybut.Poczatek.Sprawdz(6, 6);
            atrybut.Koniec.Sprawdz(6, 18);
            atrybut.Parameters.Should().BeNullOrEmpty();
        }

        [Test]
        public void ParsujePola()
        {
            var pola = enumeration.Pola;

            pola.Should().HaveCount(2);

            var pierwszePole = pola[0];
            pierwszePole.Nazwa.Should().Be("Pierwsza");
            pierwszePole.NazwaTypu.Should().Be(null);
            pierwszePole.Generyczny.Should().BeFalse();

            var drugiePole = pola[1];
            drugiePole.Nazwa.Should().Be("Druga");
            drugiePole.NazwaTypu.Should().Be(null);
            drugiePole.Generyczny.Should().BeFalse();
        }
    }
}
