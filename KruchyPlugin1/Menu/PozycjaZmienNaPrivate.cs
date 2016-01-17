using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaZmienNaPrivate : PozycjaMenu
    {
        public PozycjaZmienNaPrivate(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZmienNaPrivate; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new ZmianaModyfikatoraMetody(solution.AktualnyDokument).ZmienNa("private");
        }
    }
}
