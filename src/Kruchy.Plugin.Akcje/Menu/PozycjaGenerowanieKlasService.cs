using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaGenerowanieKlasService : IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public PozycjaGenerowanieKlasService(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZrobKlaseService; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.Modul;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            var dialog = new NazwaKlasyWindow(true);
            dialog.EtykietaNazwyPliku = "Nazwa pliku implementacji serwisu";
            dialog.EtykietaCheckBoxa = "Interfejs i implementacja w Impl";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.NazwaPliku))
                return;

            var g = new GenerowanieKlasService(solution, solutionExplorer);

            g.Generuj(solution.CurrentFile, dialog.NazwaPliku, dialog.StanCheckBoxa);
        }
    }
}
