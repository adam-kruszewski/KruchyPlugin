using Kruchy.Plugin.Pincasso.Akcje.Akcje;
using Kruchy.Plugin.Pincasso.Akcje.Atrybuty;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Pincasso.Akcje.Menu
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
            get { return PincassoPkgCmdIDList.cmdidUzupelnijAtrybutKluczaObcego; }
        }

        public void Execute(object sender, EventArgs args)
        {
            new UzupelnianieReferencedObject(solution.CurentDocument).Uzupelnij();
        }
    }
}
