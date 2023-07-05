
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu
{
    public class UsingNamespace : ParsowanaJednostka
    {
        public string Nazwa { get; private set; }

        public UsingNamespace(string nazwa)
        {
            Nazwa = nazwa;
            Poczatek = new PlaceInFile();
            Koniec = new PlaceInFile();
        }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}