using KruchyParserKodu.ParserKodu.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KruchyParserKodu.ParserKodu
{
    public static class SzukanieParsowanegoExtension
    {
        public static Metoda SzukajMetodyWLinii(
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

        private static IEnumerable<Metoda> WszystkieMetodyObiektu(Obiekt obiekt)
        {
            var metodyObiektowWewnetrznych =
                obiekt.ObiektyWewnetrzne.SelectMany(o => WszystkieMetodyObiektu(o));

            return obiekt.Metody.Union(metodyObiektowWewnetrznych);
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

        private static IEnumerable<Constructor> WszystkieKonstruktoryObiektow(Obiekt obiekt)
        {
            var konstruktoryObiektowWewnetrznych =
                obiekt.ObiektyWewnetrzne
                    .SelectMany(o => WszystkieKonstruktoryObiektow(o));

            return obiekt.Konstruktory.Union(konstruktoryObiektowWewnetrznych);
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

        private static IEnumerable<Property> WszystkiePropertiesyObiektow(Obiekt obiekt)
        {
            var propertiesyObiektowWewnetrznych =
                obiekt.ObiektyWewnetrzne.SelectMany(o => WszystkiePropertiesyObiektow(o));

            return obiekt.Propertiesy.Union(propertiesyObiektowWewnetrznych);
        }

        public static Pole SzukajPolaWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var pola = parsowane.DefiniowaneObiekty.SelectMany(o => o.Pola);
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

            var ostatnieLinieDefinicji = obiekt.Pola.Select(o => o.Koniec)
                .Union(obiekt.Propertiesy.Select(o => o.Koniec))
                .Union(obiekt.Konstruktory.Select(o => o.Koniec))
                .Union(obiekt.Metody.Select(o => o.Koniec))
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
                obiekt.Pola.Select(o => o.Koniec)
                    .Union(obiekt.Propertiesy.Select(o => o.Koniec))
                        .Select(o => o.Row);

            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.StartingBrace.Row + 1;
            }

            return ostatnieLinieDefinicji.Max() + 1;
        }

        public static Obiekt SzukajKlasyWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            return
                parsowane
                    .DefiniowaneObiekty
                    .SelectMany(o => WszystkieObiektyObiektu(o))
                    .Where(o => o.Rodzaj == RodzajObiektu.Klasa)
                    .Where(o => o.ZawieraLinie(numerLinii))
                    .OrderBy(o => WyliczOdleglosc(o, numerLinii))
                            .FirstOrDefault();
        }

        private static object WyliczOdleglosc(Obiekt o, int numerLinii)
        {
            return Math.Abs(o.Poczatek.Row - numerLinii)
                + Math.Abs(o.Koniec.Row - numerLinii);
        }

        public static IEnumerable<Obiekt> WszystkieObiektyObiektu(Obiekt obiekt)
        {
            var wynik =
                obiekt.ObiektyWewnetrzne
                    .SelectMany(o => WszystkieObiektyObiektu(o))
                        .ToList();

            wynik.Add(obiekt);

            return wynik;
        }

        public static Obiekt SzukajObiektuWLinii(
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