using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KrucheBuilderyKodu.Builders;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

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

            var bylaAktualna = false;

            //return
            //klasa.Metody.SkipWhile(o => o == aktualnaMetoda).Take(2).LastOrDefault();

            foreach (var m in interfejs.Metody)
            {
                if (bylaAktualna)
                    return m;

                if (m == aktualnaMetoda)
                    bylaAktualna = true;
            }

            return null;
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

            if (nastepnaMetodaWImplementacji == null)
                numerLiniiGdzieDodawac = parsowane.SzukajPierwszejLiniiDlaMetody();
            else
                numerLiniiGdzieDodawac = nastepnaMetodaWImplementacji.Poczatek.Wiersz -1;

            string wstawianyTekst =
                GenerujTekstDoWstawienia(definicja);
            solution.AktualnyDokument
                .WstawWLinii(wstawianyTekst, numerLiniiGdzieDodawac);
            solution.AktualnyDokument.UstawKursosDlaMetodyDodanejWLinii(
                numerLiniiGdzieDodawac + 1);

            foreach (var u in usingi.Select(o => o.Nazwa))
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