using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class SolutionWrapperExtension
    {
        public static string NamespaceAktualnegoPliku(this ISolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            return Parser.Parsuj(zawartosc).Namespace;
        }

        public static string NazwaObiektuAktualnegoPliku(
            this ISolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count <= 0)
                return null;

            return parsowane.DefiniowaneObiekty[0].Nazwa;
        }

        public static string NazwaAktualnejMetody(this ISolutionWrapper solution)
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

        public static Plik ParsujZawartoscAktualnegoDokumetu(
            this ISolutionWrapper solution)
        {
            return Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
        }

        public static IProjektWrapper SzukajProjektuTestowego(
            this ISolutionWrapper solution,
            IProjektWrapper projekt)
        {
            var nazwaSzukanegoProjektu = solution.AktualnyProjekt.Nazwa + ".Tests";
            var projektTestow = SzukajProjektuWgNazwy(solution, nazwaSzukanegoProjektu);
            return projektTestow;
        }

        public static IProjektWrapper SzukajProjektuModulu(
            this ISolutionWrapper solution,
            IProjektWrapper projekt)
        {
            var nazwaSzukanegoProjektu =
                solution.AktualnyProjekt.Nazwa.Replace(".Tests", "");
            return SzukajProjektuWgNazwy(solution, nazwaSzukanegoProjektu);
        }

        private static IProjektWrapper SzukajProjektuWgNazwy(
            ISolutionWrapper solution,
            string nazwaSzukanegoProjektu)
        {
            var projekt =
                solution
                    .Projekty
                        .Where(o => o.Nazwa == nazwaSzukanegoProjektu)
                            .FirstOrDefault();
            return projekt;
        }
    }
}
