using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaUzupelnianieReferencedObject : PozycjaMenu, IPozycjaMenu
    {
        public PozycjaUzupelnianieReferencedObject(
            ISolutionWrapper solution)
            : base(solution)
        {
        }

        public override IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.DomainObject;
            }
        }

        public override uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidUzupelnijAtrybutKluczaObcego; }
        }

        public override void Execute(object sender, EventArgs args)
        {
            new UzupelnianieReferencedObject(solution.AktualnyDokument).Uzupelnij();
        }
    }
}
