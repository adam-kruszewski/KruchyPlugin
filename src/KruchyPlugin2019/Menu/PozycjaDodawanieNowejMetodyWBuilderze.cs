using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using System;
using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    [SpecyficzneDlaPincasso]
    class PozycjaDodawanieNowejMetodyWBuilderze : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaDodawanieNowejMetodyWBuilderze(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidGenerujMetodeWBuilderze; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Builder;
            }
        }

        public void Execute(object sender, EventArgs args)
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
