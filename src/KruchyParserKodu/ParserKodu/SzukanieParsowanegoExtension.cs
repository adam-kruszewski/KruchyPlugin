using KruchyParserKodu.ParserKodu.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public static class SzukanieParsowanegoExtension
    {
        public static Method SzukajMetodyWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            var metody =
                parsowane
                    .DefinedItems
                        .SelectMany(o => WszystkieMetodyObiektu(o));

            return
                metody
                    .Where(o =>
                        o.StartPosition.Row <= numerLinii
                            && o.EndPosition.Row >= numerLinii)
                            .FirstOrDefault();
        }

        private static IEnumerable<Method> WszystkieMetodyObiektu(DefinedItem obiekt)
        {
            var metodyObiektowWewnetrznych =
                obiekt.InternalDefinedItems.SelectMany(o => WszystkieMetodyObiektu(o));

            return obiekt.Methods.Union(metodyObiektowWewnetrznych);
        }

        public static Constructor SzukajKonstruktoraWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            var konstruktory = 
                parsowane
                    .DefinedItems
                        .SelectMany(o => WszystkieKonstruktoryObiektow(o));

            return konstruktory
                    .Where(o => ZawieraLinie(o, numerLinii))
                        .FirstOrDefault();
        }

        private static IEnumerable<Constructor> WszystkieKonstruktoryObiektow(DefinedItem obiekt)
        {
            var konstruktoryObiektowWewnetrznych =
                obiekt.InternalDefinedItems
                    .SelectMany(o => WszystkieKonstruktoryObiektow(o));

            return obiekt.Constructors.Union(konstruktoryObiektowWewnetrznych);
        }

        public static Property SzukajPropertiesaWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            var propertiesy =
                parsowane
                    .DefinedItems
                        .SelectMany(o => WszystkiePropertiesyObiektow(o));
            return
                propertiesy
                    .Where(o =>
                        o.StartPosition.Row <= numerLinii
                            && o.EndPosition.Row >= numerLinii)
                            .FirstOrDefault();
        }

        private static IEnumerable<Property> WszystkiePropertiesyObiektow(DefinedItem obiekt)
        {
            var propertiesyObiektowWewnetrznych =
                obiekt.InternalDefinedItems.SelectMany(o => WszystkiePropertiesyObiektow(o));

            return obiekt.Properties.Union(propertiesyObiektowWewnetrznych);
        }

        public static Field SzukajPolaWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            var pola = parsowane.DefinedItems.SelectMany(o => o.Fields);
            return
                pola
                    .Where(o =>
                        o.StartPosition.Row <= numerLinii
                            && o.EndPosition.Row >= numerLinii)
                            .FirstOrDefault();
        }

        public static int SzukajPierwszejLiniiDlaMetody(this FileWithCode parsowane)
        {
            if (parsowane.DefinedItems.Count != 1)
                throw new Exception("Liczba definiowanych obiektów rózna od 1");
            var obiekt = parsowane.DefinedItems.First();

            var ostatnieLinieDefinicji = obiekt.Fields.Select(o => o.EndPosition)
                .Union(obiekt.Properties.Select(o => o.EndPosition))
                .Union(obiekt.Constructors.Select(o => o.EndPosition))
                .Union(obiekt.Methods.Select(o => o.EndPosition))
                    .Select(o => o.Row);
            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.StartingBrace.Row + 1;
            }

            return ostatnieLinieDefinicji.Max() + 1;
        }

        public static int SzukajPierwszejLiniiDlaKonstruktora(
            this FileWithCode parsowane,
            int numerLiniiWObiekcie)
        {
            var obiekt = parsowane.SzukajKlasyWLinii(numerLiniiWObiekcie);

            var ostatnieLinieDefinicji =
                obiekt.Fields.Select(o => o.EndPosition)
                    .Union(obiekt.Properties.Select(o => o.EndPosition))
                        .Select(o => o.Row);

            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.StartingBrace.Row + 1;
            }

            return ostatnieLinieDefinicji.Max() + 1;
        }

        public static DefinedItem SzukajKlasyWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            return
                parsowane
                    .DefinedItems
                    .SelectMany(o => WszystkieObiektyObiektu(o))
                    .Where(o => o.KindOfItem == KindOfItem.Class)
                    .Where(o => o.ZawieraLinie(numerLinii))
                    .OrderBy(o => WyliczOdleglosc(o, numerLinii))
                            .FirstOrDefault();
        }

        private static object WyliczOdleglosc(DefinedItem o, int numerLinii)
        {
            return Math.Abs(o.StartPosition.Row - numerLinii)
                + Math.Abs(o.EndPosition.Row - numerLinii);
        }

        public static IEnumerable<DefinedItem> WszystkieObiektyObiektu(DefinedItem obiekt)
        {
            var wynik =
                obiekt.InternalDefinedItems
                    .SelectMany(o => WszystkieObiektyObiektu(o))
                        .ToList();

            wynik.Add(obiekt);

            return wynik;
        }

        public static DefinedItem SzukajObiektuWLinii(
            this FileWithCode parsowane,
            int numerLinii)
        {
            return
                parsowane
                    .DefinedItems
                        .Where(o => o.ZawieraLinie(numerLinii))
                            .FirstOrDefault();
        }

        private static bool ZawieraLinie(
            this ParsedUnit jednostka,
            int numerLinii)
        {
            if (jednostka.StartPosition.Row <= numerLinii
                    && jednostka.EndPosition.Row >= numerLinii)
                return true;
            else
                return false;
        }
    }
}