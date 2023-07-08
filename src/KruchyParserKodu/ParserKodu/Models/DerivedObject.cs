using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class DerivedObject : ParsedUnit, IWithName
    {
        public string Name { get; set; }

        public List<string> ParameterTypeNames { get; private set; }

        public DerivedObject()
        {
            ParameterTypeNames = new List<string>();
        }
    }
}
