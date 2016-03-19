using System.Collections.Generic;

namespace KruchyParserKodu.ParserKodu
{
    public class Atrybut : ParsowanaJednostka
    {
        public string Nazwa { get; set; }

        public IList<ParametrAtrybutu> Parametry;

        public Atrybut()
        {
            Parametry = new List<ParametrAtrybutu>();
        }
    }
}