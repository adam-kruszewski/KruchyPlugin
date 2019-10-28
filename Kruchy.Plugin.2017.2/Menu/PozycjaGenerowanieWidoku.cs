﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieWidoku : PozycjaMenu, IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaGenerowanieWidoku(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
            : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidGenerujWidok; }
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
            var dialog = new NazwaKlasyWindow(false);
            dialog.EtykietaNazwyPliku = "Nazwa widoku";
            dialog.InicjalnaWartosc = solution.NazwaAktualnejMetody();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
            {
                new GenerowanieWidoku(solution, solutionExplorer).Generuj(dialog.NazwaPliku);
            }
        }
    }
}
