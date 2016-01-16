using KrucheBuilderyKodu.Builders;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    [TestFixture]
    public class PlikClassBuilderTests
    {
        [Test]
        public void GenerujeNamespace()
        {
            var klasaBuilder = new ClassBuilder()
                .ZModyfikatorem("public")
                .DodajAtrybut(
                    new AtrybutBuilder()
                        .ZNazwa("Atrybut").DodajWartoscParametru("p1")
                        .DodajWartoscParametru("p2"))
                .DodajAtrybut(new AtrybutBuilder().ZNazwa("DrugiAtrybut"))
                .ZNazwa("Klasa2")
                .ZNadklasa("Klasa3");

            var wynik =
                new PlikClassBuilder()
                    .DodajUsing("piatka.using1")
                    .DodajUsing("piatka.klasa1")
                    .ZObiektem(klasaBuilder)
                    .WNamespace("Kruchy.Plugin")
                        .Build();
        }
    }
}
