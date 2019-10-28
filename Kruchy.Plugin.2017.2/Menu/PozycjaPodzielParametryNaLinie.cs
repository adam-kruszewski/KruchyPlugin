using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaPodzielParametryNaLinie : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaPodzielParametryNaLinie(ISolutionWrapper solution)
            : base(solution)
        {

        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidPodzielParametryNaLinie; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new PodzielParametryNaLinie(solution).Podziel();
        }
    }
}
