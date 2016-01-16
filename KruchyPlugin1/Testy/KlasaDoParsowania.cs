using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KruchyCompany.KruchyPlugin1.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    class KlasaDoParsowania
    {
        private readonly IParser PoleReadOnly;
        public readonly IList<string> PoleStringReadOnly;

        public int Wlasciwosc { get; set; }

        public KlasaDoParsowania(int a)
        {
            PoleReadOnly = null;
            Console.WriteLine(PoleReadOnly);
            PoleStringReadOnly = null;
        }

        public KlasaDoParsowania()
        {

        }

        private static void MetodaStatyczna(string b)
        {

        }

        private int MetodaZwykla()
        {
            return 0;
        }
    }
}