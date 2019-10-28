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

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidWstawDoSchowkaNazweControllera; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new WstawianieNazwyControlleraDoSchowka(solution).Wstaw();
        }
    }
}
