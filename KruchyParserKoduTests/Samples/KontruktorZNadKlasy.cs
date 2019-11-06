using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyParserKoduTests.Samples
{
    class KontruktorZNadKlasy : InnaKlasa
    {
        private readonly string serwis1;
        private readonly string serwis2;

        public KontruktorZNadKlasy(
            string serwis1,
            string serwis0) : base(serwis0)
        {
            this.serwis1 = serwis1;
        }
    }
}