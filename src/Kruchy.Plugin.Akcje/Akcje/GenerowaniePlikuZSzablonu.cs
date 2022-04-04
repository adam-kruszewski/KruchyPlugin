using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.UI;
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

            string wybranaSciezka = null;

            IProjektWrapper wybranyProjekt = null;

            if (szablon.WyborSciezki)
            {
                IGeneratingFromTemplateParamsWindow dialogAdd =
                    UIObjects.FactoryInstance.Get<IGeneratingFromTemplateParamsWindow>();

                dialogAdd.Projects = solution.Projekty;

                dialogAdd.VariablesToFill =
                    szablon.Zmienne.Where(o => o.WprowadzanaWUI)
                        .Select(o => new VariableToFill {
                            Name = o.Symbol,
                            Type = "string",
                            InitialValue = o.WartoscInicjalna });

                UIObjects.ShowWindowModal(dialogAdd);

                if (string.IsNullOrEmpty(dialogAdd.Directory))
                    return;

                var variableValues = dialogAdd.VariablesValues;

                wybranaSciezka = dialogAdd.Directory;
                wybranyProjekt = dialogAdd.SelectedProject;
            }

            if (wybranyProjekt == null)
                wybranyProjekt = solution.AktualnyProjekt;

            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            foreach (var schematKlasy in szablon.SchematyKlas)
            {
                GenerujWgSchematu(szablon, schematKlasy, sparsowane, wybranaSciezka, wybranyProjekt);
            }
        }

        private void GenerujWgSchematu(
            SchematGenerowania szablon,
            SchematKlasy schematKlasy,
            Plik sparsowane,
            string wybranaSciezka,
            IProjektWrapper wybranyProjekt)
        {
            var sciezkaDoPliku =
                Path.Combine(
                    wybranyProjekt.SciezkaDoKatalogu,
                    DajNazwePliku(szablon, schematKlasy, sparsowane, wybranaSciezka, wybranyProjekt));

            if (File.Exists(sciezkaDoPliku))
            {
                MessageBox.Show(string.Format("Plik {0} już istnieje", sciezkaDoPliku));
                return;
            }

            var tresc = schematKlasy.Tresc;

            tresc = ZamienZmienneNaWartosci(szablon, tresc, sparsowane, wybranaSciezka, wybranyProjekt);

            var fileInfo = new FileInfo(sciezkaDoPliku);

            if (!System.IO.Directory.Exists(fileInfo.Directory.FullName))
                System.IO.Directory.CreateDirectory(fileInfo.Directory.FullName);

            File.WriteAllText(sciezkaDoPliku, tresc, Encoding.UTF8);
            wybranyProjekt.DodajPlik(sciezkaDoPliku);
            solutionExplorer.OtworzPlik(sciezkaDoPliku);
        }

        private string DajNazwePliku(
            SchematGenerowania szablon,
            SchematKlasy schematKlasy,
            Plik sparsowane,
            string wybranaSciezka,
            IProjektWrapper wybranyProjekt)
        {
            return ZamienZmienneNaWartosci(
                szablon,
                schematKlasy.NazwaPliku,
                sparsowane,
                wybranaSciezka,
                wybranyProjekt);
        }

        private string ZamienZmienneNaWartosci(
            SchematGenerowania szablon,
            string tekst,
            Plik sparsowane,
            string wybranaSciezka,
            IProjektWrapper wybranyProjekt)
        {
            var zmienne = PrzygotujWartosciZmiennych(sparsowane, wybranaSciezka, wybranyProjekt);

            foreach (var zmienna in zmienne)
                tekst = tekst.Replace("%" + zmienna.Key + "%", zmienna.Value);

            return tekst;
        }

        private Dictionary<string, string> PrzygotujWartosciZmiennych(
            Plik sparsowane,
            string wybranaSciezka,
            IProjektWrapper wybranyProjekt)
        {
            var wynik = new Dictionary<string, string>();

            wynik["NAZWA_KLASY"] = DajNazweKlasy(sparsowane);
            wynik["NAZWA_PROJEKTU"] = solution.AktualnyProjekt.Nazwa.Replace(".csproj", "");
            wynik["NAMESPACE_KLASY"] = sparsowane.Namespace;
            wynik["NAZWA_PLIKU"] = solution.AktualnyPlik.Nazwa;
            wynik["NAZWA_PLIKU_BEZ_ROZSZERZENIA"] =
                solution.AktualnyPlik.NazwaBezRozszerzenia;
            wynik["WYBRANA_SCIEZKA"] = wybranaSciezka;
            wynik["SCIEZKA_W_PROJEKCIE"] = DajSciezkeWProjekcie(wybranaSciezka, wybranyProjekt);

            //foreach (var zmienna in schematKlasy.Zmienne.Where(o => !o.WprowadzanaWUI))
            //{
            //    var pasujacyPlik =
            //        solution.AktualnyProjekt.Pliki
            //        .SingleOrDefault(o => PasujePlik(o, zmienna.DopasowaniePliku));

            //    if (zmienna.BezRozszerzenia)
            //    {
            //        wynik[zmienna.Symbol] = pasujacyPlik.NazwaBezRozszerzenia;
            //    }
            //    else
            //        wynik[zmienna.Symbol] = pasujacyPlik.Nazwa;
            //}

            return wynik;
        }

        private static string DajSciezkeWProjekcie(string wybranaSciezka, IProjektWrapper wybranyProjekt)
        {
            var wynik = wybranaSciezka.Substring(wybranyProjekt.SciezkaDoKatalogu.Length);

            if (wynik.Length > 0 && (wynik[0] == '\\' || wynik[0] == '/'))
                return wynik.Substring(1);

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