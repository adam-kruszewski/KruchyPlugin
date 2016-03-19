using System;
using System.IO;
using System.Linq;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Extensions
{
    static class PlikWrapperExtension
    {
        public static bool JestInterfejsem(this PlikWrapper aktualny)
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

        public static string SzukajSciezkiDoImplementacji(this PlikWrapper aktualny)
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
            return null;
        }

        public static bool JestBuilderem(this PlikWrapper aktualny)
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

        public static string SciezkaKataloguControllera(this PlikWrapper plik)
        {
            string nazwaControllera = DajNazweControllera(plik.NazwaBezRozszerzenia);

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

        public static string DajNazweControllera(this PlikWrapper plik)
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