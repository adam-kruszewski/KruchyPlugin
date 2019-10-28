using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaWstawianieNazwyControlleraDoSchowka : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaWstawianieNazwyControlleraDoSchowka(
            ISolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidWstawDoSchowkaNazweControllera; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new WstawianieNazwyControlleraDoSchowka(solution).Wstaw();
        }
    }
}
