using FluentAssertions;
using KruchyCompany.KruchyPlugin1.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Utils
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