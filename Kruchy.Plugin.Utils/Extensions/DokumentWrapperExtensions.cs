using System.Linq;
using System.Text;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class DokumentWrapperExtensions
    {
        public static void DodajUsingaJesliTrzeba(
            this IDokumentWrapper dokument,
            string nazwaNamespace)
        {
            var parsowane = Parser.Parsuj(dokument.DajZawartosc());

            if (parsowane.Usingi.Select(o => o.Nazwa).Contains(nazwaNamespace))
                return;

            int wierszWstawienia = 1;
            int kolumnaWstawienia = 1;
            var aktualneUsingi = parsowane.Usingi.Select(o => o.Nazwa).ToList();
            aktualneUsingi.Add(nazwaNamespace);

            if (parsowane.Usingi.Any())
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

            dokument.WstawWMiejscu(nowyTekst, wierszWstawienia, 1);
        }

        private static void UsunWszystkieUsingi(
            this IDokumentWrapper dokument,
            Plik parsowane,
            ref int wierszWstawienia,
            ref int kolumnaWstawienia)
        {
            var dotychczasowePosortowane =
                parsowane.Usingi.OrderBy(o => o.Poczatek.Wiersz);

            var pierwszyUsing = dotychczasowePosortowane.First();
            var ostatniUsing = dotychczasowePosortowane.Last();

            dokument.Usun(pierwszyUsing.Poczatek.Wiersz, pierwszyUsing.Poczatek.Kolumna,
                ostatniUsing.Koniec.Wiersz, ostatniUsing.Koniec.Kolumna);
            wierszWstawienia = pierwszyUsing.Poczatek.Wiersz;
            kolumnaWstawienia = pierwszyUsing.Poczatek.Kolumna;
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