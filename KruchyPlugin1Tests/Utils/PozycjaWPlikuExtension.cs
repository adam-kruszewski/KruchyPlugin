using FluentAssertions;
using KruchyParserKodu.ParserKodu;

namespace KruchyPlugin1Tests.Utils
{
    static class PozycjaWPlikuExtension
    {
        public static void Sprawdz(
            this PozycjaWPliku pozycja,
            int wiersz,
            int kolumna)
        {
            pozycja.Wiersz.Should().Be(wiersz);
            pozycja.Kolumna.Should().Be(kolumna);
        }
    }
}