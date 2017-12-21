using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieKlasService : PozycjaMenu
    {
        public PozycjaGenerowanieKlasService(SolutionWrapper solution)
            : base(solution)
        {
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZrobKlaseService; }
        }

        protected override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.Modul;
            }
        }

        protected override void Execute(object sender, EventArgs args)
        {
            var dialog = new NazwaKlasyWindow(true);
            dialog.EtykietaNazwyPliku = "Nazwa pliku implementacji serwisu";
            dialog.EtykietaCheckBoxa = "Interfejs i implementacja w Impl";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.NazwaPliku))
                return;

            var g = new GenerowanieKlasService(solution);

            g.Generuj(solution.AktualnyPlik, dialog.NazwaPliku, dialog.StanCheckBoxa);
        }
    }
}
