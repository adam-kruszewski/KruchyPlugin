using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaPrzejdzDo : AbstractPozycjaMenuDynamicznieRozwijane
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaPrzejdzDo(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public override uint MenuCommandID => PkgCmdIDList.cmdidMyDynamicStartCommand;

        public override IEnumerable<WymaganieDostepnosci> Wymagania =>
            new List<WymaganieDostepnosci>();

        protected override IEnumerable<IPodpozycjaMenuDynamicznego> DajDostepnePozycje()
        {
            var konf = Konfiguracja.GetInstance(solution);

            var pozycjePrzejdzDo = konf.PrzejdzDo().ToList();

            for (uint i = 0; i < pozycjePrzejdzDo.Count(); i++)
            {
                yield return new PozycjaPlikDoOtwarcia(
                    solution,
                    solutionExplorer,
                    MenuCommandID + i,
                    pozycjePrzejdzDo[(int)i].Sciezka);
            }
        }

        private class PozycjaPlikDoOtwarcia : IPozycjaMenu, IPodpozycjaMenuDynamicznego
        {
            uint menuCommandID;
            private readonly string sciezka;
            private readonly ISolutionWrapper solution;
            private readonly ISolutionExplorerWrapper solutionExplorer;

            public PozycjaPlikDoOtwarcia(
                ISolutionWrapper solution,
                ISolutionExplorerWrapper solutionExplorer,
                uint menuCommandID,
                string sciezka)
            {
                this.menuCommandID = menuCommandID;
                this.sciezka = sciezka;
                this.solution = solution;
                this.solutionExplorer = solutionExplorer;
            }

            public uint MenuCommandID => menuCommandID;

            public IEnumerable<WymaganieDostepnosci> Wymagania =>
                new List<WymaganieDostepnosci>();

            public void Execute(object sender, EventArgs args)
            {
                var sciezkaDoOtwarcia = sciezka;
                if (!sciezka.Contains(":"))
                {
                    sciezkaDoOtwarcia =
                        Path.Combine(
                            solution.Katalog,
                            sciezka);
                }

                if (File.Exists(sciezkaDoOtwarcia))
                    solutionExplorer.OtworzPlik(sciezkaDoOtwarcia);
                else
                    MessageBox.Show("Brak pliku: " + sciezkaDoOtwarcia);
            }

            public string DajOpis()
            {
                return sciezka;
            }
        }
    }
}
