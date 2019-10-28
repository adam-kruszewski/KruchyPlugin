﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieUprawnienDomyslnych : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaDodawanieUprawnienDomyslnych(ISolutionWrapper solution)
            : base(solution)
        {

        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajUprawnienieDomyslne; }
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Controller;
            }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new DodawanieUprawnienDomyslnych(solution).Dodaj();
        }
    }
}
