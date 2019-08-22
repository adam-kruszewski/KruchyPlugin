﻿using System;
using System.Collections.Generic;
using EnvDTE80;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieReferencedObject : PozycjaMenu
    {
        public PozycjaUzupelnianieReferencedObject(
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
            get { return PkgCmdIDList.cmdidUzupelnijAtrybutKluczaObcego; }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            new UzupelnianieReferencedObject(solution.DTE).Uzupelnij();
        }
    }
}
