using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class IdzMiedzyInterfejsemAImplementacja
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public IdzMiedzyInterfejsemAImplementacja(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
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
            var zawartosc = aktualny.Dokument.GetContent();
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
            var parsowane = Parser.Parsuj(aktualny.Dokument.GetContent());
            var metoda =
                parsowane.SzukajMetodyWLinii(
                    aktualny.Dokument.GetCursorLineNumber());

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
                Parser.Parsuj(solution.AktualnyDokument.GetContent());

            if (parsowane.DefiniowaneObiekty.Count != 1)
                return;

            var znalezionaMetoda =
                parsowane.DefiniowaneObiekty[0].Metody
                    .Where(o => metoda.TaSamaMetoda(o))
                        .FirstOrDefault();

            if (znalezionaMetoda != null)
                solution.AktualnyDokument.SetCursor(
                    znalezionaMetoda.Poczatek.Wiersz,
                    znalezionaMetoda.Poczatek.Kolumna);
        }

        private string SzukajSciezkiDoImplementacji(IPlikWrapper aktualny)
        {
            var wynik = aktualny.SzukajSciezkiDoImplementacji();

            if (wynik == null)
                MessageBox.Show("Nie znalazłem");

            return wynik;
        }

        private void SprobujPrzejscDoInterfejsu(IPlikWrapper aktualny)
        {
            string sciezkaDoInterfejsu = SzukajSciezkiDoInterfejsu(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaDoInterfejsu);
        }

        private string SzukajSciezkiDoInterfejsu(IPlikWrapper aktualny)
        {
            var wynik = aktualny.SzukajSciezkiDoInterfejsu();

            if (wynik == null)
                MessageBox.Show("Nie znalazłem");

            return wynik;
        }

        private void OtworzJesliSciezkaNieNullowa(string sciezka)
        {
            if (sciezka == null)
                return;
            solutionExplorer.OpenFile(sciezka);
        }
    }
}
