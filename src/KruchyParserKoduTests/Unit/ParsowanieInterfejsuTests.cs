using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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
            var sparsowane = Parser.Parse(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("InterfejsDoParsowania.cs"));

            //assert
            var interfejs = sparsowane.DefinedItems.Single();

            interfejs.KindOfItem = KindOfItem.Interface;
            interfejs.Name = "InterfejsDoParsowania";
            interfejs.Owner.Should().BeNull();

            interfejs.Constructors.Should().BeEmpty();
            interfejs.Fields.Should().BeEmpty();

            interfejs.Properties.Should().HaveCount(2);
            interfejs.Methods.Should().HaveCount(1);

            interfejs.Attributes.Should().BeEmpty();
            interfejs.SuperClassAndInterfaces.Should().BeEmpty();
            interfejs.InternalDefinedItems.Should().BeEmpty();

            interfejs.StartingBrace.Sprawdz(7, 5);
            interfejs.ClosingBrace.Sprawdz(12, 5);

            interfejs.StartPosition.Sprawdz(6, 5);
            interfejs.EndPosition.Sprawdz(12, 6);
        }

        [Test]
        public void ParsujeInterfejsZDziedziczeniemIAtrybutami()
        {
            //act
            var sparsowane = Parser.Parse(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("InterfejsZDziedziczeniemIAtrybutami.cs"));

            //assert
            var interfejs = sparsowane.DefinedItems.Single();

            interfejs.SuperClassAndInterfaces.Should().HaveCount(2);

            interfejs.SuperClassAndInterfaces[0].Name.Should().Be("InterfejsDoParsowania");
            interfejs.SuperClassAndInterfaces[0].ParameterTypeNames.Should().BeEmpty();
            interfejs.SuperClassAndInterfaces[0].StartPosition.Sprawdz(5, 53);
            interfejs.SuperClassAndInterfaces[0].EndPosition.Sprawdz(5, 74);

            interfejs.SuperClassAndInterfaces[1].Name.Should().Be("Interfejs2");
            interfejs.SuperClassAndInterfaces[1].ParameterTypeNames.Should().HaveCount(2);
            var parametryTypu = interfejs.SuperClassAndInterfaces[1].ParameterTypeNames;
            parametryTypu[0].Should().Be("Klasa1");
            parametryTypu[1].Should().Be("int?");
            interfejs.SuperClassAndInterfaces[1].StartPosition.Sprawdz(5, 76);
            interfejs.SuperClassAndInterfaces[1].EndPosition.Sprawdz(5, 100);
        }
    }
}
