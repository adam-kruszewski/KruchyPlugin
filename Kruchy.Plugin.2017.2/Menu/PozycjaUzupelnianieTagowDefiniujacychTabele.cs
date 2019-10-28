using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieTagowDefiniujacychTabele : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaUzupelnianieTagowDefiniujacychTabele(
            ISolutionWrapper solution)
            : base(solution)
        {
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.DomainObject;
            }
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijTagiDefiniujaceTabele; }
        }

        public override void Execute(object sender, EventArgs args)
        {
            var dokumentWrapper = solution.AktualnyDokument;
            new UzupelnianieTagowDefiniujacychTabele(dokumentWrapper).Uzupelnij();
        }
    }
}