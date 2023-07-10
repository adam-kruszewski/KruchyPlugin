using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using System.IO;
using System.Windows.Forms;

namespace Kruchy.Plugin.Akcje.Akcje
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
                        .CurrentFile
                            .DajNazweControllera();

                System.Windows.Forms.Clipboard.SetText(nazwaControllera);
                return;
            }
            else
            {
                if (solution.CurrentFile == null)
                    return;
                var fi = new FileInfo(solution.CurrentFile.FullPath);
                if (fi.Extension.ToLower() == ".cshtml")
                    Clipboard.SetText(fi.DirectoryName);
            }
        }
    }
}
