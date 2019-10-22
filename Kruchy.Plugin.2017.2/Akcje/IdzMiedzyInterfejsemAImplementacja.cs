using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzMiedzyInterfejsemAImplementacja
    {
        private readonly ISolutionWrapper solution;

        public IdzMiedzyInterfejsemAImplementacja(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Przejdz()
        {
            var aktualny = solution.AktualnyPlik;
            if (aktualny == null)
                return;

            if (JestInterfejsem(aktualny))
                SprobujPrzejscDoImplementacji(aktualny);
            else
                SprobujPrzejscDoInterfejsu(aktualny);
        }

        private bool JestInterfejsem(IPlikWrapper aktualny)
        {
            var zawartosc = aktualny.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                return parsowane.DefiniowaneObiekty[0].Rodzaj == RodzajObiektu.Interfejs;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        private void SprobujPrzejscDoImplementacji(IPlikWrapper aktualny)
        {
            var parsowane = Parser.Parsuj(aktualny.Dokument.DajZawartosc());
            var metoda =
                parsowane.SzukajMetodyWLinii(
                    aktualny.Dokument.DajNumerLiniiKursora());

            string sciezkaImplementacji = SzukajSciezkiDoImplementacji(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaImplementacji);

            if (!string.IsNullOrEmpty(sciezkaImplementacji) && metoda != null)
            {
                UstawSieNaMetodzie(metoda);
            }
        }

        private void UstawSieNaMetodzie(Metoda metoda)
        {
            var parsowane =
                Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());

            if (parsowane.DefiniowaneObiekty.Count != 1)
                return;

            var znalezionaMetoda =
                parsowane.DefiniowaneObiekty[0].Metody
                    .Where(o => metoda.TaSamaMetoda(o))
                        .FirstOrDefault();

            if (znalezionaMetoda != null)
                solution.AktualnyDokument.UstawKursor(
                    znalezionaMetoda.Poczatek.Wiersz,
                    znalezionaMetoda.Poczatek.Kolumna);
        }

        private string SzukajSciezkiDoImplementacji(IPlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogImpl = Path.Combine(katalog, "Impl");
            var nazwa = aktualny.Nazwa.Substring(1);
            var sciezka = Path.Combine(katalogImpl, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            return null;
        }

        private void SprobujPrzejscDoInterfejsu(IPlikWrapper aktualny)
        {
            string sciezkaDoInterfejsu = SzukajSciezkiDoInterfejsu(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaDoInterfejsu);
        }

        private string SzukajSciezkiDoInterfejsu(IPlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogInterfejsu = Directory.GetParent(katalog).FullName;
            var nazwa = "I" + aktualny.Nazwa;
            var sciezka = Path.Combine(katalogInterfejsu, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            MessageBox.Show("Nie znalazłem " + sciezka);
            return null;
        }

        private void OtworzJesliSciezkaNieNullowa(string sciezka)
        {
            if (sciezka == null)
                return;
            var solutionExplorer = SolutionExplorerWrapper.DajDlaSolution(solution);
            solutionExplorer.OtworzPlik(sciezka);
        }
    }
}