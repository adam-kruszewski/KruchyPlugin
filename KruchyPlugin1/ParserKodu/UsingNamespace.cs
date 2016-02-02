
namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public class UsingNamespace : ParsowanaJednostka
    {
        public string Nazwa { get; private set; }

        public UsingNamespace(string nazwa)
        {
            Nazwa = nazwa;
            Poczatek = new PozycjaWPliku();
            Koniec = new PozycjaWPliku();
        }

        public override string ToString()
        {
            return Nazwa;
        }
    }
}