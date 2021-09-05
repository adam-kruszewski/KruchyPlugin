using Kruchy.Plugin.Pincasso.Akcje.Akcje;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Pincasso.Akcje.Menu
{
    [SpecyficzneDlaPincasso]
    public class PozycjaUzupelnianieTagowDefiniujacychTabele
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
            get { return PincassoPkgCmdIDList.cmdidUzupelnijTagiDefiniujaceTabele; }
        }

        public void Execute(object sender, EventArgs args)
        {
            var dokumentWrapper = solution.AktualnyDokument;
            new UzupelnianieTagowDefiniujacychTabele(dokumentWrapper).Uzupelnij();
        }
    }
}
