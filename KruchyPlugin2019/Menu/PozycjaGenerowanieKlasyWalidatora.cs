﻿using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Atrybuty;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    [SpecyficzneDlaPincasso]
    class PozycjaGenerowanieKlasyWalidatora : IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public PozycjaGenerowanieKlasyWalidatora(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajKlaseWalidatora; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.Modul;
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            var nazwaPlikuDoWalidacji =
                solution.AktualnyPlik.NazwaBezRozszerzenia;
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa klasy implementacji walidatora";
            dialog.InicjalnaWartosc = nazwaPlikuDoWalidacji + "Validator";
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
                new GenerowanieKlasyWalidatora(solution, solutionExplorer)
                    .Generuj(dialog.NazwaPliku);
        }
    }
}
