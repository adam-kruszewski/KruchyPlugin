using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KruchyCompany.KruchyPlugin1.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    [Testowo]
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

        private static void MetodaStatyczna(string b, int? a, DateTime? d)
        {

        }

        [Testowo]
        private int MetodaZwykla(System.DateTime d, System.DateTime? d1)
        {
            return 0;
        }
    }
}