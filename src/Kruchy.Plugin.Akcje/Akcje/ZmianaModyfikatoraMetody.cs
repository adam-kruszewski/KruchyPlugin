using System;
using System.Collections.Generic;
using System.Linq;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class ZmianaModyfikatoraMetody
    {
        private readonly IDocumentWrapper dokument;
        private readonly string[] modyfikatory = { "public", "private", "internal", "protected" };

        public ZmianaModyfikatoraMetody(IDocumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void ZmienNa(string modyfikator)
        {
            var parsowane = Parser.Parsuj(dokument.GetContent());
            var metoda =
                parsowane.SzukajMetodyWLinii(dokument.GetCursorLineNumber());

            if (metoda != null)
            {
                if (metoda.ZawieraModyfikator(modyfikator))
                    return;

                ZmienWMetodzie(modyfikator, metoda);
            }
            else
            {
                var klasa = parsowane.SzukajObiektuWLinii(dokument.GetCursorLineNumber());
                if (klasa != null)
                    ZmienWKlasie(modyfikator, klasa);
            }
        }

        private void ZmienWKlasie(string modyfikator, Obiekt klasa)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(klasa.Modyfikatory);

            if (klasa.Owner == null && modyfikator == "private")
                modyfikator = "";

            if (dotychczasowyModyfikator == null)
                WstawModyfikator(modyfikator, klasa.RodzajObiektuObiekt.Poczatek);
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienWMetodzie(string modyfikator, Metoda metoda)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(metoda.Modyfikatory);

            if (dotychczasowyModyfikator == null)
            {
                WstawModyfikator(
                    modyfikator,
                    metoda.TypZwracany.Poczatek);
            }
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienModyfikator(
            string modyfikator,
            Modyfikator dotychczasowyModyfikator)
        {
            dokument.Remove(
                dotychczasowyModyfikator.Poczatek.Row,
                dotychczasowyModyfikator.Poczatek.Column,
                dotychczasowyModyfikator.Koniec.Row,
                dotychczasowyModyfikator.Koniec.Column);
            dokument.InsertInPlace(
                modyfikator,
                dotychczasowyModyfikator.Poczatek.Row,
                dotychczasowyModyfikator.Poczatek.Column);
        }

        private void WstawModyfikator(string modyfikator, PozycjaWPliku polozenie)
        {
            dokument.InsertInPlace(
                modyfikator + " ",
                polozenie.Row,
                polozenie.Column);
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