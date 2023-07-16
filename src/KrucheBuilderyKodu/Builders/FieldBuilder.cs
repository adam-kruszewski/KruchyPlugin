using System.Collections.Generic;
using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class FieldBuilder
    {
        private string name;
        private string typeName;
        private IList<string> modifiers;

        public FieldBuilder()
        {
            modifiers = new List<string>();
        }

        public FieldBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public FieldBuilder WithTypeName(string typeName)
        {
            this.typeName = typeName;
            return this;
        }

        public FieldBuilder AddModifier(string modifier)
        {
            modifiers.Add(modifier);
            return this;
        }
        
        public string Build(string wciecie = "")
        {
            var builder = new StringBuilder();
            builder.Append(wciecie);
            builder.Append(string.Join(" ", modifiers));
            builder.Append(" ");
            builder.Append(typeName);
            builder.Append(" ");
            builder.Append(name);
            builder.AppendLine(";");
            return builder.ToString();
        }
    }
}
