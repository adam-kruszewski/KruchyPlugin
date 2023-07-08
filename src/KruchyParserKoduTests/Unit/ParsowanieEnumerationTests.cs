using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieEnumerationTests
    {
        FileWithCode parsowane;

        Enumeration enumeration;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parse(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("EnumerationDoParsowania.cs"));

            enumeration = parsowane.DefinedEnumerations.Single();
        }

        [Test]
        public void ParsujeNazweIPozycjeEnumeracji()
        {
            enumeration.Name.Should().Be("Enum1");
            enumeration.StartPosition.Sprawdz(6, 5);
            enumeration.EndPosition.Sprawdz(12, 6);
        }

        [Test]
        public void ParsujeAtrybutyEnumeracji()
        {
            var atrybut = enumeration.Attributes.Single();

            atrybut.Name.Should().Be("Serializable");
            atrybut.StartPosition.Sprawdz(6, 6);
            atrybut.EndPosition.Sprawdz(6, 18);
            atrybut.Parameters.Should().BeNullOrEmpty();
        }

        [Test]
        public void ParsujePola()
        {
            var pola = enumeration.Fields;

            pola.Should().HaveCount(2);

            var pierwszePole = pola[0];
            pierwszePole.Name.Should().Be("Pierwsza");
            pierwszePole.TypeName.Should().Be(null);
            pierwszePole.IsGeneric.Should().BeFalse();

            var drugiePole = pola[1];
            drugiePole.Name.Should().Be("Druga");
            drugiePole.TypeName.Should().Be(null);
            drugiePole.IsGeneric.Should().BeFalse();
        }
    }
}
