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
    public class PozycjaGenerowanieKlasyWalidatora : IPozycjaMenu
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
            get { return PincassoPkgCmdIDList.cmdidDodajKlaseWalidatora; }
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
