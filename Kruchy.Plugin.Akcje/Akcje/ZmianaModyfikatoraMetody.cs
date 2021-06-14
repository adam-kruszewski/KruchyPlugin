using System;
using System.Collections.Generic;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class ZmianaModyfikatoraMetody
    {
        private readonly IDokumentWrapper dokument;
        private readonly string[] modyfikatory = { "public", "private", "internal", "protected" };

        public ZmianaModyfikatoraMetody(IDokumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void ZmienNa(string modyfikator)
        {
            var parsowane = Parser.Parsuj(dokument.DajZawartosc());
            var metoda =
                parsowane.SzukajMetodyWLinii(dokument.DajNumerLiniiKursora());

            if (metoda != null)
            {
                if (metoda.ZawieraModyfikator(modyfikator))
                    return;

                ZmienWMetodzie(modyfikator, metoda);
            }else
            {
                var klasa = parsowane.SzukajObiektuWLinii(dokument.DajNumerLiniiKursora());
                if (klasa != null)
                    ZmienWKlasie(modyfikator, klasa);
            }
        }

        private void ZmienWKlasie(string modyfikator, Obiekt klasa)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(klasa.Modyfikatory);

            if (klasa.Wlasciciel == null && modyfikator == "private")
                modyfikator = "";

            if (dotychczasowyModyfikator == null)
                WstawModyfikator(modyfikator, klasa.Poczatek);
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienWMetodzie(string modyfikator, Metoda metoda)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(metoda.Modyfikatory);

            if (dotychczasowyModyfikator == null)
                WstawModyfikator(modyfikator, metoda.Poczatek);
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienModyfikator(
            string modyfikator,
            Modyfikator dotychczasowyModyfikator)
        {
            dokument.Usun(
                dotychczasowyModyfikator.Poczatek.Wiersz,
                dotychczasowyModyfikator.Poczatek.Kolumna,
                dotychczasowyModyfikator.Koniec.Wiersz,
                dotychczasowyModyfikator.Koniec.Kolumna);
            dokument.WstawWMiejscu(
                modyfikator,
                dotychczasowyModyfikator.Poczatek.Wiersz,
                dotychczasowyModyfikator.Poczatek.Kolumna);
        }

        private void WstawModyfikator(string modyfikator, PozycjaWPliku polozenie)
        {
            dokument.WstawWMiejscu(
                modyfikator + " ",
                polozenie.Wiersz,
                polozenie.Kolumna);
        }

        private Modyfikator SzukajDotychczasowegoModyfikatora(
            IEnumerable<Modyfikator> aktualneModyfikatory)
        {
            var dotychczasowyModyfikator =
                aktualneModyfikatory
                    .Where(o => modyfikatory.Any(m => m == o.Nazwa))
                        .FirstOrDefault();

            return dotychczasowyModyfikator;
        }
    }
}