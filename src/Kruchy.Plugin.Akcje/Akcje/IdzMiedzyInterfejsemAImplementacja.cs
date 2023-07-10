using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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
            var aktualny = solution.CurrentFile;
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
            var parsowane = Parser.Parse(zawartosc);
            if (parsowane.DefinedItems.Count == 1)
            {
                return parsowane.DefinedItems[0].KindOfItem == KindOfItem.Interface;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        private void SprobujPrzejscDoImplementacji(IFileWrapper aktualny)
        {
            var parsowane = Parser.Parse(aktualny.Document.GetContent());
            var metoda =
                parsowane.FindMethodByLineNumber(
                    aktualny.Document.GetCursorLineNumber());

            string sciezkaImplementacji = SzukajSciezkiDoImplementacji(aktualny);
            OtworzJesliSciezkaNieNullowa(sciezkaImplementacji);

            if (!string.IsNullOrEmpty(sciezkaImplementacji) && metoda != null)
            {
                UstawSieNaMetodzie(metoda);
            }
        }

        private void UstawSieNaMetodzie(Method metoda)
        {
            var parsowane =
                Parser.Parse(solution.CurentDocument.GetContent());

            if (parsowane.DefinedItems.Count != 1)
                return;

            var znalezionaMetoda =
                parsowane.DefinedItems[0].Methods
                    .Where(o => metoda.TheSameMethod(o))
                        .FirstOrDefault();

            if (znalezionaMetoda != null)
                solution.CurentDocument.SetCursor(
                    znalezionaMetoda.StartPosition.Row,
                    znalezionaMetoda.StartPosition.Column);
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
