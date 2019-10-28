using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieUsingDbContext : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaDodawanieUsingDbContext(ISolutionWrapper solution)
            : base(solution)
        {
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajUsingDbContext; }
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
            new DodawanieUsingDbContext(solution).Dodaj();
        }
    }
}