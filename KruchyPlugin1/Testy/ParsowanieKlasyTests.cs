﻿using System.IO;
using System.Linq;
using FluentAssertions;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using NUnit.Framework;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    [TestFixture]
    public class ParsowanieKlasyTests
    {
        [Test]
        public void ParsujeKlase()
        {
            //arrange
            var zawartosc = File.ReadAllText(DajSciezkeTestu());
            //act
            var plik = Parser.Parsuj(zawartosc);

            //assert
            plik.Should().NotBeNull();
            plik.Namespace.Should().Be("KruchyCompany.KruchyPlugin1.Testy");
            plik.Usingi.Count.Should().Be(6);

            var obiekt = plik.DefiniowaneObiekty.First();
            obiekt.Rodzaj.Should().Be(RodzajObiektu.Klasa);
            obiekt.Nazwa.Should().Be("KlasaDoParsowania");
            obiekt.PoczatkowaKlamerka.Wiersz.Should().Be(12);
            obiekt.PoczatkowaKlamerka.Kolumna.Should().Be(5);
            obiekt.Atrybuty.Count().Should().Be(1);
            obiekt.Atrybuty.First().Nazwa.Should().Be("Testowo");

            SprawdzPola(obiekt);

            SprawdzPropertiesa(obiekt);

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

        private static void SprawdzPropertiesa(Obiekt obiekt)
        {
            obiekt.Propertiesy.Count().Should().Be(1);
            var wlasciwosc = obiekt.Propertiesy.First();
            wlasciwosc.Nazwa.Should().Be("Wlasciwosc");
            wlasciwosc.NazwaTypu.Should().Be("int");
            wlasciwosc.Modyfikatory[0].Nazwa.Should().Be("public");
        }

        private void SprawdzKonstruktory(Obiekt obiekt)
        {
            obiekt.Konstruktory.Count().Should().Be(2);

            var bezparametrowyKonstruktor = obiekt.Konstruktory[1];
            bezparametrowyKonstruktor.Parametry.Count().Should().Be(0);
            SprawdzPozycje(bezparametrowyKonstruktor.Poczatek, 25, 9);
            SprawdzPozycje(bezparametrowyKonstruktor.Koniec, 28, 10);
            //SprawdzPozycje(
            //    bezparametrowyKonstruktor.PoczatekParametrow, 22, 33);
            //SprawdzPozycje(
            //    bezparametrowyKonstruktor.KoniecParametrow, 22, 34);

            var konstrZ1Parametrem = obiekt.Konstruktory.First();
            konstrZ1Parametrem.Parametry.Count().Should().Be(1);
            var parametr = konstrZ1Parametrem.Parametry.First();
            parametr.NazwaParametru.Should().Be("a");
            parametr.NazwaTypu.Should().Be("int");
            SprawdzPozycje(konstrZ1Parametrem.Poczatek, 18, 9);
            SprawdzPozycje(konstrZ1Parametrem.Koniec, 23, 10);
            //SprawdzPozycje(konstrZ1Parametrem.PoczatekParametrow, 17, 33);
            //SprawdzPozycje(konstrZ1Parametrem.KoniecParametrow, 17, 39);

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
            metodaStatyczna.NawiasOtwierajacyParametry.Wiersz.Should().Be(30);
            metodaStatyczna.NawiasOtwierajacyParametry.Kolumna.Should().Be(44);
            metodaStatyczna.NawiasZamykajacyParametry.Wiersz.Should().Be(30);
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
            SprawdzPozycje(modyfikatorPublic.Poczatek, 30, 9);
            SprawdzPozycje(modyfikatorPublic.Koniec, 30, 16);
            var modyfikatorStatic = metodaStatyczna.Modyfikatory[1];
            modyfikatorStatic.Nazwa.Should().Be("static");
        }

        private string DajSciezkeTestu()
        {
            return Path.Combine("..", "..", "Testy", "KlasaDoParsowania.cs");
        }

        private void SprawdzPozycje(PozycjaWPliku pozycja, int wiersz, int kolumna)
        {
            pozycja.Wiersz.Should().Be(wiersz);
            pozycja.Kolumna.Should().Be(kolumna);
        }
    }
}
