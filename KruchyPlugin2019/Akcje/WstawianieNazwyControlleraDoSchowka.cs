using System.IO;
using System.Windows;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class WstawianieNazwyControlleraDoSchowka
    {
        private readonly ISolutionWrapper solution;

        public WstawianieNazwyControlleraDoSchowka(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Wstaw()
        {
            if (solution.CzyPlikControllera())
            {
                var nazwaControllera =
                    solution
                        .AktualnyPlik
                            .DajNazweControllera();

                Clipboard.SetText(nazwaControllera);
                return;
            }
            else
            {
                if (solution.AktualnyPlik == null)
                    return;
                var fi = new FileInfo(solution.AktualnyPlik.SciezkaPelna);
                if (fi.Extension.ToLower() == ".cshtml")
                    Clipboard.SetText(fi.DirectoryName);
            }
        }

    }
}