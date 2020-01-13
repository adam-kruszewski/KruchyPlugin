using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    class PozycjaPrzejdzDo : AbstractPozycjaMenuDynamicznieRozwijane
    {
        public PozycjaPrzejdzDo(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
        }

        public override uint MenuCommandID => PkgCmdIDList.cmdidMyDynamicStartCommand;

        public override IEnumerable<WymaganieDostepnosci> Wymagania =>
            new List<WymaganieDostepnosci>();

        private static string[] miasta =
        {
            "Warszawa",
            "Kraków",
            "Wrocław",
            "Białystok",
            "Gdańsk",
            "Szczecin"
        };

        protected override IEnumerable<IPodpozycjaMenuDynamicznego> DajDostepnePozycje()
        {
            for (uint i = 0; i < miasta.Length; i++)
                yield return new PozycjaMiasto(MenuCommandID + i, miasta[i]);
        }

        private class PozycjaMiasto : IPozycjaMenu, IPodpozycjaMenuDynamicznego
        {
            uint menuCommandID;
            private readonly string miasto;

            public PozycjaMiasto(uint menuCommandID, string miasto)
            {
                this.menuCommandID = menuCommandID;
                this.miasto = miasto;
            }

            public uint MenuCommandID => menuCommandID;

            public IEnumerable<WymaganieDostepnosci> Wymagania =>
                new List<WymaganieDostepnosci>();

            public void Execute(object sender, EventArgs args)
            {
                MessageBox.Show("Miasto: " + miasto);
            }

            public string DajOpis()
            {
                return miasto;
            }
        }
    }
}
