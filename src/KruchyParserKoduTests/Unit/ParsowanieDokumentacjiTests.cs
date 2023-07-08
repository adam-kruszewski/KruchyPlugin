using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;
using System.Linq;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieDokumentacjiTests
    {
        Plik parsowane;
        DefinedItem klasa;

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
            klasa.Documentation.FullText.Should().Be(@"/// <summary>
    /// klasa do testowania dokumentacji
    /// </summary>
");

            klasa.Documentation.Lines.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "klasa do testowania dokumentacji",
                    "</summary>"
                });

            klasa.Documentation.Poczatek.Sprawdz(3, 5);
            klasa.Documentation.Koniec.Sprawdz(6, 1);
        }

        [Test]
        public void ParsujeDokumentacjeNadMetoda()
        {
            var metoda = klasa.Methods.Single(o => o.Name == "Metoda1");

            //assert
            metoda.Documentation.Lines.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "metoda1",
                    "</summary>"
                });

            metoda.Documentation.Poczatek.Sprawdz(8, 9);
            metoda.Documentation.Koniec.Sprawdz(11, 1);
        }

        [Test]
        public void ParsujeDokumentacjeNadKontruktorem()
        {
            var konstruktor = klasa.Constructors.Single();

            //assert
            konstruktor.Documentation.Lines.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "konstruktor",
                    "</summary>"
                });

            konstruktor.Documentation.Poczatek.Sprawdz(16, 9);
            konstruktor.Documentation.Koniec.Sprawdz(19, 1);
        }

        [Test]
        public void ParsujeDokumentacjaNadPolem()
        {
            var pole = klasa.Fields.Single(o => o.Nazwa == "poleString");

            //assert
            pole.Documentation.Lines.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "pole string",
                    "</summary>"
                });

            pole.Documentation.Poczatek.Sprawdz(24, 9);
            pole.Documentation.Koniec.Sprawdz(27, 1);
        }

        [Test]
        public void ParsujeDokumentacjaNadProperty()
        {
            var property = klasa.Properties.Single(o => o.Nazwa == "MyProperty");
            //act

            //assert
            property.Documentation.Lines.Should().BeEquivalentTo(
                new[]
                {
                    "<summary>",
                    "my property",
                    "</summary>"
                });

            property.Documentation.Poczatek.Sprawdz(29, 9);
            property.Documentation.Koniec.Sprawdz(32, 1);
        }
    }
}
