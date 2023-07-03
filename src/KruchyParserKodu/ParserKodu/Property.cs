using KruchyParserKodu.ParserKodu.Interfaces;
using KruchyParserKodu.ParserKodu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruchyParserKodu.ParserKodu
{
    public class Property : ParsowanaJednostka, IWithDocumentation, IZWlascicielem
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public IList<Modyfikator> Modyfikatory { get; private set; }
        public List<Atrybut> Atrybuty { get; private set; }
        public bool JestGet { get; set; }
        public bool JestSet { get; set; }
        public Documentation Dokumentacja { get; set; }
        public Obiekt Wlasciciel { get; set; }

        public Property()
        {
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Nazwa);

            builder.Append(" {");
            builder.Append(NazwaTypu);
            builder.Append("}");

            builder.Append(" [");
            builder.Append(string.Join(", " ,Modyfikatory.Select(o => o.Nazwa)));
            builder.Append("]");

            return builder.ToString();
        }
    }
}