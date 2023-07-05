using FluentAssertions;
using KruchyParserKodu.ParserKodu;

namespace KruchyParserKoduTests.Utils
{
    static class PozycjaWPlikuExtension
    {
        public static void Sprawdz(
            this PozycjaWPliku pozycja,
            int wiersz,
            int kolumna)
        {
            pozycja.Row.Should().Be(wiersz);
            pozycja.Column.Should().Be(kolumna);
        }
    }
}