using System.IO;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class IdzDoKataloguControllera
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public IdzDoKataloguControllera(
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

            if (!aktualny.Name.ToLower().EndsWith("controller.cs"))
            {
                MessageBox.Show("To nie jest plik controllera");
                return;
            }
            string nazwaControllera =
                DajNazweControllera(aktualny.NameWithoutExtension);

            var katalogPlikControllera = aktualny.Directory;
            var katalogDlaControllera =
                Path.Combine(
                    Directory.GetParent(katalogPlikControllera).FullName,
                    "Views",
                    nazwaControllera);
            if (!Directory.Exists(katalogDlaControllera))
            {
                MessageBox.Show("Brak katalogu dla controllera " + nazwaControllera);
                return;
            }


            solutionExplorer.SelectPath(katalogDlaControllera);
        }

        private string DajNazweControllera(string nazwaKlasyControllera)
        {
            var dl = "Controller".Length;
            return nazwaKlasyControllera.Substring(
                0,
                nazwaKlasyControllera.Length - dl);
        }
    }
}
