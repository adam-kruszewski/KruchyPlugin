using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSharpCode.NRefactory.CSharp;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;
using KrucheBuilderyKodu.Builders;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieKontruktora
    {
        private readonly SolutionWrapper solution;

        public UzupelnianieKontruktora(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Uzupelnij()
        {
            if (solution.AktualnyDokument == null)
                return;

            var obiekt = SzukajKlasy();
            if (obiekt == null)
            {
                MessageBox.Show(
                    "Nie udało się ustalić klasy do uzupełniania konstruktora");
                return;
            }

            if (obiekt.Konstruktory.Count > 1)
            {
                MessageBox.Show("Klasa ma więcej niż jeden konstruktor");
                return;
            }
            var konstruktor = obiekt.Konstruktory.FirstOrDefault();

            var polaDoDodania =
                WyliczPolaDoDodaniaDoKonstruktora(obiekt.Pola, konstruktor);

            if (polaDoDodania.Count > 0)
            {
                var polaReadOnly = obiekt.Pola.Where(
                    o => o.Modyfikatory.Select(m => m.Nazwa).Contains("private")
                        && o.Modyfikatory.Select(m => m.Nazwa).Contains("readonly"));
                var nowyKonstruktor =
                    GenerujKonstruktor(polaReadOnly, obiekt.Nazwa);

                if (konstruktor != null)
                {
                    solution.AktualnyDokument.Usun(
                        konstruktor.Poczatek.Wiersz,
                        1,
                        konstruktor.Koniec.Wiersz,
                        konstruktor.Koniec.Kolumna);
                    solution.AktualnyDokument.WstawWLinii(
                        nowyKonstruktor, konstruktor.Poczatek.Wiersz);
                }else
                {
                    //tu zawsze będzie jakieś pole lub właściwośc
                    //, bo są pola które czekają na dodanie do konstruktora
                    var maksymalnyNumerLiniiPol =
                        obiekt.Pola.Select(o => o.Poczatek.Wiersz)
                            .Union(obiekt.Propertiesy.Select(o => o.Poczatek.Wiersz))
                                .Max();
                    var dodatek = "\n";
                    solution.AktualnyDokument.WstawWLinii(
                        "\n" + nowyKonstruktor + dodatek, maksymalnyNumerLiniiPol + 1);
                }
            }
        }

        private Obiekt SzukajKlasy()
        {
            var kod = solution.AktualnyDokument.DajZawartosc();

            var parsowanyPlik = Parser.Parsuj(kod);
            var klasy =
                parsowanyPlik
                    .DefiniowaneObiekty
                        .Where(o => o.Rodzaj == RodzajObiektu.Klasa);

            if (klasy.Count() == 1)
                return klasy.First();

            return
                parsowanyPlik
                    .SzukajKlasyWLinii(
                        solution.AktualnyDokument.DajNumerLiniiKursora());
        }

        private string GenerujKonstruktor(IEnumerable<Pole> pola, string nazwaKlasy)
        {
            var builder = new MetodaBuilder();
            builder.JedenParametrWLinii(true);
            builder.ZNazwa(nazwaKlasy);
            builder.ZTypemZwracanym("");
            builder.DodajModyfikator("public");
            
            foreach (var pole in pola)
            {
                builder.DodajParametr(pole.NazwaTypu, pole.Nazwa);
                builder.DodajLinie("this." + pole.Nazwa + " = " + pole.Nazwa + ";");
            }
            return builder.Build(StaleDlaKodu.WciecieDlaMetody).TrimEnd();
        }

        private List<Pole> WyliczPolaDoDodaniaDoKonstruktora(
            IList<Pole> pola,
            Konstruktor konstruktor)
        {
            var wynik = new List<Pole>();
            var polaReadOnly =
                pola.Where(o => o.Modyfikatory.Select(m => m.Nazwa).Contains("private")
                    && o.Modyfikatory.Select(m => m.Nazwa).Contains("readonly"));
            foreach (var pole in polaReadOnly)
            {
                if (!KonstruktorMaWParametrzePole(konstruktor, pole))
                    wynik.Add(pole);
            }
            return wynik;
        }

        private bool KonstruktorMaWParametrzePole(
            Konstruktor konstruktor,
            Pole pole)
        {
            if (konstruktor == null)
                return false;

            return konstruktor.Parametry
                .Any(o => o.NazwaParametru == pole.Nazwa
                    && o.NazwaTypu == pole.NazwaTypu);
        }
    }
}