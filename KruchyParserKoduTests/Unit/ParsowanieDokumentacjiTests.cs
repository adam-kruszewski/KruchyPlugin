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

        [Test]
        public void ParsujeDokumentacjeNadMetoda()
        {
            var metoda = klasa.Metody.Single(o => o.Nazwa == "Metoda1");

            //assert
            metoda.Dokumentacja.Linie.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "metoda1",
                    "</summary>"
                });

            metoda.Dokumentacja.Poczatek.Sprawdz(8, 9);
            metoda.Dokumentacja.Koniec.Sprawdz(11, 1);
        }

        [Test]
        public void ParsujeDokumentacjeNadKontruktorem()
        {
            var konstruktor = klasa.Konstruktory.Single();

            //assert
            konstruktor.Dokumentacja.Linie.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "konstruktor",
                    "</summary>"
                });

            konstruktor.Dokumentacja.Poczatek.Sprawdz(16, 9);
            konstruktor.Dokumentacja.Koniec.Sprawdz(19, 1);
        }

        [Test]
        public void ParsujeDokumentacjaNadPolem()
        {
            var pole = klasa.Pola.Single(o => o.Nazwa == "poleString");

            //assert
            pole.Dokumentacja.Linie.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "pole string",
                    "</summary>"
                });

            pole.Dokumentacja.Poczatek.Sprawdz(24, 9);
            pole.Dokumentacja.Koniec.Sprawdz(27, 1);
        }

        [Test]
        public void ParsujeDokumentacjaNadProperty()
        {
            var property = klasa.Propertiesy.Single(o => o.Nazwa == "MyProperty");
            //act

            //assert
            property.Dokumentacja.Linie.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "my property",
                    "</summary>"
                });

            property.Dokumentacja.Poczatek.Sprawdz(29, 9);
            property.Dokumentacja.Koniec.Sprawdz(32, 1);
        }
    }
}
