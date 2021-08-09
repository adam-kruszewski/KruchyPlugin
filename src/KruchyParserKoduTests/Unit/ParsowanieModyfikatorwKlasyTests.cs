using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
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

            klasa.Modyfikatory.Should().HaveCount(2);
            var modyfikatorPublic = klasa.Modyfikatory[0];
            modyfikatorPublic.Nazwa.Should().Be("public");
            modyfikatorPublic.Poczatek.Sprawdz(3, 5);
            modyfikatorPublic.Koniec.Sprawdz(3, 11);

            var modyfikatorStatic = klasa.Modyfikatory[1];
            modyfikatorStatic.Nazwa.Should().Be("static");
            modyfikatorStatic.Poczatek.Sprawdz(3, 12);
            modyfikatorStatic.Koniec.Sprawdz(3, 18);
        }

        [Test]
        public void ParsujeModyfikatorKlasyWewntrzenej()
        {
            var klasa = sparsowane.DefiniowaneObiekty.Single().ObiektyWewnetrzne.Single();

            klasa.Modyfikatory.Should().HaveCount(1);
            var modyfikatorPublic = klasa.Modyfikatory[0];
            modyfikatorPublic.Nazwa.Should().Be("private");
            modyfikatorPublic.Poczatek.Sprawdz(5, 9);
            modyfikatorPublic.Koniec.Sprawdz(5, 16);
        }
    }
}