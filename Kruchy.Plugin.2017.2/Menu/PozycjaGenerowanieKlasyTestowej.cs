using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;
using KruchyCompany.KruchyPlugin1.Interfejs;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaGenerowanieKlasyTestowej : PozycjaMenu, IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaGenerowanieKlasyTestowej(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
            : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        protected override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZrobKlaseTestowa; }
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
            var dialog = new NazwaKlasyTestowForm(solution);
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.NazwaKlasy))
                return;

            new GenerowanieKlasyTestowej(solution, solutionExplorer)
                .Generuj(
                    dialog.NazwaKlasy,
                    dialog.Rodzaj,
                    dialog.InterfejsTestowany,
                    dialog.Integracyjny);
        }
    }
}
