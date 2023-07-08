using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Parameter
    {
        public string TypeName { get; set; }
        public string ParameterName { get; set; }

        public bool WithThis { get; set; }
        public string DefaultValue { get; set; }

        public bool WithRef { get; set; }
        public bool WithOut { get; set; }
        public bool WithParams { get; set; }

        public string Modifier { get; set; }

        public List<Attribute> Attributes { get; private set; }

        public Parameter()
        {
            Attributes = new List<Attribute>();
        }
    }
}