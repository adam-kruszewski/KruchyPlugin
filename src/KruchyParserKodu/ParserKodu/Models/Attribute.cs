using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Attribute : ParsowanaJednostka
    {
        public string Name { get; set; }

        public IList<ParametrAtrybutu> Parameters;

        public Attribute()
        {
            Parameters = new List<ParametrAtrybutu>();
        }
    }
}