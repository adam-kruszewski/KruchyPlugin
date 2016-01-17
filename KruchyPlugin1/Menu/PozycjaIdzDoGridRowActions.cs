using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzDoGridRowActions : PozycjaMenu
    {
        public PozycjaIdzDoGridRowActions(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmidPrzejdzDoGridRowActions; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new IdzDoPlikuWidoku(solution)
                .PrzejdzLubStworz("GridRowActions.cshtml");
        }
    }
}
