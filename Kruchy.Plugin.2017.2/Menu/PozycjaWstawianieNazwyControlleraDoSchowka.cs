using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaWstawianieNazwyControlleraDoSchowka : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaWstawianieNazwyControlleraDoSchowka(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidWstawDoSchowkaNazweControllera; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new WstawianieNazwyControlleraDoSchowka(solution).Wstaw();
        }
    }
}
