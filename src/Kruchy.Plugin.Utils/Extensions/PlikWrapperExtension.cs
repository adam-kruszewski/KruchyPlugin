using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
using System;
using System.IO;
using System.Linq;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class PlikWrapperExtension
    {
        public static bool JestInterfejsem(this IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefinedItems.Count == 1)
            {
                return parsowane.DefinedItems[0].KindOfItem == KindOfItem.Interface;
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
            if (parsowane.DefinedItems.Count == 1)
            {
                var obiekt = parsowane.DefinedItems[0];
                if (obiekt.Name.EndsWith("Builder")
                    && obiekt.KindOfItem == KindOfItem.Class)
                    return true;
            }
            return false;
        }

        public static bool JestWBuilderze(this IFileWrapper aktualny)
        {
            var zawartosc = aktualny.Document.GetContent();
            var parsowane = Parser.Parsuj(zawartosc);
            var obiekt =
                parsowane.FindDefinedItemByLineNumber(aktualny.Document.GetCursorLineNumber());
            if (obiekt != null)
            {
                if (obiekt.Name.EndsWith("Builder")
                    && obiekt.KindOfItem == KindOfItem.Class)
                    return true;
            }
            return false;
        }

        public static string SciezkaKataloguControllera(this IFileWrapper plik)
        {
            var parsowane = Parser.Parsuj(plik.Document.GetContent());
            string nazwaControllera = DajNazweControllera(parsowane.DefinedItems.Single().Name);

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
            if (parsowane.DefinedItems.Count() == 1)
            {
                var nazwa = parsowane.DefinedItems.First().Name;
                if (nazwa.EndsWith("Controller"))
                    return DajNazweControllera(nazwa);
            }
            return null;
        }
    }
}
