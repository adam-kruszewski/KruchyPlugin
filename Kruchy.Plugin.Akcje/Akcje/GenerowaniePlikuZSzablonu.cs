using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class GenerowaniePlikuZSzablonu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public GenerowaniePlikuZSzablonu(
            ISolutionExplorerWrapper solutionExplorer,
            ISolutionWrapper solution)
        {
            this.solutionExplorer = solutionExplorer;
            this.solution = solution;
        }

        public void Generuj(string nazwaSzablonu)
        {
            var konf = Konfiguracja.GetInstance(solution);

            var szablon =
                konf
                    .SchematyGenerowania()
                        .Single(o => o.TytulSchematu == nazwaSzablonu);

            foreach (var schematKlasy in szablon.SchematyKlas)
            {
                GenerujWgSchmatu(schematKlasy);
            }
        }

        private void GenerujWgSchmatu(SchematKlasy schematKlasy)
        {
            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            var sciezkaDoPliku =
                Path.Combine(
                    solution.AktualnyProjekt.SciezkaDoKatalogu,
                    DajNazwePliku(schematKlasy, sparsowane));

            var tresc = schematKlasy.Tresc;

            tresc = ZamienZmienneNaWartosci(tresc, schematKlasy, sparsowane);

            File.WriteAllText(sciezkaDoPliku, tresc, Encoding.UTF8);
            solution.AktualnyProjekt.DodajPlik(sciezkaDoPliku);
        }

        private string DajNazwePliku(SchematKlasy schematKlasy, Plik sparsowane)
        {
            return ZamienZmienneNaWartosci(schematKlasy.NazwaPliku, schematKlasy, sparsowane);
        }

        private string ZamienZmienneNaWartosci(
            string tekst,
            SchematKlasy schematKlasy,
            Plik sparsowane)
        {
            var zmienne = PrzygotujWartosciZmiennych(schematKlasy, sparsowane);

            foreach (var zmienna in zmienne)
                tekst = tekst.Replace("%" + zmienna.Key + "%", zmienna.Value);

            return tekst;
        }

        private Dictionary<string, string> PrzygotujWartosciZmiennych(
            SchematKlasy schematKlasy,
            Plik sparsowane)
        {
            var wynik = new Dictionary<string, string>();

            wynik["NAZWA_KLASY"] = DajNazweKlasy(sparsowane);
            wynik["NAMESPACE_KLASY"] = sparsowane.Namespace;
            wynik["NAZWA_PLIKU"] = solution.AktualnyPlik.Nazwa;
            wynik["NAZWA_PLIKU_BEZ_ROZSZERZENIA"] =
                solution.AktualnyPlik.NazwaBezRozszerzenia;

            foreach (var zmienna in schematKlasy.Zmienne)
            {
                var pasujacyPlik =
                    solution.AktualnyProjekt.Pliki
                    .SingleOrDefault(o => PasujePlik(o, zmienna.DopasowaniePliku));

                if (zmienna.BezRozszerzenia)
                {
                    wynik[zmienna.Symbol] = pasujacyPlik.NazwaBezRozszerzenia;
                }
                else
                    wynik[zmienna.Symbol] = pasujacyPlik.Nazwa;
            }

            return wynik;
        }

        private bool PasujePlik(IPlikWrapper plik, string dopasowaniePliku)
        {
            var regex = new Regex(dopasowaniePliku);

            var match = regex.Match(plik.SciezkaPelna);

            return match.Success;
        }

        private string DajNazweKlasy(Plik sparsowane)
        {
            var obiekt =
            sparsowane.SzukajKlasyWLinii(solution.AktualnyDokument.DajNumerLiniiKursora());

            if (obiekt == null)
                return sparsowane.DefiniowaneObiekty.Single().Nazwa;
            else
                return obiekt.Nazwa;
        }
    }
}