﻿using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieWlasciwosciZapisanychJakoLambdaTests
    {
        [Test]
        public void Test1()
        {
            //arrange
            string zawartosc;
            using (
                var stream =
            GetType().Assembly.GetManifestResourceStream("KruchyParserKoduTests.Samples.WlasciwosciZapisaneJakoLambda.cs"))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                zawartosc = reader.ReadToEnd();
            }

            //act
            var sparsowane = Parser.Parsuj(zawartosc);

            //assert
            var wlasciwosc = sparsowane.DefiniowaneObiekty.First().Propertiesy.Single();

            wlasciwosc.Nazwa.Should().Be("MyProperty");
            wlasciwosc.NazwaTypu.Should().Be("int");
        }
    }
}
