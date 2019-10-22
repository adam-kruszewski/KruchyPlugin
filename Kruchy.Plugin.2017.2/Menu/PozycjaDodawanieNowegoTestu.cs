using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieNowegoTestu : PozycjaMenu
    {
        public PozycjaDodawanieNowegoTestu(ISolutionWrapper solution)
            : base(solution)
        {

        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajNowyTest; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.KlasaTestowa;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            var dialog = new NazwaKlasyWindow();
            dialog.EtykietaNazwyPliku = "Nazwa metody testu";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.NazwaPliku))
                return;

            new DodawanieNowegoTestu(solution)
                .DodajNowyTest(dialog.NazwaPliku);
        }
    }
}