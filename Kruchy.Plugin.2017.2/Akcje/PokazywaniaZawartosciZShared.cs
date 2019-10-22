using System.IO;
using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class PokazywaniaZawartosciZShared
    {
        private readonly ISolutionWrapper solution;

        public PokazywaniaZawartosciZShared(ISolutionWrapper solution)
        {
            this.solution = solution;
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

            solution.OtworzPlik(sciezkaWShared);
        }
    }
}