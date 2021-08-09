using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruchy.Plugin.Akcje.Tests.Samples
{
    class UzupelnianieKonstruktoraPrzyNadklasie : Klasa1
    {
        private readonly string serwis1;
        private readonly string serwis2;

        public UzupelnianieKonstruktoraPrzyNadklasie(
            string serwis1,
            string serwis0) : base(serwis0)
        {
            this.serwis1 = serwis1;
        }
    }
}
