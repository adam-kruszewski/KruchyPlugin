using System;
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
                        .SelectMany(o => o.Metody);
                    //.First().Metody;

            return
                metody
                    .Where(o =>
                        o.Poczatek.Wiersz <= numerLinii
                            && o.Koniec.Wiersz >= numerLinii)
                            .FirstOrDefault();
        }

        public static Konstruktor SzukajKontruktoraWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var konstruktory =
                parsowane
                    .DefiniowaneObiekty
                        .SelectMany(o => o.Konstruktory);
            return konstruktory
                    .Where(o => ZawieraLinie(o, numerLinii))
                        .FirstOrDefault();
        }

        public static Property SzukajPropertiesaWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var propertiesy = parsowane.DefiniowaneObiekty.SelectMany(o => o.Propertiesy);
            return
                propertiesy
                    .Where(o =>
                        o.Poczatek.Wiersz <= numerLinii
                            && o.Koniec.Wiersz >= numerLinii)
                            .FirstOrDefault();
        }

        public static Pole SzukajPolaWLinii(
            this Plik parsowane,
            int numerLinii)
        {
            var pola = parsowane.DefiniowaneObiekty.SelectMany(o => o.Pola);
            return
                pola
                    .Where(o =>
                        o.Poczatek.Wiersz <= numerLinii
                            && o.Koniec.Wiersz >= numerLinii)
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
                    .Select(o => o.Wiersz);
            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.PoczatkowaKlamerka.Wiersz + 1;
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
                        .Select(o => o.Wiersz);

            if (ostatnieLinieDefinicji.Count() == 0)
            {
                return obiekt.PoczatkowaKlamerka.Wiersz + 1;
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
                        .Where(o => o.Rodzaj == RodzajObiektu.Klasa)
                        .Where(o => o.ZawieraLinie(numerLinii))
                            .FirstOrDefault();
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
            if (jednostka.Poczatek.Wiersz <= numerLinii
                    && jednostka.Koniec.Wiersz >= numerLinii)
                return true;
            else
                return false;
        }
    }
}