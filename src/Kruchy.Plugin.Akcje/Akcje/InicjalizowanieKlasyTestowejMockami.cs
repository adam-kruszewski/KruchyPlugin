using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class InicjalizowanieKlasyTestowejMockami
    {
        private readonly ISolutionWrapper solution;

        const string NazwaMetody = "Initialize";

        const string NazwaPolaInstancji = "_instance";

        public InicjalizowanieKlasyTestowejMockami(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Inicjuj()
        {
            var plikTestow = solution.AktualnyPlik;

            if (plikTestow == null)
                throw new ArgumentException("Brak aktualnego pliku");

            var plikTestowany = solution.SzukajPlikuKlasyTestowanej();

            if (plikTestowany == null)
                throw new AggregateException("Nie znaleziono pliku testowanego");

            var parserTestowanego = Parser.ParsujPlik(plikTestowany.FullPath);

            var konstruktor = parserTestowanego.DefiniowaneObiekty?.First()?.Konstruktory?.Single();

            if (konstruktor != null)
            {
                IEnumerable<Tuple<string, string>> polaZTypemZKonstruktora = DajPolaKonstruktora(konstruktor);

                IEnumerable<Tuple<string, string>> polaMockowDoDodania = DajPolaMockowDoDodania(polaZTypemZKonstruktora);

                DodajPolaMockow(polaMockowDoDodania);

                DodajPoleInstancji(plikTestowany.NameWithoutExtension);

                DodajLubZmienMetodeInitialize(polaZTypemZKonstruktora, plikTestowany.NameWithoutExtension);

                DodajPotrzebneUsingi(polaZTypemZKonstruktora);
            }

            solution.AktualnyDokument.DodajUsingaJesliTrzeba("Moq");
        }

        private void DodajPotrzebneUsingi(IEnumerable<Tuple<string, string>> polaZTypemZKonstruktora)
        {
            var wszystkiePliki = solution.Projekty.SelectMany(o => o.Files);

            foreach (var poleZTypem in polaZTypemZKonstruktora)
            {
                var plikZDefinicja = wszystkiePliki.SingleOrDefault(o => o.NameWithoutExtension == poleZTypem.Item1);

                if (plikZDefinicja != null)
                {
                    var sparsowane = Parser.ParsujPlik(plikZDefinicja.FullPath);

                    solution.AktualnyDokument.DodajUsingaJesliTrzeba(sparsowane.Namespace);
                }
            }
        }

        private IEnumerable<Tuple<string, string>> DajPolaKonstruktora(Konstruktor konstruktor)
        {
            return konstruktor.Parametry.Select(o => Tuple.Create(o.NazwaTypu, o.NazwaParametru));
        }

        private IEnumerable<Tuple<string, string>> DajPolaMockowDoDodania(
            IEnumerable<Tuple<string, string>> polaZTypemZKonstruktora)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var polaZdefiniowane = parsowane.DefiniowaneObiekty.First().Pola;

            return polaZTypemZKonstruktora
                .Where(o =>
                !polaZdefiniowane.Any(pz => pz.Nazwa == $"_{o.Item2}Mock") &&
                !polaZdefiniowane.Any(pz => pz.Nazwa == $"{o.Item2}Mock"));
        }

        private void DodajPolaMockow(IEnumerable<Tuple<string, string>> polaMockowDoDodania)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            int numerLiniiDoDodawaniaPol = SzukajLiniiDoWstawieniaPola(parsowane);

            var array = polaMockowDoDodania.ToArray();

            for (int i = array.Length - 1; i >= 0; i--)
            {
                var nowaLinia =
                    new PoleBuilder()
                    .ZNazwa(DajNazwaPolaMocka(array[i].Item2))
                    .ZNazwaTypu($"Mock<{array[i].Item1}>")
                    .Build(StaleDlaKodu.WcieciaDlaPolaKlasy);

                solution.AktualnyDokument.InsertInLine(nowaLinia, numerLiniiDoDodawaniaPol);
            }
        }

        private static int SzukajLiniiDoWstawieniaPola(Plik parsowane)
        {
            int numerLiniiDoDodawaniaPol;

            if (parsowane.DefiniowaneObiekty.First().Pola.Any())
            {
                var ostatnie = parsowane.DefiniowaneObiekty.First().Pola.Last();

                numerLiniiDoDodawaniaPol = ostatnie.Koniec.Wiersz + 1;
            }
            else
            {
                numerLiniiDoDodawaniaPol = parsowane.DefiniowaneObiekty.First().PoczatkowaKlamerka.Wiersz + 1;
            }

            return numerLiniiDoDodawaniaPol;
        }

        private void DodajPoleInstancji(string nazwaBezRozszerzenia)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            if (!parsowane.DefiniowaneObiekty.First().Pola.Any(o => o.Nazwa == NazwaPolaInstancji))
            {
                var nowaLinia = new PoleBuilder()
                    .ZNazwa(NazwaPolaInstancji)
                    .ZNazwaTypu(nazwaBezRozszerzenia)
                    .Build(StaleDlaKodu.WcieciaDlaPolaKlasy);

                solution.AktualnyDokument.InsertInLine(nowaLinia, SzukajLiniiDoWstawieniaPola(parsowane));
            }
        }

        private static string DajNazwaPolaMocka(string nazwaParametru)
        {
            return $"_{nazwaParametru}Mock";
        }

        private void DodajLubZmienMetodeInitialize(
            IEnumerable<Tuple<string, string>> polaZTypemZKontruktora,
            string nazwaKlasyTestowanej)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var aktualna = parsowane.DefiniowaneObiekty.First().Metody.SingleOrDefault(o => o.Nazwa == NazwaMetody);

            PozycjaWPliku poczatek;

            if (aktualna != null)
            {
                poczatek = aktualna.Poczatek;

                solution.AktualnyDokument.Remove(
                    aktualna.Poczatek.Wiersz, aktualna.Poczatek.Kolumna,
                    aktualna.Koniec.Wiersz, aktualna.Koniec.Kolumna);
            }else
            {
                poczatek = parsowane.DefiniowaneObiekty.First().Pola.LastOrDefault()?.Koniec;

                if (poczatek == null)
                {
                    throw new ArgumentException("Brak zdefiniowanych pól - to nie powinno się zdarzyć");
                }else
                {
                    poczatek = new PozycjaWPliku(poczatek.Wiersz + 2, poczatek.Kolumna);
                }
            }

            if (poczatek.Wiersz > parsowane.DefiniowaneObiekty.First().KoncowaKlamerka.Wiersz)
            {
                poczatek.Wiersz = parsowane.DefiniowaneObiekty.First().KoncowaKlamerka.Wiersz;
            }

            solution.AktualnyDokument.InsertInLine(GenerujMetode(polaZTypemZKontruktora, nazwaKlasyTestowanej), poczatek.Wiersz);
        }

        private string GenerujMetode(
            IEnumerable<Tuple<string, string>> polaZTypemZKontruktora,
            string nazwaKlasyTestowanej)
        {
            var resultBuilder = new StringBuilder();
            var atrybut = new AtrybutBuilder().ZNazwa("TestInitialize").Build(StaleDlaKodu.WciecieDlaMetody);

            resultBuilder.Append(atrybut);

            var metoda =
                new MetodaBuilder()
                    .ZNazwa(NazwaMetody)
                    .DodajModyfikator("private")
                    .ZTypemZwracanym("void");

            foreach (var pole in polaZTypemZKontruktora)
            {
                metoda.DodajLinie($"{DajNazwaPolaMocka(pole.Item2)} = new Mock<{pole.Item1}>();");
            }

            var tworzenieInstancjiBuilder = new StringBuilder();

            tworzenieInstancjiBuilder.AppendLine($"{NazwaPolaInstancji} = new {nazwaKlasyTestowanej}(");

            var indeks = 0;

            foreach (var parametr in polaZTypemZKontruktora)
            {
                tworzenieInstancjiBuilder.Append($"{StaleDlaKodu.WciecieDlaZawartosciMetody}{StaleDlaKodu.JednostkaWciecia}{DajNazwaPolaMocka(parametr.Item2)}.Object");

                if (indeks != polaZTypemZKontruktora.Count() -1)
                {
                    tworzenieInstancjiBuilder.AppendLine(",");
                }
                indeks++;
            }

            tworzenieInstancjiBuilder.Append(");");

            metoda.DodajLinie(tworzenieInstancjiBuilder.ToString());

            resultBuilder.Append(metoda.Build(StaleDlaKodu.WciecieDlaMetody));

            return resultBuilder.ToString().TrimEnd();
        }
    }
}