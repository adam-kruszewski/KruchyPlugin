using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaFolderContextTest : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaFolderContextTest(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID => PkgCmdIDList.TestContextMenuAction;

        public IEnumerable<WymaganieDostepnosci> Wymagania => Enumerable.Empty<WymaganieDostepnosci>();

        public void Execute(object sender, EventArgs args)
        {
            var folder = solutionExplorer.GetSelection().GetSingleSelectedFolder();
        }
    }
}
