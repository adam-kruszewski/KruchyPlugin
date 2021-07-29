using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;
using System;
using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaInicjalizowanieKlasyTestowejMockami : IPozycjaMenu
    {
        private readonly ISolutionWrapper solution;

        public PozycjaInicjalizowanieKlasyTestowejMockami(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidInicjujTestyMockami;

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.KlasaTestowa;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            new InicjalizowanieKlasyTestowejMockami(solution)
                .Inicjuj();
        }
    }
}