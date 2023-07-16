using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyCodeBuilders.Builders
{
    public class FileWithCodeBuilder : ICodeBuilder
    {
        private const string indentUnit = ConstsForCode.IndentUnit;
        private string KindOfObjectName { get; set; }
        private string Namespace { get; set; }
        private IList<string> Usings { get; set; }
        private string Name { get; set; }
        private IList<ICodeBuilder> Constructors { get; set; }
        private IList<ICodeBuilder> Methods { get; set; }
        private IList<ICodeBuilder> ClassAttributes { get; set; }
        private ICodeBuilder Object { get; set; }

        public FileWithCodeBuilder()
        {
            WithKindOfObjectName("class");
            Usings = new List<string>();
            Constructors = new List<ICodeBuilder>();
            ClassAttributes = new List<ICodeBuilder>();
        }

        public FileWithCodeBuilder WithKindOfObjectName(string name)
        {
            this.KindOfObjectName = name;
            return this;
        }

        public FileWithCodeBuilder InNamespace(string namespaceName)
        {
            Namespace = namespaceName;
            return this;
        }

        public FileWithCodeBuilder AddUsing(string usingName)
        {
            Usings.Add(usingName);
            return this;
        }

        public FileWithCodeBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public FileWithCodeBuilder WithObject(ICodeBuilder objectBuilder)
        {
            Object = objectBuilder;
            return this;
        }

        public FileWithCodeBuilder WithObjectAttribute(string name, params string[] parameters)
        {
            var atrybutBuilder = new AttributeBuilder().WithName(name);
            foreach (var p in parameters)
                atrybutBuilder.AddParameterValue(p);
            ClassAttributes.Add(atrybutBuilder);
            return this;
        }

        public FileWithCodeBuilder AddContructor(ICodeBuilder builder)
        {
            Constructors.Add(builder);
            return this;
        }

        public FileWithCodeBuilder AddMethod(ICodeBuilder methodBuilder)
        {
            Methods.Add(methodBuilder);
            return this;
        }

        public string Build(string indent = "")
        {
            var outputBuilder = new StringBuilder();

            //usingi
            var usings = Usings.OrderBy(o => o).ToList();
            foreach (var u in usings)
                outputBuilder.AppendLine("using " + u + ";");
            //namespace
            outputBuilder.AppendLine();
            outputBuilder.AppendLine("namespace " + Namespace);
            outputBuilder.AppendLine("{");
            GenerateNamespaceContent(outputBuilder);
            outputBuilder.Append("}");
            return outputBuilder.ToString();
        }

        private void GenerateNamespaceContent(StringBuilder outputBuilder)
        {
            outputBuilder.Append(Object.Build(ConstsForCode.IndentMultiplication(1)));

            ////atrybuty klasy
            //foreach (var a in AtrybutyKlasy)
            //    outputBuilder.Append(a.Build(jednostkaWciecia));
            ////foreach (var a in )
            ////klasa
            //var klasaBuilder = new ClassBuilder().ZNazwa(Nazwa);
            //outputBuilder.AppendLine(klasaBuilder.Build(jednostkaWciecia));
            //konstruktory

            //metody

        }
    }
}