using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaGenerowanieKlasyTestowej : IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public PozycjaGenerowanieKlasyTestowej(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZrobKlaseTestowa; }
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
            var dialog = new NazwaKlasyTestowForm(solution);
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.NazwaKlasy))
                return;

            new GenerowanieKlasyTestowej(solution, solutionExplorer)
                .Generuj(
                    dialog.NazwaKlasy,
                    dialog.Rodzaj,
                    dialog.InterfejsTestowany,
                    dialog.Integracyjny);
        }
    }
}
