using System;
using System.IO;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Utils.Extensions
{
    public static class PlikWrapperExtension
    {
        public static bool JestInterfejsem(this IPlikWrapper aktualny)
        {
            var zawartosc = aktualny.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                return parsowane.DefiniowaneObiekty[0].Rodzaj == RodzajObiektu.Interfejs;
            }
            else
                throw new Exception("Brak zdefiniowanego obiektu");
        }

        public static string SzukajSciezkiDoImplementacji(this IPlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogImpl = Path.Combine(katalog, "Impl");
            var nazwa = aktualny.Nazwa.Substring(1);
            var sciezka = Path.Combine(katalogImpl, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;

            return aktualny.Projekt.Files
                .FirstOrDefault(o => o.Nazwa == nazwa)
                ?.SciezkaPelna;
        }


        public static string SzukajSciezkiDoInterfejsu(this IPlikWrapper aktualny)
        {
            var katalog = aktualny.Katalog;
            var katalogInterfejsu = Directory.GetParent(katalog).FullName;
            var nazwa = "I" + aktualny.Nazwa;
            var sciezka = Path.Combine(katalogInterfejsu, nazwa);
            if (File.Exists(sciezka))
                return sciezka;
            sciezka = Path.Combine(aktualny.Katalog, nazwa);
            if (File.Exists(sciezka))
                return sciezka;

            return aktualny.Projekt.Files
                .FirstOrDefault(o => o.Nazwa == nazwa)
                ?.SciezkaPelna;
        }


        public static bool JestBuilderem(this IPlikWrapper aktualny)
        {
            var zawartosc = aktualny.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count == 1)
            {
                var obiekt = parsowane.DefiniowaneObiekty[0];
                if (obiekt.Nazwa.EndsWith("Builder")
                    && obiekt.Rodzaj == RodzajObiektu.Klasa)
                    return true;
            }
            return false;
        }

        public static bool JestWBuilderze(this IPlikWrapper aktualny)
        {
            var zawartosc = aktualny.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            var obiekt =
                parsowane.SzukajObiektuWLinii(aktualny.Dokument.DajNumerLiniiKursora());
            if (obiekt != null)
            {
                if (obiekt.Nazwa.EndsWith("Builder")
                    && obiekt.Rodzaj == RodzajObiektu.Klasa)
                    return true;
            }
            return false;
        }

        public static string SciezkaKataloguControllera(this IPlikWrapper plik)
        {
            var parsowane = Parser.Parsuj(plik.Dokument.DajZawartosc());
            string nazwaControllera = DajNazweControllera(parsowane.DefiniowaneObiekty.Single().Nazwa);

            var katalogPlikControllera = plik.Katalog;
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

        public static string DajNazweControllera(this IPlikWrapper plik)
        {
            var zawartosc = plik.Dokument.DajZawartosc();
            var parsowane = Parser.Parsuj(zawartosc);
            if (parsowane.DefiniowaneObiekty.Count() == 1)
            {
                var nazwa = parsowane.DefiniowaneObiekty.First().Nazwa;
                if (nazwa.EndsWith("Controller"))
                    return DajNazweControllera(nazwa);
            }
            return null;
        }
    }
}
