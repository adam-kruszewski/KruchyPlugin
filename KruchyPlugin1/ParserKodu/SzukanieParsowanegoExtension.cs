using System;
using System.Linq;

namespace KruchyCompany.KruchyPlugin1.ParserKodu
{
    public static class SzukanieParsowanegoExtension
    {
        public static Metoda SzukajMetodyWLinii(this Plik parsowane, int numerLinii)
        {
            var metody = parsowane.DefiniowaneObiekty.First().Metody;

            return
                metody
                    .Where(o =>
                        o.Poczatek.Wiersz <= numerLinii
                            && o.Koniec.Wiersz >= numerLinii)
                            .FirstOrDefault();
        }

        public static Property SzukajPropertiesaWLinii(this Plik parsowane, int numerLinii)
        {
            var propertiesy = parsowane.DefiniowaneObiekty.First().Propertiesy;
            return
                propertiesy
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
    }
}