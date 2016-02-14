using System.Collections.Generic;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Property : ParsowanaJednostka
    {
        public string Nazwa { get; set; }
        public string NazwaTypu { get; set; }
        public IList<Modyfikator> Modyfikatory { get; private set; }
        public List<Atrybut> Atrybuty { get; private set; }

        public Property()
        {
            Modyfikatory = new List<Modyfikator>();
            Atrybuty = new List<Atrybut>();
        }
    }
}