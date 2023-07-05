using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
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
            var aktualnyPlik = solution.AktualnyPlik;
            if (!aktualnyPlik.JestInterfejsem())
            {
                MessageBox.Show("Aktualny plik nie jest interfejsem");
                return;
            }

            var parsowane = Parser.Parsuj(aktualnyPlik.Document.GetContent());

            var dokument = aktualnyPlik.Document;
            var aktualnaMetoda =
                parsowane.SzukajMetodyWLinii(dokument.GetCursorLineNumber());

            if (aktualnaMetoda == null)
            {
                MessageBox.Show("Kursor nie stoi ma metodzie");
                return;
            }

            Metoda nastepnaMetoda = SzukajNastepnejMetody(parsowane, aktualnaMetoda);

            var sciezkaDoImplementacji =
                solution.AktualnyPlik.SzukajSciezkiDoImplementacji();

            if (sciezkaDoImplementacji == null)
            {
                MessageBox.Show("Nie znaleziono implementacji");
                return;
            }

            var definicja = dokument.GetContent(
                aktualnaMetoda.Poczatek.Row, aktualnaMetoda.Poczatek.Column,
                aktualnaMetoda.Koniec.Row, aktualnaMetoda.Koniec.Column);

            DodajDefincjeWImplementacji(
                sciezkaDoImplementacji,
                definicja,
                parsowane.Usingi,
                nastepnaMetoda);
        }

        private Metoda SzukajNastepnejMetody(Plik parsowane, Metoda aktualnaMetoda)
        {
            var interfejs = parsowane.SzukajObiektuWLinii(aktualnaMetoda.Poczatek.Row);
            if (interfejs == null)
                return null;

            return
                interfejs
                    .Metody
                        .SkipWhile(o => !o.TaSamaMetoda(aktualnaMetoda))
                            .Skip(1)
                                .FirstOrDefault();
        }

        private void DodajDefincjeWImplementacji(
            string sciezkaDoImplementacji,
            string definicja,
            IEnumerable<UsingNamespace> usingi,
            Metoda nastepnaMetoda)
        {
            solutionExplorer.OpenFile(sciezkaDoImplementacji);

            var zawartosc = solution.AktualnyDokument.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            int numerLiniiGdzieDodawac = 0;

            Metoda nastepnaMetodaWImplementacji = null;
            if (nastepnaMetoda == null)
                numerLiniiGdzieDodawac = parsowane.SzukajPierwszejLiniiDlaMetody();
            else
            {
                nastepnaMetodaWImplementacji =
                    parsowane
                        .DefiniowaneObiekty
                            .SelectMany(o => o.Metody)
                                .FirstOrDefault(o => o.TaSamaMetoda(nastepnaMetoda));

            }
            string wstawianyTekst = GenerujTekstDoWstawienia(definicja);

            if (nastepnaMetodaWImplementacji == null)
                numerLiniiGdzieDodawac = parsowane.SzukajPierwszejLiniiDlaMetody();
            else
            {
                numerLiniiGdzieDodawac =
                    WyliczLinieDodanieWgNastepnejMetody(
                        parsowane,
                        nastepnaMetodaWImplementacji,
                        ref wstawianyTekst);

            }

            solution.AktualnyDokument
                .InsertInLine(wstawianyTekst, numerLiniiGdzieDodawac);
            solution.AktualnyDokument.SetCursorForAddedMethod(
                numerLiniiGdzieDodawac + 1);

            foreach (var u in usingi.Select(o => o.Nazwa))
                solution.AktualnyDokument.DodajUsingaJesliTrzeba(u);
        }

        private static int WyliczLinieDodanieWgNastepnejMetody(
            Plik parsowane,
            Metoda nastepnaMetodaWImplementacji,
            ref string wstawianyTekst)
        {
            int numerLiniiGdzieDodawac;
            var obiekt =
                parsowane
                .SzukajObiektuWLinii(nastepnaMetodaWImplementacji.Poczatek.Row);

            numerLiniiGdzieDodawac = nastepnaMetodaWImplementacji.Poczatek.Row - 1;

            if (nastepnaMetodaWImplementacji?.Dokumentacja?.Lines != null)
                numerLiniiGdzieDodawac -= nastepnaMetodaWImplementacji.Dokumentacja.Lines.Count;

            if (nastepnaMetodaWImplementacji?.Komentarz?.Lines != null)
                numerLiniiGdzieDodawac -= nastepnaMetodaWImplementacji.Komentarz.Lines.Count;

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
            builder.Append(StaleDlaKodu.WciecieDlaMetody);
            builder.Append("public ");
            builder.AppendLine(def);

            builder.AppendLine(StaleDlaKodu.WciecieDlaMetody + "{");
            builder.Append(StaleDlaKodu.WciecieDlaMetody);
            builder.Append(StaleDlaKodu.JednostkaWciecia);
            builder.AppendLine("throw new System.NotImplementedException();");
            builder.AppendLine(StaleDlaKodu.WciecieDlaMetody + "}");
            return builder.ToString();
        }
    }
}
