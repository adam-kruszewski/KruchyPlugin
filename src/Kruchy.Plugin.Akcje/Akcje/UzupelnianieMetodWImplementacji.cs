using KruchyCodeBuilders.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class UzupelnianieMetodWImplementacji
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public UzupelnianieMetodWImplementacji(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Uzupelnij()
        {
            var aktualnyPlik = solution.CurrentFile;
            if (!aktualnyPlik.JestInterfejsem())
            {
                MessageBox.Show("Aktualny plik nie jest interfejsem");
                return;
            }

            var parsowane = Parser.Parse(aktualnyPlik.Document.GetContent());

            var dokument = aktualnyPlik.Document;
            var aktualnaMetoda =
                parsowane.FindMethodByLineNumber(dokument.GetCursorLineNumber());

            if (aktualnaMetoda == null)
            {
                MessageBox.Show("Kursor nie stoi ma metodzie");
                return;
            }

            Method nastepnaMetoda = SzukajNastepnejMetody(parsowane, aktualnaMetoda);

            var sciezkaDoImplementacji =
                solution.CurrentFile.SzukajSciezkiDoImplementacji();

            if (sciezkaDoImplementacji == null)
            {
                MessageBox.Show("Nie znaleziono implementacji");
                return;
            }

            var definicja = dokument.GetContent(
                aktualnaMetoda.StartPosition.Row, aktualnaMetoda.StartPosition.Column,
                aktualnaMetoda.EndPosition.Row, aktualnaMetoda.EndPosition.Column);

            DodajDefincjeWImplementacji(
                sciezkaDoImplementacji,
                definicja,
                parsowane.Usings,
                nastepnaMetoda);
        }

        private Method SzukajNastepnejMetody(FileWithCode parsowane, Method aktualnaMetoda)
        {
            var interfejs = parsowane.FindDefinedItemByLineNumber(aktualnaMetoda.StartPosition.Row);
            if (interfejs == null)
                return null;

            return
                interfejs
                    .Methods
                        .SkipWhile(o => !o.TheSameMethod(aktualnaMetoda))
                            .Skip(1)
                                .FirstOrDefault();
        }

        private void DodajDefincjeWImplementacji(
            string sciezkaDoImplementacji,
            string definicja,
            IEnumerable<UsingNamespace> usingi,
            Method nastepnaMetoda)
        {
            solutionExplorer.OpenFile(sciezkaDoImplementacji);

            var zawartosc = solution.CurentDocument.GetContent();
            var parsowane = Parser.Parse(zawartosc);
            int numerLiniiGdzieDodawac = 0;

            Method nastepnaMetodaWImplementacji = null;
            if (nastepnaMetoda == null)
                numerLiniiGdzieDodawac = parsowane.FindFirstLineForMethod();
            else
            {
                nastepnaMetodaWImplementacji =
                    parsowane
                        .DefinedItems
                            .SelectMany(o => o.Methods)
                                .FirstOrDefault(o => o.TheSameMethod(nastepnaMetoda));

            }
            string wstawianyTekst = GenerujTekstDoWstawienia(definicja);

            if (nastepnaMetodaWImplementacji == null)
                numerLiniiGdzieDodawac = parsowane.FindFirstLineForMethod();
            else
            {
                numerLiniiGdzieDodawac =
                    WyliczLinieDodanieWgNastepnejMetody(
                        parsowane,
                        nastepnaMetodaWImplementacji,
                        ref wstawianyTekst);

            }

            solution.CurentDocument
                .InsertInLine(wstawianyTekst, numerLiniiGdzieDodawac);
            solution.CurentDocument.SetCursorForAddedMethod(
                numerLiniiGdzieDodawac + 1);

            foreach (var u in usingi.Select(o => o.Name))
                solution.CurentDocument.DodajUsingaJesliTrzeba(u);
        }

        private static int WyliczLinieDodanieWgNastepnejMetody(
            FileWithCode parsowane,
            Method nastepnaMetodaWImplementacji,
            ref string wstawianyTekst)
        {
            int numerLiniiGdzieDodawac;
            var obiekt =
                parsowane
                .FindDefinedItemByLineNumber(nastepnaMetodaWImplementacji.StartPosition.Row);

            numerLiniiGdzieDodawac = nastepnaMetodaWImplementacji.StartPosition.Row - 1;

            if (nastepnaMetodaWImplementacji?.Documentation?.Lines != null)
                numerLiniiGdzieDodawac -= nastepnaMetodaWImplementacji.Documentation.Lines.Count;

            if (nastepnaMetodaWImplementacji?.Comment?.Lines != null)
                numerLiniiGdzieDodawac -= nastepnaMetodaWImplementacji.Comment.Lines.Count;

            if (numerLiniiGdzieDodawac <= obiekt.StartingBrace.Row)
            {
                numerLiniiGdzieDodawac = obiekt.StartingBrace.Row + 1;
                wstawianyTekst += new StringBuilder().AppendLine().ToString();
            }

            return numerLiniiGdzieDodawac;
        }

        private string GenerujTekstDoWstawienia(string definicja)
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            var def = definicja.TrimStart().Replace(";", "");
            builder.Append(ConstsForCode.DefaultIndentForMethod);
            builder.Append("public ");
            builder.AppendLine(def);

            builder.AppendLine(ConstsForCode.DefaultIndentForMethod + "{");
            builder.Append(ConstsForCode.DefaultIndentForMethod);
            builder.Append(ConstsForCode.IndentUnit);
            builder.AppendLine("throw new System.NotImplementedException();");
            builder.AppendLine(ConstsForCode.DefaultIndentForMethod + "}");
            return builder.ToString();
        }
    }
}
