using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieUsingDbContext : PozycjaMenu
    {
        public PozycjaDodawanieUsingDbContext(SolutionWrapper solution)
            : base(solution)
        {
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajUsingDbContext; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Klasa;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new DodawanieUsingDbContext(solution).Dodaj();
        }
    }
}