using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class DodawanieUsinga
    {
        private readonly ISolutionWrapper solution;

        public DodawanieUsinga(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj(params string[] usingi)
        {
            if (solution.CurrentFile == null)
            {
                MessageBox.Show("Brak otwartego pliku");
                return;
            }
            foreach (var nazwaUsinga in usingi)
                solution
                    .CurenctDocument
                        .DodajUsingaJesliTrzeba(nazwaUsinga);
        }
    }
}