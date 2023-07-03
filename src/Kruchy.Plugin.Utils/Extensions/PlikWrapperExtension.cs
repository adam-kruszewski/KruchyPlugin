using System;
using System.IO;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class PlikWrapperExtension
    {
        public static bool JestInterfejsem(this IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                return parsowane.DefiniowaneObiekty[0].Rodzaj == RodzajObiektu.Interfejs;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        public static string SzukajSciezkiDoImplementacji(this IFileWrapper aktualny)
        {
            var katalog = aktualny.Directory;
            var katalogImpl = Path.Combine(katalog, "Impl");
            var nazwa = aktualny.Name.Substring(1);
            var sciezka = Path.Combine(katalogImpl, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Directory, nazwa);
            if (File.Exists(sciezka))
                return sciezka;

            return aktualny.Project.Files
                .FirstOrDefault(o => o.Name == nazwa)
                ?.FullPath;
        }


        public static string SzukajSciezkiDoInterfejsu(this IFileWrapper aktualny)
        {
            var katalog = aktualny.Directory;
            var katalogInterfejsu = Directory.GetParent(katalog).FullName;
            var nazwa = "I" + aktualny.Name;
            var sciezka = Path.Combine(katalogInterfejsu, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Directory, nazwa);
            if (File.Exists(sciezka))
                return sciezka;

            return aktualny.Project.Files
                .FirstOrDefault(o => o.Name == nazwa)
                ?.FullPath;
        }


        public static bool JestBuilderem(this IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                var obiekt = parsowane.DefiniowaneObiekty[0];
                if (obiekt.Name.EndsWith("Builder")
                    && obiekt.Rodzaj == RodzajObiektu.Klasa)
                    return true;
            }
            return false;
        }

        public static bool JestWBuilderze(this IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            var obiekt =
                parsowane.SzukajObiektuWLinii(aktualny.Document.GetCursorLineNumber());
            if (obiekt != null)
            {
                if (obiekt.Name.EndsWith("Builder")
                    && obiekt.Rodzaj == RodzajObiektu.Klasa)
                    return true;
            }
            return false;
        }

        public static string SciezkaKataloguControllera(this IFileWrapper plik)
        {
            var parsowane = Parser.Parsuj(plik.Document.GetContent());
            string nazwaControllera = DajNazweControllera(parsowane.DefiniowaneObiekty.Single().Name);

            var katalogPlikControllera = plik.Directory;
            var katalogDlaControllera =
                Path.Combine(
                    Directory.GetParent(katalogPlikControllera).FullName,
                    "Views",
                    nazwaControllera);

            return katalogDlaControllera;
        }

        private static string DajNazweControllera(string nazwaKlasyControllera)
        {
            var dl = "Controller".Length;
            return nazwaKlasyControllera.Substring(
                0,
                nazwaKlasyControllera.Length - dl);
        }

        public static string DajNazweControllera(this IFileWrapper plik)
        {
            var zawartosc = plik.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count() == 1)
            {
                var nazwa = parsowane.DefiniowaneObiekty.First().Name;
                if (nazwa.EndsWith("Controller"))
                    return DajNazweControllera(nazwa);
            }
            return null;
        }
    }
}
