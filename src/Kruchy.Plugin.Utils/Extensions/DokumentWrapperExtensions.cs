using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class DokumentWrapperExtensions
    {
        public static void DodajUsingaJesliTrzeba(
            this IDocumentWrapper dokument,
            params string[] nazwyNamespace)
        {
            var parsowane = Parser.Parsuj(dokument.GetContent());

            if (nazwyNamespace.All(o => ZawieraUsingNamespace(o, parsowane)))
                return;

            int wierszWstawienia = 1;
            int kolumnaWstawienia = 1;
            var aktualneUsingi = parsowane.Usings.Select(o => o.Nazwa).ToList();
            aktualneUsingi.AddRange(
                nazwyNamespace.Where(o => !ZawieraUsingNamespace(o, parsowane)));

            if (parsowane.Usings.Any())
            {
                UsunWszystkieUsingi(
                    dokument,
                    parsowane,
                    ref wierszWstawienia,
                    ref kolumnaWstawienia);
            }

            var posortowaneDoWstawienia =
                aktualneUsingi
                    .OrderBy(o => DajKluczDoSortowaniaUsingow(o))
                        .ToList();
            var builder = new StringBuilder();
            foreach (var u in posortowaneDoWstawienia)
                builder.AppendLine("using " + u + ";");
            var nowyTekst = builder.ToString().TrimEnd();

            dokument.InsertInPlace(nowyTekst, wierszWstawienia, 1);
        }

        private static bool ZawieraUsingNamespace(string nazwaNamespace, FileWithCode parsowane)
        {
            return parsowane.Usings.Select(o => o.Nazwa).Contains(nazwaNamespace);
        }

        private static void UsunWszystkieUsingi(
            this IDocumentWrapper dokument,
            FileWithCode parsowane,
            ref int wierszWstawienia,
            ref int kolumnaWstawienia)
        {
            var dotychczasowePosortowane =
                parsowane.Usings.OrderBy(o => o.StartPosition.Row);

            var pierwszyUsing = dotychczasowePosortowane.First();
            var ostatniUsing = dotychczasowePosortowane.Last();

            dokument.Remove(pierwszyUsing.StartPosition.Row, pierwszyUsing.StartPosition.Column,
                ostatniUsing.EndPosition.Row, ostatniUsing.EndPosition.Column);
            wierszWstawienia = pierwszyUsing.StartPosition.Row;
            kolumnaWstawienia = pierwszyUsing.StartPosition.Column;
        }

        private static string DajKluczDoSortowaniaUsingow(string nazwaUsinga)
        {
            if (nazwaUsinga.StartsWith("System.") || nazwaUsinga == "System")
                return "0" + nazwaUsinga;
            else
                return "1" + nazwaUsinga;
        }

    }
}