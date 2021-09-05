using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using System;
using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    [SpecyficzneDlaPincasso]
    class PozycjaDodawanieUsingDbContext : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieUsingDbContext(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajUsingDbContext; }
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
            new DodawanieUsingDbContext(solution).Dodaj();
        }
    }
}