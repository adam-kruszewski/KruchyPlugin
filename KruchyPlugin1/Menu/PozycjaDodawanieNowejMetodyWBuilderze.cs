using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieNowejMetodyWBuilderze : PozycjaMenu
    {
        public PozycjaDodawanieNowejMetodyWBuilderze(SolutionWrapper solution)
            : base(solution)
        {

        }
        
        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidGenerujMetodeWBuilderze; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Builder;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa metody";
            dialog.InicjalnaWartosc = "Z";
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.NazwaPliku))
                new DodawanieNowejMetodyWBuilderze(solution)
                    .Dodaj(dialog.NazwaPliku);
        }
    }
}
