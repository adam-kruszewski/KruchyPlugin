using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.Utils;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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

            var konstruktor = parserTestowanego.DefinedItems?.First()?.Constructors?.Single();

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

        private IEnumerable<Tuple<string, string>> DajPolaKonstruktora(Constructor konstruktor)
        {
            return konstruktor.Parametry.Select(o => Tuple.Create(o.TypeName, o.ParameterName));
        }

        private IEnumerable<Tuple<string, string>> DajPolaMockowDoDodania(
            IEnumerable<Tuple<string, string>> polaZTypemZKonstruktora)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            var polaZdefiniowane = parsowane.DefinedItems.First().Fields;

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

        private static int SzukajLiniiDoWstawieniaPola(FileWithCode parsowane)
        {
            int numerLiniiDoDodawaniaPol;

            if (parsowane.DefinedItems.First().Fields.Any())
            {
                var ostatnie = parsowane.DefinedItems.First().Fields.Last();

                numerLiniiDoDodawaniaPol = ostatnie.EndPosition.Row + 1;
            }
            else
            {
                numerLiniiDoDodawaniaPol = parsowane.DefinedItems.First().StartingBrace.Row + 1;
            }

            return numerLiniiDoDodawaniaPol;
        }

        private void DodajPoleInstancji(string nazwaBezRozszerzenia)
        {
            var parsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            if (!parsowane.DefinedItems.First().Fields.Any(o => o.Nazwa == NazwaPolaInstancji))
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

            var aktualna = parsowane.DefinedItems.First().Methods.SingleOrDefault(o => o.Name == NazwaMetody);

            PlaceInFile poczatek;

            if (aktualna != null)
            {
                poczatek = aktualna.StartPosition;

                solution.AktualnyDokument.Remove(
                    aktualna.StartPosition.Row, aktualna.StartPosition.Column,
                    aktualna.EndPosition.Row, aktualna.EndPosition.Column);
            }else
            {
                poczatek = parsowane.DefinedItems.First().Fields.LastOrDefault()?.EndPosition;

                if (poczatek == null)
                {
                    throw new ArgumentException("Brak zdefiniowanych pól - to nie powinno się zdarzyć");
                }else
                {
                    poczatek = new PlaceInFile(poczatek.Row + 2, poczatek.Column);
                }
            }

            if (poczatek.Row > parsowane.DefinedItems.First().ClosingBrace.Row)
            {
                poczatek.Row = parsowane.DefinedItems.First().ClosingBrace.Row;
            }

            solution.AktualnyDokument.InsertInLine(GenerujMetode(polaZTypemZKontruktora, nazwaKlasyTestowanej), poczatek.Row);
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