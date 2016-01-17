using System;
using System.Collections.Generic;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Interfejs;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieWidoku : PozycjaMenu
    {
        public PozycjaGenerowanieWidoku(SolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidGenerujWidok; }
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
            var dialog = new NazwaKlasyWindow(false);
            dialog.EtykietaNazwyPliku = "Nazwa widoku";
            dialog.InicjalnaWartosc = solution.NazwaAktualnejMetody();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
            {
                new GenerowanieWidoku(solution).Generuj(dialog.NazwaPliku);
            }
        }
    }
}
