﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

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
