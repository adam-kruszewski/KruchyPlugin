﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaZmienNaPrivate : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaZmienNaPrivate(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZmienNaPrivate; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new ZmianaModyfikatoraMetody(solution.AktualnyDokument).ZmienNa("private");
        }
    }
}