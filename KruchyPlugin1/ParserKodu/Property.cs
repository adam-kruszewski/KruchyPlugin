using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    class Property : ParsowanaJednostka
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public IList<string> Modyfikatory { get; private set; }

        public Property()
        {
            Modyfikatory = new List<string>();
        }
    }
}