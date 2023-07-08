using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKomentarzyTests
    {
        FileWithCode parsowane;
        DefinedItem klasa;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parse(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaZKomentarzemDoParsowania.cs"));

            klasa = parsowane.DefinedItems.Single();
        }


        [Test]
        public void ParsujeKomentarzJednolinijkowyNadKlasa()
        {
            klasa.Comment.Lines.Single().Should().Be("// komentarz jednolinijkowy");
            klasa.Comment.StartPosition.Sprawdz(3, 5);
            klasa.Comment.EndPosition.Sprawdz(3, 32);
        }
    }
}
