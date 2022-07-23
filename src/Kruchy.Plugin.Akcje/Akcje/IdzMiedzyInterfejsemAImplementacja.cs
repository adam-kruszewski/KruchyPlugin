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

        private bool JestInterfejsem(IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                return parsowane.DefiniowaneObiekty[0].Rodzaj == RodzajObiektu.Interfejs;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        private void SprobujPrzejscDoImplementacji(IFileWrapper aktualny)
        {
            var parsowane = Parser.Parsuj(aktualny.Document.GetContent());
            var metoda =
                parsowane.SzukajMetodyWLinii(
                    aktualny.Document.GetCursorLineNumber());

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

        private string SzukajSciezkiDoImplementacji(IFileWrapper aktualny)
        {
            var wynik = aktualny.SzukajSciezkiDoImplementacji();

            if (wynik == null)
                MessageBox.Show("Nie znalazłem");

            return wynik;
        }

        private void SprobujPrzejscDoInterfejsu(IFileWrapper aktualny)
        {
            string sciezkaDoInterfejsu = SzukajSciezkiDoInterfejsu(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaDoInterfejsu);
        }

        private string SzukajSciezkiDoInterfejsu(IFileWrapper aktualny)
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
