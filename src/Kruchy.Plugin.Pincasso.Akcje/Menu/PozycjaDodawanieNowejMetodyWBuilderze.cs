using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.Menu;
using Kruchy.Plugin.Pincasso.Akcje.Akcje;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Pincasso.Akcje.Menu
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
            get { return PincassoPkgCmdIDList.cmdidGenerujMetodeWBuilderze; }
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
