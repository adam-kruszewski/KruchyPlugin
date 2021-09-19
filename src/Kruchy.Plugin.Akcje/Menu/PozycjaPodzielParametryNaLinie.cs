using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaPodzielParametryNaLinie : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaPodzielParametryNaLinie(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidPodzielParametryNaLinie; }
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
            new PodzielParametryNaLinie(solution).Podziel();
        }
    }
}
