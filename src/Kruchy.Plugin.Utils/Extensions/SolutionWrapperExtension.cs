using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class SolutionWrapperExtension
    {
        public static string NamespaceAktualnegoPliku(this ISolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.GetContent();
            return Parser.Parse(zawartosc).Namespace;
        }

        public static string NazwaObiektuAktualnegoPliku(
            this ISolutionWrapper solution)
        {
            if (solution.AktualnyDokument == null)
                return null;

            var zawartosc = solution.AktualnyDokument.GetContent();
            var parsowane = Parser.Parse(zawartosc);
            if (parsowane.DefinedItems.Count <= 0)
                return null;

            return parsowane.DefinedItems[0].Name;
        }

        public static string NazwaAktualnejMetody(this ISolutionWrapper solution)
        {
            var zawartosc = solution.AktualnyDokument.GetContent();
            var parsowane = Parser.Parse(zawartosc);
            var liniaKursora = solution.AktualnyDokument.GetCursorLineNumber();

            var aktualnaMetoda = parsowane.FindMethodByLineNumber(liniaKursora);

            if (aktualnaMetoda != null)
                return aktualnaMetoda.Name;
            else
                return string.Empty;
        }

        public static FileWithCode ParsujZawartoscAktualnegoDokumetu(
            this ISolutionWrapper solution)
        {
            return Parser.Parse(solution.AktualnyDokument.GetContent());
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
