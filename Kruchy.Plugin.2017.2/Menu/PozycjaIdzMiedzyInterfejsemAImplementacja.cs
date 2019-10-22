using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzMiedzyInterfejsemAImplementacja : PozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzMiedzyInterfejsemAImplementacja(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer) : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoImplementacji; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new IdzMiedzyInterfejsemAImplementacja(solution, solutionExplorer).Przejdz();
        }
    }
}
