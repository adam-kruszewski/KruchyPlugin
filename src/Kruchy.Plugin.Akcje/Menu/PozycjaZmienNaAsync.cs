using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaZmienNaAsync : IPozycjaMenu
    {
        public uint MenuCommandID => PkgCmdIDList.cmdidZrobMetodeAsync;

        private readonly ISolutionWrapper solution;

        public PozycjaZmienNaAsync(
            ISolutionWrapper solution)
        {
            this.solution = solution;
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
            new ZmianaNaAsync(solution.CurentDocument).ZamienNaAsyncMethod();
        }
    }
}
