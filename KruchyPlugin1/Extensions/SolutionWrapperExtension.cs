using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Extensions
{
    static class SolutionWrapperExtension
    {
        public static string NamespaceAktualnegoPliku(this SolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            return Parser.Parsuj(zawartosc).Namespace;
        }

        public static string NazwaObiektuAktualnegoPliku(
            this SolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count <= 0)
                return null;

            return parsowane.DefiniowaneObiekty[0].Nazwa;
        }

        public static string NazwaAktualnejMetody(this SolutionWrapper solution)
        {
            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            var liniaKursora = solution.AktualnyDokument.DajNumerLiniiKursora();

            var aktualnaMetoda = parsowane.SzukajMetodyWLinii(liniaKursora);

            if (aktualnaMetoda != null)
                return aktualnaMetoda.Nazwa;
            else
                return string.Empty;
        }
    }
}