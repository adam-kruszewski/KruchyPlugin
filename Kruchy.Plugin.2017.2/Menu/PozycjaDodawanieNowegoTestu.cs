using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaDodawanieNowegoTestu : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieNowegoTestu(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidDodajNowyTest; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.KlasaTestowa;
            }
        }

        public void Execute(object sender, EventArgs args)
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