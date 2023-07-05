
namespace KruchyParserKodu.ParserKodu.Models
{
    public class Modifier : ParsowanaJednostka
    {
        public string Name { get; private set; }

        public Modifier(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}