using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Field : ParsedUnit, IWithDocumentation, IWithOwner, IWithName
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsGeneric { get; set; }
        public IList<Modifier> Modifiers { get; private set; }

        public Documentation Documentation { get; set; }
        public DefinedItem Owner { get; set; }

        public Field()
        {
            Modifiers = new List<Modifier>();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} [{2}]", Name, TypeName, JoinModifiers());
        }

        private string JoinModifiers()
        {
            return string.Join(", ", Modifiers.Select(o => o.Name));
        }
    }
}