using KrucheBuilderyKodu.Builders;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    [TestFixture]
    public class MetodaBuilderTests
    {
        [Test]
        public void GenerujeMetode()
        {
            var metodaBuilder =
                new MetodaBuilder()
                    .ZNazwa("metoda")
                    .ZTypemZwracanym("int")
                    .DodajModyfikator("public")
                    .DodajModyfikator("override")
                    .DodajParametr("int", "param1")
                    .DodajParametr("string", "param2")
                    .DodajLinie("if (a == b)")
                    .DodajLinie("")
                    .DodajLinie("  stop();");

            var wynik = metodaBuilder.Build(StaleDlaKodu.WciecieDlaMetody);
        }
    }
}
