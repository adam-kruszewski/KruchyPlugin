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
    class PozycjaDodawanieUprawnienDomyslnych : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieUprawnienDomyslnych(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajUprawnienieDomyslne; }
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
            new DodawanieUprawnienDomyslnych(solution).Dodaj();
        }
    }
}
