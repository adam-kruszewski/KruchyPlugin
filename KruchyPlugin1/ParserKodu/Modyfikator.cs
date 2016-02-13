
namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class Modyfikator : ParsowanaJednostka
    {
        public string Nazwa { get; private set; }

        public Modyfikator(string nazwa)
        {
            Nazwa = nazwa;
        }
    }
}