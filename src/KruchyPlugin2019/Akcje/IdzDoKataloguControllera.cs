using System.IO;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
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
            var aktualny = solution.AktualnyPlik;
            if (aktualny == null)
                return;

            if (!aktualny.Nazwa.ToLower().EndsWith("controller.cs"))
            {
                MessageBox.Show("To nie jest plik controllera");
                return;
            }
            string nazwaControllera =
                DajNazweControllera(aktualny.NazwaBezRozszerzenia);

            var katalogPlikControllera = aktualny.Katalog;
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


            solutionExplorer.UstawSieNaMiejscu(katalogDlaControllera);
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