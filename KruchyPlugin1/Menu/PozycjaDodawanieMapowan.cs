using System;
using System.Collections.Generic;
using System.Windows;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieMapowan : PozycjaMenu
    {
        public PozycjaDodawanieMapowan(SolutionWrapper solution)
            : base(solution)
        {
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajMapowania; }
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
            new DodawanieMapowan(solution).Generuj();
        }
    }
}
