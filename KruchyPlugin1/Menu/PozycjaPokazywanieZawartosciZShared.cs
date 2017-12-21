﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaPokazywanieZawartosciZShared : PozycjaMenu
    {
        public PozycjaPokazywanieZawartosciZShared(SolutionWrapper solution)
            : base(solution)
        {
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidPokazZawartoscZShared; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.WidokCshtml;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new PokazywaniaZawartosciZShared(solution).Pokaz();
        }
    }
}