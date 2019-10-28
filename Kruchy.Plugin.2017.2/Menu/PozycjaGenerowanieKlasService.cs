using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieKlasService : IPozycjaMenu
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

            g.Generuj(solution.AktualnyPlik, dialog.NazwaPliku, dialog.StanCheckBoxa);
        }
    }
}
