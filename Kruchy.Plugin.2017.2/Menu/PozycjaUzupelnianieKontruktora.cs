using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieKontruktora : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaUzupelnianieKontruktora(ISolutionWrapper solution)
            : base(solution)
        {

        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijKontruktor; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Klasa;
            }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new UzupelnianieKontruktora(solution).Uzupelnij();
        }
    }
}
