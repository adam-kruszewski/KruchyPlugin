using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class AttributeBuilder : ICodeBuilder
    {
        private string Name { get; set; }

        private IList<string> Parameters { get; set; }
        private IList<AttributeBuilder> NextAttributes { get; set; }
        private bool Brackets { get; set; }

        public AttributeBuilder()
        {
            Parameters = new List<string>();
            NextAttributes = new List<AttributeBuilder>();
            Brackets = true;
        }

        public AttributeBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public AttributeBuilder AddParameterValue(string value)
        {
            Parameters.Add("\"" + value + "\"");
            return this;
        }

        public AttributeBuilder AddNonStringParameterValue(string text)
        {
            Parameters.Add(text);
            return this;
        }

        public AttributeBuilder AddNextAttribute(AttributeBuilder builder)
        {
            NextAttributes.Add(builder);
            return this;
        }

        public AttributeBuilder WithoutBrackets()
        {
            Brackets = false;
            return this;
        }

        public string Build(string indent = "")
        {
            return Build(false, indent);
        }

        public string Build(bool withoutLineEnd, string indent = "")
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append(indent);
            if (Brackets)
                outputBuilder.Append("[");
            outputBuilder.Append(Name);

            if (Parameters.Any())
            {
                outputBuilder.Append("(");
                outputBuilder.Append(string.Join(", ", Parameters));
                outputBuilder.Append(")");
            }
            foreach (var nextAttribute in NextAttributes)
            {
                outputBuilder.Append(", ");
                outputBuilder.Append(nextAttribute.WithoutBrackets().Build());
            }

            if (Brackets)
            {
                outputBuilder.Append("]");
                if (!withoutLineEnd)
                    outputBuilder.AppendLine();
            }

            return outputBuilder.ToString();
        }
    }
}
