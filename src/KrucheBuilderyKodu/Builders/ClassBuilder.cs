using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class ClassBuilder : ICodeBuilder
    {
        private string modifier { get; set; }
        private string name { get; set; }
        private string superClassName { get; set; }
        private IList<string> interfaces { get; set; }
        private IList<ICodeBuilder> methods { get; set; }
        private IList<ICodeBuilder> constructors { get; set; }
        private IList<ICodeBuilder> attributes { get; set; }

        public ClassBuilder()
        {
            methods = new List<ICodeBuilder>();
            constructors = new List<ICodeBuilder>();
            attributes = new List<ICodeBuilder>();
            interfaces = new List<string>();
        }

        public ClassBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public ClassBuilder WithModifier(string modifier)
        {
            this.modifier = modifier;
            return this;
        }

        public ClassBuilder WithSuperClass(string name)
        {
            superClassName = name;
            return this;
        }

        public ClassBuilder AddInterface(string name)
        {
            interfaces.Add(name);
            return this;
        }

        public ClassBuilder AddContructor(ICodeBuilder constructor)
        {
            constructors.Add(constructor);
            return this;
        }

        public ClassBuilder AddMethod(ICodeBuilder methodBuilder)
        {
            methods.Add(methodBuilder);
            return this;
        }

        public ClassBuilder AddAttribute(ICodeBuilder attributeBuilder)
        {
            attributes.Add(attributeBuilder);
            return this;
        }

        public string Build(string indent = "")
        {
            var outputBuilder = new StringBuilder();

            foreach (var a in attributes)
                outputBuilder.Append(a.Build(ConstsForCode.IndentUnit));

            outputBuilder.Append(indent);
            if (!string.IsNullOrEmpty(modifier))
                outputBuilder.Append(modifier + " ");
            outputBuilder.Append("class ");
            outputBuilder.Append(name);
            if (!string.IsNullOrEmpty(superClassName))
                outputBuilder.Append(" : " + superClassName);

            var indentForInterface = indent;
            if (interfaces.Any())
            {
                if (string.IsNullOrEmpty(superClassName))
                    outputBuilder.Append(" : ");
                else
                {
                    outputBuilder.Append(indentForInterface);
                    outputBuilder.Append(", ");
                }
                outputBuilder.Append(interfaces.First());
            }

            for (int i = 1; i < interfaces.Count; i++)
            {
                indentForInterface += ConstsForCode.IndentUnit;
                outputBuilder.Append(indentForInterface);
                outputBuilder.Append(", ");
                outputBuilder.AppendLine(interfaces[i]);
            }

            outputBuilder.AppendLine();
            outputBuilder.AppendLine(indent + "{");

            foreach (var k in constructors)
                outputBuilder.AppendLine(
                    k.Build(ConstsForCode.IndentMultiplication(2)));

            foreach (var m in methods)
                outputBuilder.AppendLine(
                    m.Build(ConstsForCode.IndentMultiplication(2)));

            outputBuilder.AppendLine(indent + "}");
            return outputBuilder.ToString();
        }
    }
}
