using KruchyParserKodu.ParserKodu.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyParserKodu.ParserKodu.Models
{
    public class Property : ParsedUnit, IWithDocumentation, IWithOwner
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public IList<Modifier> Modifiers { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public bool HasGet { get; set; }
        public bool HasSet { get; set; }
        public Documentation Documentation { get; set; }
        public DefinedItem Owner { get; set; }

        public Property()
        {
            Modifiers = new List<Modifier>();
            Attributes = new List<Attribute>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Name);

            builder.Append(" {");
            builder.Append(TypeName);
            builder.Append("}");

            builder.Append(" [");
            builder.Append(string.Join(", " ,Modifiers.Select(o => o.Name)));
            builder.Append("]");

            return builder.ToString();
        }
    }
}