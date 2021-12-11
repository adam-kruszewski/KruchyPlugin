using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaIdzDoKataloguControllera : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzDoKataloguControllera(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoKataloguControllera; }
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
            new IdzDoKataloguControllera(solution, solutionExplorer).Przejdz();
        }
    }
}
