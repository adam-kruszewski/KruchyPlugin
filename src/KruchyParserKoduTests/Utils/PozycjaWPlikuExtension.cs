using FluentAssertions;
using KruchyParserKodu.ParserKodu.Models;

namespace KruchyParserKoduTests.Utils
{
    static class PozycjaWPlikuExtension
    {
        public static void Sprawdz(
            this PlaceInFile pozycja,
            int wiersz,
            int kolumna)
        {
            pozycja.Row.Should().Be(wiersz);
            pozycja.Column.Should().Be(kolumna);
        }
    }
}