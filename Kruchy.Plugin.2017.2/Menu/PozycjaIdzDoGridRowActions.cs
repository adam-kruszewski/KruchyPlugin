using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzDoGridRowActions : PozycjaMenu, IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzDoGridRowActions(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
            : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmidPrzejdzDoGridRowActions; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new IdzDoPlikuWidoku(solution, solutionExplorer)
                .PrzejdzLubStworz("GridRowActions.cshtml");
        }
    }
}
