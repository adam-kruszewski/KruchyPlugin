using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class MethodBuilder : ICodeBuilder
    {
        private IList<string> modifiers;
        private string name;
        private string returnType;
        private IList<KeyValuePair<string, string>> parameters;
        private IList<ICodeBuilder> attributes;
        private IList<string> lines;
        private bool parameterInSingleLine = false;
        private bool extensionMethod = false;
        private string constructorInitialization = null;
        private IEnumerable<string> constructorInitializationParameters = null;

        public MethodBuilder()
        {
            returnType = "void";
            modifiers = new List<string>();
            parameters = new List<KeyValuePair<string, string>>();
            attributes = new List<ICodeBuilder>();
            lines = new List<string>();
        }

        public MethodBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public MethodBuilder WithReturnType(string type)
        {
            returnType = type;
            return this;
        }

        public MethodBuilder AddModifier(string modifier)
        {
            modifiers.Add(modifier);
            return this;
        }

        public MethodBuilder AddParameter(string parameterType, string parameterName)
        {
            var nowy = new KeyValuePair<string, string>(parameterType, parameterName);
            parameters.Add(nowy);
            return this;
        }

        public MethodBuilder AddAttribute(ICodeBuilder builder)
        {
            attributes.Add(builder);
            return this;
        }

        public MethodBuilder AddLine(string line)
        {
            lines.Add(line);
            return this;
        }

        public MethodBuilder ParameterInSigleLine(bool parameterInSingleLine)
        {
            this.parameterInSingleLine = parameterInSingleLine;
            return this;
        }

        public MethodBuilder ExtensionMethod(bool extensionMethod)
        {
            this.extensionMethod = extensionMethod;
            return this;
        }

        public MethodBuilder AddContructorInitialization(string keyWord, IEnumerable<string> parameters)
        {
            constructorInitialization = keyWord;
            constructorInitializationParameters = parameters.ToList();
            return this;
        }

        public string Build(string indent = "")
        {
            var builder = new StringBuilder();

            WriteAttributes(indent, builder);

            builder.Append(indent);
            WriteModifiers(builder);
            WriteReturnType(builder);
            builder.Append(name);
            builder.Append("(");
            if (extensionMethod)
                builder.Append("this ");
            var par = parameters.Select(o => o.Key + " " + o.Value).ToArray();

            string connector = PrepareParameterConnector(builder);

            builder.Append(string.Join(connector, par));
            builder.Append(")");
            WriteConstructorInitializationIfNeeded(builder);
            builder.AppendLine();

            builder.AppendLine(indent + "{");

            foreach (var line in lines)
            {
                if (line.Length > 0)
                    builder.AppendLine(indent + ConstsForCode.IndentUnit + line);
                else
                    builder.AppendLine();
            }
            builder.AppendLine(indent + "}");
            return builder.ToString();
        }

        private void WriteConstructorInitializationIfNeeded(StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(constructorInitialization))
            {
                builder.Append(" : ");
                builder.Append(constructorInitialization);
                builder.Append("(");

                var parametry = string.Join(", ", constructorInitializationParameters);
                builder.Append(parametry);

                builder.Append(")");

            }
        }

        private string PrepareParameterConnector(StringBuilder builder)
        {
            var lacznik = ", ";
            if (parameterInSingleLine)
            {
                var lacznikBuilder =
                    new StringBuilder()
                        .Append(",")
                        .AppendLine()
                        .Append(ConstsForCode.DefaultIndentForMethod)
                        .Append(ConstsForCode.IndentUnit);
                lacznik = lacznikBuilder.ToString();
                builder.AppendLine();
                builder.Append(ConstsForCode.DefaultIndentForMethod + ConstsForCode.IndentUnit);
            }

            return lacznik;
        }

        private void WriteAttributes(string indent, StringBuilder builder)
        {
            foreach (var attr in attributes)
                builder.Append(attr.Build(indent));
        }

        private void WriteReturnType(StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(returnType))
                builder.Append(returnType + " ");
        }

        private void WriteModifiers(StringBuilder builder)
        {
            builder.Append(string.Join(" ", modifiers));
            if (modifiers.Any(o => !string.IsNullOrEmpty(o)))
                builder.Append(" ");
        }
    }
}
