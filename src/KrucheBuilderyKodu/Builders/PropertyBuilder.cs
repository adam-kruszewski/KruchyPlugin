using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCodeBuilders.Builders
{
    public class PropertyBuilder : ICodeBuilder
    {
        private string nazwa;
        private string nazwaTypu;
        private string modyfikator;

        public PropertyBuilder()
        {
            modyfikator = "public";
        }

        public PropertyBuilder ZNazwa(string nazwa)
        {
            this.nazwa = nazwa;
            return this;
        }

        public PropertyBuilder ZNazwaTypu(string nazwaTypu)
        {
            this.nazwaTypu = nazwaTypu;
            return this;
        }

        public PropertyBuilder ZModyfikatorem(string modyfikator)
        {
            this.modyfikator = modyfikator;
            return this;
        }
        
        public string Build(string wciecie = "")
        {
            var builder = new StringBuilder();
            builder.Append(wciecie);
            builder.Append(modyfikator);
            builder.Append(" ");
            builder.Append(nazwaTypu);
            builder.Append(" ");
            builder.Append(nazwa);
            builder.Append("{ get; set; }");
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
