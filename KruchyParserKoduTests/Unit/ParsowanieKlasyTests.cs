using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyParserKodu.ParserKodu;
using KruchyParserKoduTests.Utils;
using NUnit.Framework;

namespace KruchyParserKoduTests.Unit
{
    [TestFixture]
    public class ParsowanieKlasyTests
    {
        [Test]
        public void ParsujeKlase()
        {
            //arrange
            var zawartosc =
                new WczytywaczZawartosciPrzykladow()
                    .DajZawartoscPrzykladu("KlasaDoParsowania.cs");
            //act
            var plik = Parser.Parsuj(zawartosc);

            //assert
            plik.Should().NotBeNull();
            plik.Namespace.Should().Be("KruchyCompany.KruchyPlugin1Tests.ParserTests");
            plik.Usingi.Count.Should().Be(6);
            plik.Usingi[5].Poczatek.Sprawdz(6, 1);
            plik.Usingi[5].Koniec.Sprawdz(6, 35);

            var obiekt = plik.DefiniowaneObiekty.First();
            obiekt.Rodzaj.Should().Be(RodzajObiektu.Klasa);
            obiekt.Nazwa.Should().Be("KlasaDoParsowania");
            obiekt.PoczatkowaKlamerka.Wiersz.Should().Be(12);
            obiekt.PoczatkowaKlamerka.Kolumna.Should().Be(5);
            obiekt.Atrybuty.Count().Should().Be(1);
            obiekt.Atrybuty.First().Nazwa.Should().Be("Testowo");

            SprawdzPola(obiekt);

            SprawdzPropertiesy(obiekt);

            SprawdzKonstruktory(obiekt);

            SprawdzMetody(obiekt);
        }

        private static void SprawdzPola(Obiekt obiekt)
        {
            obiekt.Pola.Count().Should().Be(2);
            var pole = obiekt.Pola.First();
            pole.Nazwa.Should().Be("PoleReadOnly");
            pole.NazwaTypu.Should().Be("IParser");
            pole.Modyfikatory[0].Nazwa.Should().Be("private");
            pole.Modyfikatory[1].Nazwa.Should().Be("readonly");

            var poleStringReadonly = obiekt.Pola[1];
            poleStringReadonly.Nazwa.Should().Be("PoleStringReadOnly");
            poleStringReadonly.NazwaTypu.Should().Be("IList<string>");
            poleStringReadonly.Modyfikatory[0].Nazwa.Should().Be("public");
            poleStringReadonly.Modyfikatory[1].Nazwa.Should().Be("readonly");
        }

        private static void SprawdzPropertiesy(Obiekt obiekt)
        {
            obiekt.Propertiesy.Count().Should().Be(2);
            var wlasciwosc = obiekt.Propertiesy.First();
            wlasciwosc.Nazwa.Should().Be("Wlasciwosc");
            wlasciwosc.NazwaTypu.Should().Be("int");
            wlasciwosc.Modyfikatory[0].Nazwa.Should().Be("public");
            wlasciwosc.JestGet.Should().BeTrue();
            wlasciwosc.JestSet.Should().BeTrue();

            var wlasciwosc2 = obiekt.Propertiesy[1];
            wlasciwosc2.JestGet.Should().BeTrue();
            wlasciwosc2.JestSet.Should().BeFalse();
        }

        private void SprawdzKonstruktory(Obiekt obiekt)
        {
            obiekt.Konstruktory.Count().Should().Be(2);

            var bezparametrowyKonstruktor = obiekt.Konstruktory[1];
            bezparametrowyKonstruktor.Parametry.Count().Should().Be(0);
            SprawdzPozycje(bezparametrowyKonstruktor.Poczatek, 26, 9);
            SprawdzPozycje(bezparametrowyKonstruktor.Koniec, 29, 10);

            var konstrZ1Parametrem = obiekt.Konstruktory.First();
            konstrZ1Parametrem.Parametry.Count().Should().Be(1);
            var parametr = konstrZ1Parametrem.Parametry.First();
            parametr.NazwaParametru.Should().Be("a");
            parametr.NazwaTypu.Should().Be("int");
            SprawdzPozycje(konstrZ1Parametrem.Poczatek, 19, 9);
            SprawdzPozycje(konstrZ1Parametrem.Koniec, 24, 10);
            SprawdzPozycje(
                konstrZ1Parametrem.NawiasOtwierajacyParametry,
                19, 33);
            SprawdzPozycje(
                konstrZ1Parametrem.NawiasZamykajacyParametry,
                19, 39);
            SprawdzPozycje(
                konstrZ1Parametrem.PoczatkowaKlamerka,
                20, 9);
            SprawdzPozycje(
                konstrZ1Parametrem.KoncowaKlamerka,
                24, 9);
            konstrZ1Parametrem.Modyfikator.Should().Be("public");
        }

        private void SprawdzMetody(Obiekt obiekt)
        {
            obiekt.Metody.Count().Should().Be(2);

            var metodaStatyczna = obiekt.Metody[0];
            metodaStatyczna.Nazwa.Should().Be("MetodaStatyczna");
            metodaStatyczna.Parametry.Count().Should().Be(3);
            SprawdzModyfikatorMetodyStatycznej(metodaStatyczna);
            var p = metodaStatyczna.Parametry.First();
            p.NazwaTypu.Should().Be("string");
            p.NazwaParametru.Should().Be("b");
            metodaStatyczna.Parametry[1].NazwaTypu.Should().Be("int?");
            metodaStatyczna.Parametry[2].NazwaTypu.Should().Be("DateTime?");
            metodaStatyczna.Modyfikatory[0].Nazwa.Should().Be("private");
            metodaStatyczna.Modyfikatory[1].Nazwa.Should().Be("static");
            metodaStatyczna.TypZwracany.Should().Be("void");
            metodaStatyczna.NawiasOtwierajacyParametry.Wiersz.Should().Be(31);
            metodaStatyczna.NawiasOtwierajacyParametry.Kolumna.Should().Be(44);
            metodaStatyczna.NawiasZamykajacyParametry.Wiersz.Should().Be(31);
            metodaStatyczna.NawiasZamykajacyParametry.Kolumna.Should().Be(74);

            var metodaZwykla = obiekt.Metody[1];
            metodaZwykla.Modyfikatory.First().Nazwa.Should().Be("private");
            metodaZwykla.Nazwa.Should().Be("MetodaZwykla");
            metodaZwykla.TypZwracany.Should().Be("int");
            metodaZwykla.Parametry.Count().Should().Be(2);
            metodaZwykla.Parametry[0].NazwaTypu.Should().Be("System.DateTime");
            metodaZwykla.Parametry[1].NazwaTypu.Should().Be("System.DateTime?");
            metodaZwykla.Atrybuty.Count().Should().Be(1);
            metodaZwykla.Atrybuty.First().Nazwa.Should().Be("Testowo");
        }

        private void SprawdzModyfikatorMetodyStatycznej(Metoda metodaStatyczna)
        {
            metodaStatyczna.Modyfikatory.Count().Should().Be(2);
            var modyfikatorPublic = metodaStatyczna.Modyfikatory[0];
            modyfikatorPublic.Nazwa.Should().Be("private");
            SprawdzPozycje(modyfikatorPublic.Poczatek, 31, 9);
            SprawdzPozycje(modyfikatorPublic.Koniec, 31, 16);
            var modyfikatorStatic = metodaStatyczna.Modyfikatory[1];
            modyfikatorStatic.Nazwa.Should().Be("static");
        }

        private void SprawdzPozycje(PozycjaWPliku pozycja, int wiersz, int kolumna)
        {
            pozycja.Wiersz.Should().Be(wiersz);
            pozycja.Kolumna.Should().Be(kolumna);
        }
    }
}
