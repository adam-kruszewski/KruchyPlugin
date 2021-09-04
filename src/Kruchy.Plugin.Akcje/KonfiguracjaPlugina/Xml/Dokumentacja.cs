using System.Collections.Generic;

namespace Kruchy.Plugin.Akcje.KonfiguracjaPlugina.Xml
{
    public class Dokumentacja
    {
        public int Jezyk { get; set; }

        public List<Czasownik> Czasowniki { get; set; }

        public List<WlasciwoscPole> WlasciwosciPola { get; set; }

        public Dokumentacja()
        {
            Jezyk = 2;
            Czasowniki = new List<Czasownik>();
            WlasciwosciPola = new List<WlasciwoscPole>();
        }
    }
}