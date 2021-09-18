using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Pincasso.Akcje.Menu
{
    [SpecyficzneDlaPincasso]
    class PozycjaIdzDoGridRowActions : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzDoGridRowActions(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PincassoPkgCmdIDList.cmidPrzejdzDoGridRowActions; }
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
            new IdzDoPlikuWidoku(solution, solutionExplorer)
                .PrzejdzLubStworz("GridRowActions.cshtml");
        }
    }
}
