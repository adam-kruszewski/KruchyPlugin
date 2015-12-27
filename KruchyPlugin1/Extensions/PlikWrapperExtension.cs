using System;
using System.IO;
using KruchyCompany.KruchyPlugin1.ParserKodu;
using KruchyCompany.KruchyPlugin1.Utils;

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
    }
}