using KruchyParserKodu.ParserKodu.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public static class SzukanieParsowanegoExtension
    {
        public static Method SzukajMetodyWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var metody =
                parsowane
                    .DefiniowaneObiekty
                        .SelectMany(o => WszystkieMetodyObiektu(o));

            return
                metody
                    .Where(o =>
                        o.Poczatek.Row <= numerLinii
                            && o.Koniec.Row >= numerLinii)
                            .FirstOrDefault();
        }

        private static IEnumerable<Method> WszystkieMetodyObiektu(DefinedItem obiekt)
        {
            var metodyObiektowWewnetrznych =
                obiekt.InternalDefinedItems.SelectMany(o => WszystkieMetodyObiektu(o));

            return obiekt.Methods.Union(metodyObiektowWewnetrznych);
        }

        public static Constructor SzukajKonstruktoraWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var konstruktory = 
                parsowane
                    .DefiniowaneObiekty
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
            this Plik parsowane,
            int numerLinii)
        {
            var propertiesy =
                parsowane
                    .DefiniowaneObiekty
                        .SelectMany(o => WszystkiePropertiesyObiektow(o));
            return
                propertiesy
                    .Where(o =>
                        o.Poczatek.Row <= numerLinii
                            && o.Koniec.Row >= numerLinii)
                            .FirstOrDefault();
        }

        private static IEnumerable<Property> WszystkiePropertiesyObiektow(DefinedItem obiekt)
        {
            var propertiesyObiektowWewnetrznych =
                obiekt.InternalDefinedItems.SelectMany(o => WszystkiePropertiesyObiektow(o));

            return obiekt.Properties.Union(propertiesyObiektowWewnetrznych);
        }

        public static Pole SzukajPolaWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var pola = parsowane.DefiniowaneObiekty.SelectMany(o => o.Fields);
            return
                pola
                    .Where(o =>
                        o.Poczatek.Row <= numerLinii
                            && o.Koniec.Row >= numerLinii)
                            .FirstOrDefault();
        }

        public static int SzukajPierwszejLiniiDlaMetody(this Plik parsowane)
        {
            if (parsowane.DefiniowaneObiekty.Count != 1)
                throw new Exception("Liczba definiowanych obiektów rózna od 1");
            var obiekt = parsowane.DefiniowaneObiekty.First();

            var ostatnieLinieDefinicji = obiekt.Fields.Select(o => o.Koniec)
                .Union(obiekt.Properties.Select(o => o.Koniec))
                .Union(obiekt.Constructors.Select(o => o.Koniec))
                .Union(obiekt.Methods.Select(o => o.Koniec))
                    .Select(o => o.Row);
            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.StartingBrace.Row + 1;
            }

            return ostatnieLinieDefinicji.Max() + 1;
        }

        public static int SzukajPierwszejLiniiDlaKonstruktora(
            this Plik parsowane,
            int numerLiniiWObiekcie)
        {
            var obiekt = parsowane.SzukajKlasyWLinii(numerLiniiWObiekcie);

            var ostatnieLinieDefinicji =
                obiekt.Fields.Select(o => o.Koniec)
                    .Union(obiekt.Properties.Select(o => o.Koniec))
                        .Select(o => o.Row);

            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.StartingBrace.Row + 1;
            }

            return ostatnieLinieDefinicji.Max() + 1;
        }

        public static DefinedItem SzukajKlasyWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            return
                parsowane
                    .DefiniowaneObiekty
                    .SelectMany(o => WszystkieObiektyObiektu(o))
                    .Where(o => o.KindOfItem == RodzajObiektu.Klasa)
                    .Where(o => o.ZawieraLinie(numerLinii))
                    .OrderBy(o => WyliczOdleglosc(o, numerLinii))
                            .FirstOrDefault();
        }

        private static object WyliczOdleglosc(DefinedItem o, int numerLinii)
        {
            return Math.Abs(o.Poczatek.Row - numerLinii)
                + Math.Abs(o.Koniec.Row - numerLinii);
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
            this Plik parsowane,
            int numerLinii)
        {
            return
                parsowane
                    .DefiniowaneObiekty
                        .Where(o => o.ZawieraLinie(numerLinii))
                            .FirstOrDefault();
        }

        private static bool ZawieraLinie(
            this ParsowanaJednostka jednostka,
            int numerLinii)
        {
            if (jednostka.Poczatek.Row <= numerLinii
                    && jednostka.Koniec.Row >= numerLinii)
                return true;
            else
                return false;
        }
    }
}