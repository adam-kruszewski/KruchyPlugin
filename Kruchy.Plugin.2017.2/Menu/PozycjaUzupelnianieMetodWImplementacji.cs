using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieMetodWImplementacji : PozycjaMenu, IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaUzupelnianieMetodWImplementacji(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
            : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijMetodaWImplementacji; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Interfejs;
            }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new UzupelnianieMetodWImplementacji(solution, solutionExplorer).Uzupelnij();
        }
    }
}
