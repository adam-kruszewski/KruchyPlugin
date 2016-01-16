using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrucheBuilderyKodu.Builders
{
    public class AtrybutBuilder : ICodeBuilder
    {
        private string Nazwa { get; set; }

        private IList<string> Parametry { get; set; }
        private IList<AtrybutBuilder> KolejneAtrybuty { get; set; }
        private bool Nawiasy { get; set; }

        public AtrybutBuilder()
        {
            Parametry = new List<string>();
            KolejneAtrybuty = new List<AtrybutBuilder>();
            Nawiasy = true;
        }

        public AtrybutBuilder ZNazwa(string nazwa)
        {
            Nazwa = nazwa;
            return this;
        }

        public AtrybutBuilder DodajWartoscParametru(string wartosc)
        {
            Parametry.Add("\"" + wartosc + "\"");
            return this;
        }

        public AtrybutBuilder DodajWartoscParametruNieStringowa(string napis)
        {
            Parametry.Add(napis);
            return this;
        }

        public AtrybutBuilder DodajKolejnyAtrybut(AtrybutBuilder builder)
        {
            KolejneAtrybuty.Add(builder);
            return this;
        }

        public AtrybutBuilder BezNawiasow()
        {
            Nawiasy = false;
            return this;
        }

        public string Build(string wciecie = "")
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append(wciecie);
            if (Nawiasy)
                outputBuilder.Append("[");
            outputBuilder.Append(Nazwa);

            if (Parametry.Any())
            {
                outputBuilder.Append("(");
                outputBuilder.Append(string.Join(", ", Parametry));
                outputBuilder.Append(")");
            }
            foreach (var kolejnyAtrybut in KolejneAtrybuty)
            {
                outputBuilder.Append(", ");
                outputBuilder.Append(kolejnyAtrybut.BezNawiasow().Build());
            }

            if (Nawiasy)
                outputBuilder.AppendLine("]");

            return outputBuilder.ToString();
        }
    }
}
