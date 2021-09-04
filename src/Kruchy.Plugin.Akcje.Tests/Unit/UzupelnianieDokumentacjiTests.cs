using Kruchy.Plugin.Akcje.Akcje;
using FluentAssertions;
using NUnit.Framework;
using Kruchy.Plugin.Akcje.Tests.Utils;
using Kruchy.Plugin.Akcje.Tests.WrappersMocks;
using Moq;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using System;

namespace Kruchy.Plugin.Akcje.Tests.Unit
{
    [TestFixture]
    public class UzupelnianieDokumentacjiTests
    {
        WczytywaczZawartosciPrzykladow wczytywacz;

        [SetUp]
        public void SetUpEachTest()
        {
            wczytywacz = new WczytywaczZawartosciPrzykladow();
        }

        [Test]
        public void DodajeDokumentacjeJesliNieByloZadnej()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacji.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 1);

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasyDoDokumentacji.cs"));
        }

        [Test]
        public void DodajeOpisParametrowGenerycznych()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaGenertycznaZMetodaGeneryczna.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 1);

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasaGenertycznaZMetodaGeneryczna.cs"));
        }

        [Test]
        public void RozneCiekawePrzypadki()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacjaRozne.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 2);

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasaDoDokumentacjaRozne.cs"));
        }

        [Test]
        public void OdmianaCzasownikowZUstawien()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacjiCzasownikow.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 2,
               dok =>
               {
                   dok.Czasowniki.Add(
                       new Czasownik
                       {
                           Wartosc = "To",
                           WyjsciowaWartosc = "To"
                       });

                   dok.Czasowniki.Add(
                       new Czasownik
                       {
                           Wartosc = "Is",
                           WyjsciowaWartosc = "Is"
                       });

                   dok.Czasowniki.Add(
                   new Czasownik
                   {
                       Wartosc = "To",
                       RegexNazwyKlasy = "Klasa[0-9]",
                       WyjsciowaWartosc = "OtherTo"
                   });
               });

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("WynikKlasaDoDokumentacjiCzasownikow.cs"));
        }

        [Test]
        public void WlasciwosciIPolaZKonfiguracjiDokumentacji()
        {
            //arrange
            var solution = new SolutionWrapper(wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacjiWlasciwosciIPol.cs"));

            PrzygotujKonfiguracjeWgSolutionISzablonu(solution, 2,
                dok =>
                {
                    dok.WlasciwosciPola.Add(
                        new WlasciwoscPole
                        {
                            Wartosc = "Wlasciwosc1",
                            WyjsciowaWartosc = "Text1"
                        });

                    dok.WlasciwosciPola.Add(
                        new WlasciwoscPole
                        {
                            Wartosc = "Wlasciwosc2",
                            RegexNazwyKlasy = "Klasa2",
                            WyjsciowaWartosc = "Text2"
                        });

                    dok.WlasciwosciPola.Add(
                        new WlasciwoscPole
                        {
                            Wartosc = "pole1",
                            WyjsciowaWartosc = "TextPole1"
                        });

                    dok.WlasciwosciPola.Add(
                        new WlasciwoscPole
                        {
                            Wartosc = "pole2",
                            RegexNazwyKlasy = "Klasa2",
                            WyjsciowaWartosc = "TextPole2"
                        });

                });

            //act
            new UzupelnianieDokumentacji(solution).Uzupelnij();

            //assert
            var a1 = solution.AktualnyDokument.DajZawartosc();
            var a2 = wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacjiWlasciwosciIPolWynik.cs");

            var b1 = a1.Substring(541);
            var b2 = a2.Substring(541);

            System.IO.File.WriteAllText(@"e:\adam\tmp\a1.txt", a1);
            System.IO.File.WriteAllText(@"e:\adam\tmp\a2.txt", a2);

            solution.AktualnyDokument.DajZawartosc().Should().Be(
                wczytywacz.DajZawartoscPrzykladu("KlasaDoDokumentacjiWlasciwosciIPolWynik.cs"));
        }

        private void PrzygotujKonfiguracjeWgSolutionISzablonu(
            SolutionWrapper solution,
            int jezyk,
            Action<Dokumentacja> konfiguracjaDokumentacji = null)
        {

            var mockKonfiguracja = new Mock<Konfiguracja>();
            var dokumentacja = new Dokumentacja
            {
                Jezyk = jezyk
            };

            if (konfiguracjaDokumentacji != null)
                konfiguracjaDokumentacji(dokumentacja);

            mockKonfiguracja.Setup(o => o.Dokumentacja())
                .Returns(dokumentacja);

            mockKonfiguracja.SetupGet(o => o.Solution).Returns(solution);

            Konfiguracja.SetInstance(mockKonfiguracja.Object);
        }
    }
}