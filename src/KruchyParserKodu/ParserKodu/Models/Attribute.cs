using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Attribute : ParsedUnit
    {
        public string Name { get; set; }

        public IList<AttributeParameter> Parameters;

        public Attribute()
        {
            Parameters = new List<AttributeParameter>();
        }
    }
}