using System.IO;
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

            klasaGlowna.ParametryGeneryczne.Should().HaveCount(2);

            klasaGlowna.ParametryGeneryczne[0].Nazwa.Should().Be("TParam1");
            klasaGlowna.ParametryGeneryczne[0].Poczatek.Sprawdz(3, 41);
            klasaGlowna.ParametryGeneryczne[0].Koniec.Sprawdz(3, 48);

            klasaGlowna.ParametryGeneryczne[1].Nazwa.Should().Be("TParam2");
            klasaGlowna.ParametryGeneryczne[1].Poczatek.Sprawdz(3, 50);
            klasaGlowna.ParametryGeneryczne[1].Koniec.Sprawdz(3, 57);
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

            klasaGlowna.ParametryGeneryczne.Should().HaveCount(2);

            klasaGlowna.ParametryGeneryczne[0].Nazwa.Should().Be("TParam1");
            klasaGlowna.ParametryGeneryczne[0].Poczatek.Sprawdz(3, 50);
            klasaGlowna.ParametryGeneryczne[0].Koniec.Sprawdz(3, 57);

            klasaGlowna.ParametryGeneryczne[1].Nazwa.Should().Be("TParam2");
            klasaGlowna.ParametryGeneryczne[1].Poczatek.Sprawdz(3, 59);
            klasaGlowna.ParametryGeneryczne[1].Koniec.Sprawdz(3, 66);
        }
    }
}
