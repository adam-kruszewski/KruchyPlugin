using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class InterfaceBuilder : ICodeBuilder
    {
        private string modifier { get; set; }
        private string name { get; set; }
        private string superClassName { get; set; }

        public InterfaceBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public InterfaceBuilder WithModifier(string modifier)
        {
            this.modifier = modifier;
            return this;
        }

        public InterfaceBuilder WithSuperClass(string name)
        {
            superClassName = name;
            return this;
        }

        public string Build(string indent = "")
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.Append(indent);
            if (!string.IsNullOrEmpty(modifier))
                outputBuilder.Append(modifier + " ");
            outputBuilder.Append("interface ");
            outputBuilder.Append(name);
            if (!string.IsNullOrEmpty(superClassName))
                outputBuilder.Append(" : " + superClassName);
            outputBuilder.AppendLine();
            outputBuilder.AppendLine(indent + "{");

            outputBuilder.AppendLine(indent + "}");
            return outputBuilder.ToString();

        }
    }
}
