using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    [SpecyficzneDlaPincasso]
    class PozycjaDodawanieDaoDoContekstu : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaDodawanieDaoDoContekstu(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidDodajDaoDoContekstu;

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get { yield return WymaganieDostepnosci.PlikDao; }
        }

        public void Execute(object sender, EventArgs args)
        {
            new DodawanieDaoDaoContekstu(solution, solutionExplorer)
                .Dodaj();
        }
    }
}
