using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public uint ID { get; set; }

        public string Tekst { get; set; }
    }
}