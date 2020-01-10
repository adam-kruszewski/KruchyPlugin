using System.Collections.Generic;

namespace Kruchy.Plugin.Utils.Menu
{
    public interface IPozycjaMenuDynamicznieRozwijane : IPozycjaMenu
    {
        uint OstanieCommandID { get; }

        void WykonajPodakcje(int commandID);

        IEnumerable<PozycjaMenuRozwijanego> DajPozycje();

        bool DostepnaPodakcja(int commandID);
    }

    public class PozycjaMenuRozwijanego
    {
        public string Tekst { get; set; }

        public IPozycjaMenu PozycjaMenu { get; set; }
    }
}