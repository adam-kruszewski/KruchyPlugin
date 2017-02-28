
namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    public static class KlasaDoParsowaniaRozszerzone
    {
        public static int Metoda1(this int liczba, int liczba2 = 2, string s = "aa")
        {
            return 0;
        }

        public static void MetodaZParam(int liczba, params string[] tablica)
        {

        }

        public static void MetodaZRefIOut(ref object obiekt, out decimal d)
        {
            d = 1m;
        }
    }
}