using System.Windows;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using System.Text;
using KruchyCompany.KruchyPlugin1.CodeBuilders;
using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieMetodWImplementacji
    {
        private readonly SolutionWrapper solution;

        public UzupelnianieMetodWImplementacji(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Uzupelnij()
        {
            var aktualnyPlik = solution.AktualnyPlik;
            if (!aktualnyPlik.JestInterfejsem())
            {
                MessageBox.Show("Aktualny plik nie jest interfejsem");
                return;
            }

            var parsowane = Parser.Parsuj(aktualnyPlik.Dokument.DajZawartosc());

            var dokument = aktualnyPlik.Dokument;
            var aktualnaMetoda =
                parsowane.SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());

            if (aktualnaMetoda == null)
            {
                MessageBox.Show("Kursor nie stoi ma metodzie");
                return;
            }

            var sciezkaDoImplementacji =
                solution.AktualnyPlik.SzukajSciezkiDoImplementacji();

            if (sciezkaDoImplementacji == null)
            {
                MessageBox.Show("Nie znaleziono implementacji");
                return;
            }

            var definicja = dokument.DajZawartosc(
                aktualnaMetoda.Poczatek.Wiersz, aktualnaMetoda.Poczatek.Kolumna,
                aktualnaMetoda.Koniec.Wiersz, aktualnaMetoda.Koniec.Kolumna);

            DodajDefincjeWImplementacji(
                sciezkaDoImplementacji, definicja, parsowane.Usingi);
        }

        private void DodajDefincjeWImplementacji(
            string sciezkaDoImplementacji,
            string definicja,
            IEnumerable<string> usingi)
        {
            var solutionExplorer = new SolutionExplorerWrapper(solution);
            solutionExplorer.OtworzPlik(sciezkaDoImplementacji);

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            var numerLiniiGdzieDodawac = parsowane.SzukajPierwszejLiniiDlaMetody();

            string wstawianyTekst =
                GenerujTekstDoWstawienia(definicja);
            solution.AktualnyDokument
                .WstawWLinii(wstawianyTekst, numerLiniiGdzieDodawac);

            foreach (var u in usingi)
                solution.AktualnyDokument.DodajUsingaJesliTrzeba(u);
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