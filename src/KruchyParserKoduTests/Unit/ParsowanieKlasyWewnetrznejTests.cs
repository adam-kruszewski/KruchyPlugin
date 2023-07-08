using System.IO;
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
            var klasaGlowna = sparsowane.DefinedItems.Single();
            klasaGlowna.KindOfItem.Should().Be(RodzajObiektu.Klasa);
            klasaGlowna.Name.Should().Be("ZKlasaWewnetrzna");
            klasaGlowna.Properties.Single().Name.Should().Be("WlasciwoscWGlownym");

            var klasaWewnetrzna = klasaGlowna.InternalDefinedItems.Single();
            klasaWewnetrzna.KindOfItem.Should().Be(RodzajObiektu.Klasa);
            klasaWewnetrzna.Name.Should().Be("KlasaWewnetrzna");
            klasaWewnetrzna.Properties.Single().Name.Should().Be("WlasciwoscWWewnetrznym");
            klasaWewnetrzna.Owner.Should().Be(klasaGlowna);

            var metodaWewnetrzna = klasaWewnetrzna.Methods.Single();
            metodaWewnetrzna.Name.Should().Be("Metoda1");
            metodaWewnetrzna.Owner.Should().Be(klasaWewnetrzna);

            var konstruktorWewnetrznej = klasaWewnetrzna.Constructors.Single();
            konstruktorWewnetrznej.Owner.Should().Be(klasaWewnetrzna);
        }
    }
}
