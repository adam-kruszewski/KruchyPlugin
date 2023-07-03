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

            var zawartosc = solution.AktualnyDokument.GetContent();
            return Parser.Parsuj(zawartosc).Namespace;
        }

        public static string NazwaObiektuAktualnegoPliku(
            this ISolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count <= 0)
                return null;

            return parsowane.DefiniowaneObiekty[0].Name;
        }

        public static string NazwaAktualnejMetody(this ISolutionWrapper solution)
        {
            var zawartosc = solution.AktualnyDokument.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            var liniaKursora = solution.AktualnyDokument.GetCursorLineNumber();

            var aktualnaMetoda = parsowane.SzukajMetodyWLinii(liniaKursora);

            if (aktualnaMetoda != null)
                return aktualnaMetoda.Nazwa;
            else
                return string.Empty;
        }

        public static Plik ParsujZawartoscAktualnegoDokumetu(
            this ISolutionWrapper solution)
        {
            return Parser.Parsuj(solution.AktualnyDokument.GetContent());
        }

        public static IProjectWrapper SzukajProjektuWgNazwy(
            this ISolutionWrapper solution,
            string nazwaSzukanegoProjektu)
        {
            var projekt =
                solution
                    .Projekty
                        .Where(o => o.Name.ToLower() == nazwaSzukanegoProjektu.ToLower())
                            .FirstOrDefault();
            return projekt;
        }
    }
}
