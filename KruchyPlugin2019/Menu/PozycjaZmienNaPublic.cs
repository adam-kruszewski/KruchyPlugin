using System;
using System.Collections.Generic;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCompany.KruchyPlugin1.Akcje;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class PozycjaZmienNaPublic : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaZmienNaPublic(ISolutionWrapper solution)        
        {
            this.solution = solution;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZmienNaPublic; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new ZmianaModyfikatoraMetody(solution.AktualnyDokument).ZmienNa("public");
        }
    }
}
