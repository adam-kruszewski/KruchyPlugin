using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyParserKodu.ParserKodu
{
    public class Property : ParsowanaJednostka, IWithDocumentation, IWithOwner
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public IList<Modifier> Modyfikatory { get; private set; }
        public List<Attribute> Atrybuty { get; private set; }
        public bool JestGet { get; set; }
        public bool JestSet { get; set; }
        public Documentation Documentation { get; set; }
        public Obiekt Owner { get; set; }

        public Property()
        {
            Modyfikatory = new List<Modifier>();
            Atrybuty = new List<Attribute>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Nazwa);

            builder.Append(" {");
            builder.Append(NazwaTypu);
            builder.Append("}");

            builder.Append(" [");
            builder.Append(string.Join(", " ,Modyfikatory.Select(o => o.Name)));
            builder.Append("]");

            return builder.ToString();
        }
    }
}