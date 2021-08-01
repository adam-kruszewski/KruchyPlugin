using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieDokumentacjiTests
    {
        Plik parsowane;
        Obiekt klasa;

        [SetUp]
        public void SetUpEachTest()
        {
            parsowane = Parser.Parsuj(
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaZDokumentacjaDoParsowania.cs"));

            klasa = parsowane.DefiniowaneObiekty.Single();
        }


        [Test]
        public void ParsujeDokumentacjeNadKlasa()
        {
            klasa.Dokumentacja.PelnyTeskt.Should().Be(@"/// <summary>
    /// klasa do testowania dokumentacji
    /// </summary>
");

            klasa.Dokumentacja.Linie.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "klasa do testowania dokumentacji",
                    "</summary>"
                });

            klasa.Dokumentacja.Poczatek.Sprawdz(3, 5);
            klasa.Dokumentacja.Koniec.Sprawdz(6, 1);
        }

    }
}
