﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaIdzDoPlikuWidoku : PozycjaMenu
    {
        public PozycjaIdzDoPlikuWidoku(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidIdzDoWidoku; }
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
            new IdzDoPlikuWidoku(solution).PrzejdzDoWidokuDlaAktualnejMetody();
        }
    }
}
