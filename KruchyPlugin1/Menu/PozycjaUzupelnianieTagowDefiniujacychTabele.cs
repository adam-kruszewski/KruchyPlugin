using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieTagowDefiniujacychTabele : PozycjaMenu
    {
        public PozycjaUzupelnianieTagowDefiniujacychTabele(
            SolutionWrapper solution)
            : base(solution)
        {
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.DomainObject;
            }
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijTagiDefiniujaceTabele; }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            var dokumentWrapper = solution.AktualnyDokument;
            new UzupelnianieTagowDefiniujacychTabele(dokumentWrapper).Uzupelnij();
        }
    }
}