using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.IO;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class GenerowanieWidoku
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieWidoku(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(string nazwa)
        {
            if (!solution.CzyPlikControllera())
            {
                MessageBox.Show("To nie jest plik controllera");
                return;
            }
            nazwa = Normalizuj(nazwa);

            var katalogControllera =
                solution.AktualnyPlik.SciezkaKataloguControllera();
            if (!Directory.Exists(katalogControllera))
                Directory.CreateDirectory(katalogControllera);

            var pelnaSciezka = Path.Combine(katalogControllera, nazwa);
            if (File.Exists(pelnaSciezka))
            {
                MessageBox.Show("Plik " + pelnaSciezka + " już istnieje");
                return;
            }
            File.WriteAllText(pelnaSciezka, "");
            solution.AktualnyProjekt.AddFile(pelnaSciezka);
            solutionExplorer.OpenFile(pelnaSciezka);
        }

        private string Normalizuj(string nazwa)
        {
            if (!nazwa.ToLower().EndsWith(".cshtml"))
                return nazwa + ".cshtml";
            else
                return nazwa;
        }
    }
}
