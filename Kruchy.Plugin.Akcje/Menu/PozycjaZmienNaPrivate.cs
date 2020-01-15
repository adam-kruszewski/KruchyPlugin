using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaZmienNaPrivate : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaZmienNaPrivate(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZmienNaPrivate; }
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
            new ZmianaModyfikatoraMetody(solution.AktualnyDokument).ZmienNa("private");
        }
    }
}
