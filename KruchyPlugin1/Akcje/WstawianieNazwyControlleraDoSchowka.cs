using System.IO;
using System.Windows;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Extensions;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class WstawianieNazwyControlleraDoSchowka
    {
        private readonly SolutionWrapper solution;

        public WstawianieNazwyControlleraDoSchowka(
            SolutionWrapper solution)
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