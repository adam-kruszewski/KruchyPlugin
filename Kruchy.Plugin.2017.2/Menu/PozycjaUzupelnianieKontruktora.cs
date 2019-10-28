using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieKontruktora : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaUzupelnianieKontruktora(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijKontruktor; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Klasa;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new UzupelnianieKontruktora(solution).Uzupelnij();
        }
    }
}
