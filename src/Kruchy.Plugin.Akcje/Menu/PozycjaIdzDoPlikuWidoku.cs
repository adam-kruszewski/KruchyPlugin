using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaIdzDoPlikuWidoku : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzDoPlikuWidoku(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoWidoku; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new IdzDoPlikuWidoku(solution, solutionExplorer).PrzejdzDoWidokuDlaAktualnejMetody();
        }
    }
}