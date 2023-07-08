using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieModyfikatorwKlasyTests
    {
        Plik sparsowane;

        [SetUp]
        public void SetUpEachTest()
        {
            var zawartosc =
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowaniaModyfikatorow.cs");

            sparsowane = Parser.Parsuj(zawartosc);
        }

        [Test]
        public void ParsujeModyfikatorKlasy()
        {
            var klasa = sparsowane.DefiniowaneObiekty.Single();

            klasa.Modifiers.Should().HaveCount(2);
            var modyfikatorPublic = klasa.Modifiers[0];
            modyfikatorPublic.Name.Should().Be("public");
            modyfikatorPublic.StartPosition.Sprawdz(3, 5);
            modyfikatorPublic.EndPosition.Sprawdz(3, 11);

            var modyfikatorStatic = klasa.Modifiers[1];
            modyfikatorStatic.Name.Should().Be("static");
            modyfikatorStatic.StartPosition.Sprawdz(3, 12);
            modyfikatorStatic.EndPosition.Sprawdz(3, 18);
        }

        [Test]
        public void ParsujeModyfikatorKlasyWewntrzenej()
        {
            var klasa = sparsowane.DefiniowaneObiekty.Single().InternalDefinedItems.Single();

            klasa.Modifiers.Should().HaveCount(1);
            var modyfikatorPublic = klasa.Modifiers[0];
            modyfikatorPublic.Name.Should().Be("private");
            modyfikatorPublic.StartPosition.Sprawdz(5, 9);
            modyfikatorPublic.EndPosition.Sprawdz(5, 16);
        }
    }
}