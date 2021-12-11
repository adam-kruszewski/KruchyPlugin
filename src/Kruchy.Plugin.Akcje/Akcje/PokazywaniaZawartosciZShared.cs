using System.IO;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class PokazywaniaZawartosciZShared
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PokazywaniaZawartosciZShared(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Pokaz()
        {
            var plik = solution.AktualnyPlik;

            var sciezkaWShared =
                solution.AktualnyProjekt.SciezkaDoPlikuWShared(
                    solution.AktualnyPlik.Nazwa);

            if (!File.Exists(sciezkaWShared))
            {
                MessageBox.Show("W Shared nie ma pliku: " + sciezkaWShared);
                return;
            }

            solutionExplorer.OtworzPlik(sciezkaWShared);
        }
    }
}