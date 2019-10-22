using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class UzupelnianieMetodWImplementacji
    {
        private readonly ISolutionWrapper solution;

        public UzupelnianieMetodWImplementacji(ISolutionWrapper solution)
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

            Metoda nastepnaMetoda = SzukajNastepnejMetody(parsowane, aktualnaMetoda);

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
                sciezkaDoImplementacji,
                definicja,
                parsowane.Usingi,
                nastepnaMetoda);
        }

        private Metoda SzukajNastepnejMetody(Plik parsowane, Metoda aktualnaMetoda)
        {
            var interfejs = parsowane.SzukajObiektuWLinii(aktualnaMetoda.Poczatek.Wiersz);
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
            var solutionExplorer = new SolutionExplorerWrapper(solution);
            solutionExplorer.OtworzPlik(sciezkaDoImplementacji);

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
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
                .WstawWLinii(wstawianyTekst, numerLiniiGdzieDodawac);
            solution.AktualnyDokument.UstawKursosDlaMetodyDodanejWLinii(
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
                .SzukajObiektuWLinii(nastepnaMetodaWImplementacji.Poczatek.Wiersz);

            numerLiniiGdzieDodawac = nastepnaMetodaWImplementacji.Poczatek.Wiersz - 1;
            if (numerLiniiGdzieDodawac <= obiekt.PoczatkowaKlamerka.Wiersz)
            {
                numerLiniiGdzieDodawac = obiekt.PoczatkowaKlamerka.Wiersz + 1;
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