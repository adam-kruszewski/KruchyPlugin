using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKomentarzyTests
    {
        Plik parsowane;
        Obiekt klasa;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaZKomentarzemDoParsowania.cs"));

            klasa = parsowane.DefiniowaneObiekty.Single();
        }


        [Test]
        public void ParsujeKomentarzJednolinijkowyNadKlasa()
        {
            klasa.Komentarz.Linie.Single().Should().Be("// komentarz jednolinijkowy");
            klasa.Komentarz.Poczatek.Sprawdz(3, 5);
            klasa.Komentarz.Koniec.Sprawdz(3, 32);
        }
    }
}
