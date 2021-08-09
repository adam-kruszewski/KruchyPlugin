using System;
using System.Collections.Generic;

namespace KruchyParserKoduTests.Samples
{
    internal interface InterfejsDoParsowania
    {
        int? Wlasciwosc1 { get; }
        int Wlasciwosc2 { get; set; }

        string Metoda1(int a, string b, DateTime? c);
    }
}