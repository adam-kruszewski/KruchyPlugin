using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KruchyCompany.KruchyPlugin1.CodeBuilders
{
    class AtrybutBuilder : ICodeBuilder
    {
        private string Nazwa { get; set; }

        private IList<string> Parametry { get; set; }

        public AtrybutBuilder()
        {
            Parametry = new List<string>();
        }

        public AtrybutBuilder ZNazwa(string nazwa)
        {
            Nazwa = nazwa;
            return this;
        }

        public AtrybutBuilder DodajWartoscParametru(string wartosc)
        {
            Parametry.Add(wartosc);
            return this;
        }

        public string Build(string wciecie = "")
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append(wciecie);
            outputBuilder.Append("[");
            outputBuilder.Append(Nazwa);

            if (Parametry.Any())
            {
                outputBuilder.Append("(");
                var p = Parametry.Select(o => "\"" + o + "\"");
                outputBuilder.Append(string.Join(", ", p));
                outputBuilder.Append(")");
            }

            outputBuilder.AppendLine("]");

            return outputBuilder.ToString();
        }
    }
}
