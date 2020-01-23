using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Akcje.Generowanie.Buildera.Komponenty;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaGenerowanieBuildera : IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public PozycjaGenerowanieBuildera(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solutionExplorer = solutionExplorer;
            this.solution = solution;
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidGenerujBuildera;

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.Klasa;
            }
        }


        public void Execute(object sender, EventArgs args)
        {
            var oknoParametrow = new NazwaKlasyWindow();
            oknoParametrow.EtykietaNazwyPliku = "Interfejs klasy serwisu";

            oknoParametrow.ShowDialog();
            if (string.IsNullOrEmpty(oknoParametrow.NazwaPliku))
                return;

            new GenerowanieBuildera(solution, solutionExplorer)
                .Generuj(new Parametry(oknoParametrow.NazwaPliku));
        }

        private class Parametry : IParametryGenerowaniaBuildera
        {
            public Parametry(string nazwaInterfejsu)
            {
                NazwaInterfejsuService = nazwaInterfejsu;
            }

            public string NazwaInterfejsuService { get; private set; }
        }
    }
}
