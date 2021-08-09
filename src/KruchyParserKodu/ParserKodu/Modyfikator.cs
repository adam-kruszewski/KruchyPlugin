
namespace KruchyParserKodu.ParserKodu
{
    public class Modyfikator : ParsowanaJednostka
    {
        public string Nazwa { get; private set; }

        public Modyfikator(string nazwa)
        {
            Nazwa = nazwa;
        }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}