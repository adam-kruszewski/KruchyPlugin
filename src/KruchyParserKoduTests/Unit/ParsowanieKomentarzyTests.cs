﻿using FluentAssertions;
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
        Plik parsowane;
        DefinedItem klasa;

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
            klasa.Comment.Lines.Single().Should().Be("// komentarz jednolinijkowy");
            klasa.Comment.Poczatek.Sprawdz(3, 5);
            klasa.Comment.Koniec.Sprawdz(3, 32);
        }
    }
}
