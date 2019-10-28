using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieMapowan : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaDodawanieMapowan(ISolutionWrapper solution)
            : base(solution)
        {
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajMapowania; }
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
            new DodawanieMapowan(solution).Generuj();
        }
    }
}
