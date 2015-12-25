using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    class Metoda : ParsowanaJednostka
    {
        public IList<Parametr> Parametry { get; private set; }
        public IList<string> Modyfikatory { get; set; }
        public string TypZwracany { get; set; }
        public string Nazwa { get; set; }

        public Metoda()
        {
            Parametry = new List<Parametr>();
            Modyfikatory = new List<string>();
        }
    }
}
