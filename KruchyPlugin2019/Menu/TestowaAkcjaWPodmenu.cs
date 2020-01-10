using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Menu
{
    class TestowaAkcjaWPodmenu : IPozycjaMenuDynamicznieRozwijane
    {
        public TestowaAkcjaWPodmenu(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {

        }

        public uint MenuCommandID => PkgCmdIDList.cmdidMyDynamicStartCommand;

        public IEnumerable<WymaganieDostepnosci> Wymagania => new List<WymaganieDostepnosci>();

        public uint OstanieCommandID => MenuCommandID + (uint)miasta.Length - 1;

        public IEnumerable<PozycjaMenuRozwijanego> DajPozycje()
        {
            for (uint i = 0; i < miasta.Length; i++)
                yield return new PozycjaMenuRozwijanego
                {
                    ID = MenuCommandID + i,
                    Tekst = miasta[i]
                };
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
        }

        public bool DostepnaPodakcja(int commandID)
        {
            if (commandID == MenuCommandID + 2)
                return false;
            else
                return true;
        }
    }
}
