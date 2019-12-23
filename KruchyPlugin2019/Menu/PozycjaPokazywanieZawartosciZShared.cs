using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaPokazywanieZawartosciZShared : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaPokazywanieZawartosciZShared(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidPokazZawartoscZShared; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.WidokCshtml;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new PokazywaniaZawartosciZShared(solution, solutionExplorer).Pokaz();
        }
    }
}