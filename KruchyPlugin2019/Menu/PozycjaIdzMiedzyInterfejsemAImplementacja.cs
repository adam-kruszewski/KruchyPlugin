using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzMiedzyInterfejsemAImplementacja : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaIdzMiedzyInterfejsemAImplementacja(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoImplementacji; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new IdzMiedzyInterfejsemAImplementacja(solution, solutionExplorer).Przejdz();
        }
    }
}
