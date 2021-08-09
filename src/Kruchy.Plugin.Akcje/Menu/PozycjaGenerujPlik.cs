using System;
using System.Collections.Generic;
using System.Linq;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaGenerujPlik : AbstractPozycjaMenuDynamicznieRozwijane
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public PozycjaGenerujPlik(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer) : base(solution)
        {
            this.solutionExplorer = solutionExplorer;
        }

        public override uint MenuCommandID => PkgCmdIDList.cmdidGenerujPlikStartCommand;

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        protected override IEnumerable<IPodpozycjaMenuDynamicznego> DajDostepnePozycje()
        {
            var konf = Konfiguracja.GetInstance(solution);

            var schematy = konf.SchematyGenerowania().ToList();

            for (uint i = 0; i < schematy.Count; i++)
                yield return new PozycjaSchematGenerowania(
                    solution,
                    solutionExplorer,
                    MenuCommandID + i,
                    schematy[(int)i]);
        }

        private class PozycjaSchematGenerowania : IPozycjaMenu, IPodpozycjaMenuDynamicznego
        {
            private readonly ISolutionWrapper solution;
            private readonly ISolutionExplorerWrapper solutionExplorer;
            private readonly uint menuCommandID;
            private readonly SchematGenerowania schemat;

            public PozycjaSchematGenerowania(
                ISolutionWrapper solution,
                ISolutionExplorerWrapper solutionExplorer,
                uint menuCommandID,
                SchematGenerowania schemat)
            {
                this.solution = solution;
                this.solutionExplorer = solutionExplorer;
                this.menuCommandID = menuCommandID;
                this.schemat = schemat;
            }

            public uint MenuCommandID => menuCommandID;

            public IEnumerable<WymaganieDostepnosci> Wymagania => new List<WymaganieDostepnosci>();

            public string DajOpis()
            {
                return schemat.TytulSchematu;
            }

            public void Execute(object sender, EventArgs args)
            {
                new GenerowaniePlikuZSzablonu(solutionExplorer, solution)
                    .Generuj(schemat.TytulSchematu);
            }
        }
    }
}
