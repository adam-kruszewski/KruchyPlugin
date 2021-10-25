using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaDodajKonfiguracjeWlasciwosciPola : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodajKonfiguracjeWlasciwosciPola(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidDodajKonfiguracjePolaWlasciwosci;

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new DodajKonfiguracjeWlasciwosciPola(solution).Dodaj();
        }
    }
}
