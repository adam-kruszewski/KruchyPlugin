using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class TestowaAkcjaWPodmenu : IPozycjaMenuDynamicznieRozwijane
    {
        private IList<PozycjaMenuRozwijanego> pozycjeRozwijane;

        public TestowaAkcjaWPodmenu(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            pozycjeRozwijane = new List<PozycjaMenuRozwijanego>();
        }

        public uint MenuCommandID => PkgCmdIDList.cmdidMyDynamicStartCommand;

        public IEnumerable<WymaganieDostepnosci> Wymagania => new List<WymaganieDostepnosci>();

        public uint OstanieCommandID => MenuCommandID + (uint)miasta.Length - 1;

        public IEnumerable<PozycjaMenuRozwijanego> DajPozycje()
        {
            pozycjeRozwijane.Clear();

            for (uint i = 0; i < miasta.Length; i++)
                pozycjeRozwijane.Add(new PozycjaMenuRozwijanego
                {
                    Tekst = miasta[i],
                    PozycjaMenu = new PozycjaMiasto(MenuCommandID + i, miasta[i])
                });

            return pozycjeRozwijane;
        }

        public void Execute(object sender, EventArgs args)
        {
        }

        private static string[] miasta =
        {
            "Warszawa",
            "Kraków",
            "Wrocław",
            "Białystok",
            "Gdańsk",
            "Szczecin"
        };

        public void WykonajPodakcje(int commandID)
        {
            MessageBox.Show("Wykonanie podakcji commandID: " + commandID);

            pozycjeRozwijane
                .SingleOrDefault(o => o.PozycjaMenu.MenuCommandID == commandID)
                    ?.PozycjaMenu.Execute(null, null);
        }

        public bool DostepnaPodakcja(int commandID)
        {
            if (commandID == MenuCommandID + 2)
                return false;
            else
                return true;
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
        }
    }
}
