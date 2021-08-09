
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class ObiektDziedziczony : ParsowanaJednostka
    {
        public string Nazwa { get; set; }

        public List<string> NazwyTypowParametrow { get; private set; }

        public ObiektDziedziczony()
        {
            NazwyTypowParametrow = new List<string>();
        }
    }
}
