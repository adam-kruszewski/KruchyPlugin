﻿using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieKlasyWalidatora : PozycjaMenu
    {
        public PozycjaGenerowanieKlasyWalidatora(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajKlaseWalidatora; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.Modul;
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            var nazwaPlikuDoWalidacji =
                solution.AktualnyPlik.NazwaBezRozszerzenia;
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa klasy implementacji walidatora";
            dialog.InicjalnaWartosc = nazwaPlikuDoWalidacji + "Validator";
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
                new GenerowanieKlasyWalidatora(solution)
                    .Generuj(dialog.NazwaPliku);
        }
    }
}
