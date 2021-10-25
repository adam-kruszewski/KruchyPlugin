using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaDodajKonfiguracjeCzasownika : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodajKonfiguracjeCzasownika(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidDodajKonfiguracjeCzasownika;

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new DodajKonfiguracjeCzasownika(solution).Dodaj();
        }
    }
}
