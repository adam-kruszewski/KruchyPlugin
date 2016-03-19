using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    [Testowo]
    class KlasaDoParsowania
    {
        private readonly IParser PoleReadOnly;
        public readonly IList<string> PoleStringReadOnly;

        public int Wlasciwosc { get; set; }
        public int Wlasciwosc2 { get { return 1; } }

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