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

        public void Generuj(string nazwaSzablonu, bool directoryFromSelectemItem = false)
        {
            var konf = Konfiguracja.GetInstance(solution);

            var szablon =
                konf
                    .SchematyGenerowania()
                        .Single(o => o.TytulSchematu == nazwaSzablonu);

            string wybranaSciezka = null;

            IProjectWrapper wybranyProjekt = null;

            IDictionary<string, object> variableValues = new Dictionary<string, object>();
            IGeneratingFromTemplateParamsWindow dialogAdd =
                UIObjects.FactoryInstance.Get<IGeneratingFromTemplateParamsWindow>();

            dialogAdd.CanSelectDirectory = szablon.WyborSciezki && !directoryFromSelectemItem;

            dialogAdd.Projects = solution.Projekty;

            dialogAdd.VariablesToFill =
                szablon.Zmienne.Where(o => o.WprowadzanaWUI)
                    .Select(o => new VariableToFill
                    {
                        Name = o.Symbol,
                        Type = "string",
                        InitialValue = o.WartoscInicjalna
                    });

            UIObjects.ShowWindowModal(dialogAdd);

            if (dialogAdd.Cancelled)
                return;

            variableValues = dialogAdd.VariablesValues;

            if (directoryFromSelectemItem)
            {
                wybranaSciezka = solutionExplorer.GetSelection().GetSingleSelectedFolder()?.FullPath;
                wybranyProjekt = solution.Projekty.Single(o => wybranaSciezka.StartsWith(o.DirectoryPath));
            }
            else
            {
                wybranaSciezka = dialogAdd.Directory;

                wybranyProjekt = dialogAdd.SelectedProject;
            }

            if (wybranyProjekt == null)
                wybranyProjekt = solution.AktualnyProjekt;

            var sparsowane = Parser.Parsuj(solution.AktualnyDokument.GetContent());

            foreach (var schematKlasy in szablon.SchematyKlas)
            {
                GenerujWgSchematu(szablon, schematKlasy, sparsowane, wybranaSciezka, wybranyProjekt, variableValues);
            }
        }

        private void GenerujWgSchematu(
            SchematGenerowania szablon,
            SchematKlasy schematKlasy,
            Plik sparsowane,
            string wybranaSciezka,
            IProjectWrapper wybranyProjekt,
            IDictionary<string, object> variableValues)
        {
            var sciezkaDoPliku =
                Path.Combine(
                    wybranyProjekt.DirectoryPath,
                    DajNazwePliku(szablon, schematKlasy, sparsowane, wybranaSciezka, wybranyProjekt, variableValues));

            if (File.Exists(sciezkaDoPliku))
            {
                MessageBox.Show(string.Format("Plik {0} już istnieje", sciezkaDoPliku));
                return;
            }

            var tresc = schematKlasy.Tresc;

            tresc = ZamienZmienneNaWartosci(szablon, tresc, sparsowane, wybranaSciezka, wybranyProjekt, variableValues);

            var fileInfo = new FileInfo(sciezkaDoPliku);

            if (!System.IO.Directory.Exists(fileInfo.Directory.FullName))
                System.IO.Directory.CreateDirectory(fileInfo.Directory.FullName);

            File.WriteAllText(sciezkaDoPliku, tresc, Encoding.UTF8);
            wybranyProjekt.AddFile(sciezkaDoPliku);
            solutionExplorer.OpenFile(sciezkaDoPliku);
        }

        private string DajNazwePliku(
            SchematGenerowania szablon,
            SchematKlasy schematKlasy,
            Plik sparsowane,
            string wybranaSciezka,
            IProjectWrapper wybranyProjekt,
            IDictionary<string, object> variableValues)
        {
            return ZamienZmienneNaWartosci(
                szablon,
                schematKlasy.NazwaPliku,
                sparsowane,
                wybranaSciezka,
                wybranyProjekt,
                variableValues);
        }

        private string ZamienZmienneNaWartosci(
            SchematGenerowania szablon,
            string tekst,
            Plik sparsowane,
            string wybranaSciezka,
            IProjectWrapper wybranyProjekt,
            IDictionary<string, object> variableValues)
        {
            var zmienne = PrzygotujWartosciZmiennych(sparsowane, wybranaSciezka, wybranyProjekt, variableValues);

            foreach (var zmienna in zmienne)
                tekst = tekst.Replace("%" + zmienna.Key + "%", zmienna.Value);

            return tekst;
        }

        private Dictionary<string, string> PrzygotujWartosciZmiennych(
            Plik sparsowane,
            string wybranaSciezka,
            IProjectWrapper wybranyProjekt,
            IDictionary<string, object> variableValues)
        {
            var wynik = new Dictionary<string, string>();

            wynik["NAZWA_KLASY"] = DajNazweKlasy(sparsowane);
            wynik["NAZWA_PROJEKTU"] = solution.AktualnyProjekt.Name.Replace(".csproj", "");
            wynik["NAMESPACE_KLASY"] = sparsowane.Namespace;
            wynik["NAZWA_PLIKU"] = solution.AktualnyPlik.Name;
            wynik["NAZWA_PLIKU_BEZ_ROZSZERZENIA"] =
                solution.AktualnyPlik.NameWithoutExtension;
            wynik["WYBRANA_SCIEZKA"] = wybranaSciezka;
            var sciezkaWProjekcie = DajSciezkeWProjekcie(wybranaSciezka, wybranyProjekt);
            wynik["SCIEZKA_W_PROJEKCIE"] = sciezkaWProjekcie;
            wynik["NOWY_NAMESPACE"] =
                $"{wybranyProjekt?.Name}.{sciezkaWProjekcie.Replace("\\", ".").Replace("/", ".")}".TrimEnd('.');

            foreach (var pair in variableValues)
            {
                wynik[pair.Key] = pair.Value?.ToString();
            }

            return wynik;
        }

        private static string DajSciezkeWProjekcie(string wybranaSciezka, IProjectWrapper wybranyProjekt)
        {
            var wynik = wybranaSciezka.Substring(wybranyProjekt.DirectoryPath.Length);

            if (wynik.Length > 0 && (wynik[0] == '\\' || wynik[0] == '/'))
                return wynik.Substring(1);

            return wynik;
        }

        private bool PasujePlik(IFileWrapper plik, string dopasowaniePliku)
        {
            var regex = new Regex(dopasowaniePliku);

            var match = regex.Match(plik.FullPath);

            return match.Success;
        }

        private string DajNazweKlasy(Plik sparsowane)
        {
            var obiekt =
            sparsowane.SzukajKlasyWLinii(solution.AktualnyDokument.GetCursorLineNumber());

            if (obiekt == null)
                return sparsowane.DefiniowaneObiekty.Single().Name;
            else
                return obiekt.Name;
        }
    }
}