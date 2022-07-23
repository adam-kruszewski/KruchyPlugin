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

        public PozycjaFolderContextTest(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID => PkgCmdIDList.TestContextMenuAction;

        public IEnumerable<WymaganieDostepnosci> Wymagania => Enumerable.Empty<WymaganieDostepnosci>();

        public void Execute(object sender, EventArgs args)
        {
            
        }
    }
}
