using System.Collections.Generic;
using System.Text;

namespace KrucheBuilderyKodu.Builders
{
    public class PoleBuilder
    {
        private string nazwa;
        private string nazwaTypu;
        private IList<string> modyfikatory;

        public PoleBuilder()
        {
            modyfikatory = new List<string>();
        }

        public PoleBuilder ZNazwa(string nazwa)
        {
            this.nazwa = nazwa;
            return this;
        }

        public PoleBuilder ZNazwaTypu(string nazwaTypu)
        {
            this.nazwaTypu = nazwaTypu;
            return this;
        }

        public PoleBuilder DodajModyfikatorem(string modyfikator)
        {
            modyfikatory.Add(modyfikator);
            return this;
        }
        
        public string Build(string wciecie = "")
        {
            var builder = new StringBuilder();
            builder.Append(wciecie);
            builder.Append(string.Join(" ", modyfikatory));
            builder.Append(" ");
            builder.Append(nazwaTypu);
            builder.Append(" ");
            builder.Append(nazwa);
            builder.AppendLine(";");
            return builder.ToString();
        }
    }
}
