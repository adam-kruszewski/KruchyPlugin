using System.IO;
using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class IdzDoKataloguControllera
    {
        private readonly SolutionWrapper solution;

        public IdzDoKataloguControllera(SolutionWrapper solution)
        {
            this.solution = solution;
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

            var explorer = new SolutionExplorerWrapper(solution);
            explorer.UstawSieNaMiejscu(katalogDlaControllera);
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