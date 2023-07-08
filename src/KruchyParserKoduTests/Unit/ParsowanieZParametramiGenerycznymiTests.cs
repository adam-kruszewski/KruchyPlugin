﻿using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieZParametramiGenerycznymiTests
    {
        [Test]
        public void ParsujeKlase()
        {
            //arrange
            string zawartosc;
            using (
                var stream =
            GetType().Assembly.GetManifestResourceStream("KruchyParserKoduTests.Samples.KlasaZParametramiGenerycznymi.cs"))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                zawartosc = reader.ReadToEnd();
            }

            //act
            var sparsowane = Parser.Parsuj(zawartosc);

            //assert
            var klasaGlowna = sparsowane.DefiniowaneObiekty.Single();

            klasaGlowna.GenericParameters.Should().HaveCount(2);

            klasaGlowna.GenericParameters[0].Nazwa.Should().Be("TParam1");
            klasaGlowna.GenericParameters[0].Poczatek.Sprawdz(3, 41);
            klasaGlowna.GenericParameters[0].Koniec.Sprawdz(3, 48);

            klasaGlowna.GenericParameters[1].Nazwa.Should().Be("TParam2");
            klasaGlowna.GenericParameters[1].Poczatek.Sprawdz(3, 50);
            klasaGlowna.GenericParameters[1].Koniec.Sprawdz(3, 57);
        }

        [Test]
        public void ParsujeInterfejs()
        {
            //arrange
            string zawartosc;
            using (
                var stream =
            GetType().Assembly.GetManifestResourceStream("KruchyParserKoduTests.Samples.IInterfejsZParametramiGenerycznymi.cs"))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                zawartosc = reader.ReadToEnd();
            }

            //act
            var sparsowane = Parser.Parsuj(zawartosc);

            //assert
            var klasaGlowna = sparsowane.DefiniowaneObiekty.Single();

            klasaGlowna.GenericParameters.Should().HaveCount(2);

            klasaGlowna.GenericParameters[0].Nazwa.Should().Be("TParam1");
            klasaGlowna.GenericParameters[0].Poczatek.Sprawdz(3, 50);
            klasaGlowna.GenericParameters[0].Koniec.Sprawdz(3, 57);

            klasaGlowna.GenericParameters[1].Nazwa.Should().Be("TParam2");
            klasaGlowna.GenericParameters[1].Poczatek.Sprawdz(3, 59);
            klasaGlowna.GenericParameters[1].Koniec.Sprawdz(3, 66);
        }
    }
}
