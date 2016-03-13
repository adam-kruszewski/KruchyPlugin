using System;

namespace KruchyCompany.KruchyPlugin1.Testy
{
    class TestowoAttribute : Attribute
    {
    }

    class Testowo2Attribute : Attribute
    {
        public string Param { get; set; }
    }

    class Testowo3Attribute : Attribute
    {

    }

    class Testowo4Attribute : Attribute
    {
        public Testowo4Attribute(int ile)
        {

        }
    }

    class MapAttribute : Attribute
    {
        public Type Typ { get; set; }

        public MapAttribute(Type typ)
        {
        }
    }
}