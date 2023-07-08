
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKodu.ParserKodu
{
    public class UsingNamespace : ParsedUnit
    {
        public string Nazwa { get; private set; }

        public UsingNamespace(string nazwa)
        {
            Nazwa = nazwa;
            StartPosition = new PlaceInFile();
            EndPosition = new PlaceInFile();
        }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}