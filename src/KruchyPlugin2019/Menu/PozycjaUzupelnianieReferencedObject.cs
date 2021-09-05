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
    class PozycjaUzupelnianieReferencedObject : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaUzupelnianieReferencedObject(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.DomainObject;
            }
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijAtrybutKluczaObcego; }
        }

        public void Execute(object sender, EventArgs args)
        {
            new UzupelnianieReferencedObject(solution.AktualnyDokument).Uzupelnij();
        }
    }
}
