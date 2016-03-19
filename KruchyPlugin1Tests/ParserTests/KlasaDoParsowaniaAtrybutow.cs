
namespace KruchyCompany.KruchyPlugin1Tests.ParserTests
{
    [Testowo]
    [Map(typeof(KlasaDoParsowaniaAtrybutow))]
    class KlasaDoParsowaniaAtrybutow
    {
        [Testowo, Testowo3]
        public void Metoda1()
        {

        }

        [Testowo2(Param = "aa")]
        [Testowo3]
        public void Metoda2()
        {

        }

        [Testowo2(Param = "bb"), Testowo]
        public void Metoda3()
        {

        }

        [Testowo4(1)]
        public void Metoda4()
        {

        }
    }
}
