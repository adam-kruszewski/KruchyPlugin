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
    class PozycjaUzupelnianieTagowDefiniujacychTabele : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaUzupelnianieTagowDefiniujacychTabele(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.DomainObject;
            }
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijTagiDefiniujaceTabele; }
        }

        public void Execute(object sender, EventArgs args)
        {
            var dokumentWrapper = solution.AktualnyDokument;
            new UzupelnianieTagowDefiniujacychTabele(dokumentWrapper).Uzupelnij();
        }
    }
}