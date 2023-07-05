﻿using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKlasyWewnetrznejTests
    {
        [Test]
        public void ParsujeKlaseWewnetrzna()
        {
            //arrange
            //arrange
            string zawartosc;
            using (
                var stream =
            GetType().Assembly.GetManifestResourceStream("KruchyParserKoduTests.Samples.ZKlasaWewnetrzna.cs"))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                zawartosc = reader.ReadToEnd();
            }

            //act
            var sparsowane = Parser.Parsuj(zawartosc);

            //assert
            var klasaGlowna = sparsowane.DefiniowaneObiekty.Single();
            klasaGlowna.Rodzaj.Should().Be(RodzajObiektu.Klasa);
            klasaGlowna.Name.Should().Be("ZKlasaWewnetrzna");
            klasaGlowna.Propertiesy.Single().Nazwa.Should().Be("WlasciwoscWGlownym");

            var klasaWewnetrzna = klasaGlowna.ObiektyWewnetrzne.Single();
            klasaWewnetrzna.Rodzaj.Should().Be(RodzajObiektu.Klasa);
            klasaWewnetrzna.Name.Should().Be("KlasaWewnetrzna");
            klasaWewnetrzna.Propertiesy.Single().Nazwa.Should().Be("WlasciwoscWWewnetrznym");
            klasaWewnetrzna.Owner.Should().Be(klasaGlowna);

            var metodaWewnetrzna = klasaWewnetrzna.Metody.Single();
            metodaWewnetrzna.Name.Should().Be("Metoda1");
            metodaWewnetrzna.Owner.Should().Be(klasaWewnetrzna);

            var konstruktorWewnetrznej = klasaWewnetrzna.Konstruktory.Single();
            konstruktorWewnetrznej.Owner.Should().Be(klasaWewnetrzna);
        }
    }
}
