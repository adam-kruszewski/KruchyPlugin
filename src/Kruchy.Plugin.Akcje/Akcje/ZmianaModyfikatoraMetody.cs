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
                parsowane.FindMethodByLineNumber(dokument.GetCursorLineNumber());

            if (metoda != null)
            {
                if (metoda.ZawieraModyfikator(modyfikator))
                    return;

                ZmienWMetodzie(modyfikator, metoda);
            }
            else
            {
                var klasa = parsowane.FindDefinedItemByLineNumber(dokument.GetCursorLineNumber());
                if (klasa != null)
                    ZmienWKlasie(modyfikator, klasa);
            }
        }

        private void ZmienWKlasie(string modyfikator, DefinedItem klasa)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(klasa.Modifiers);

            if (klasa.Owner == null && modyfikator == "private")
                modyfikator = "";

            if (dotychczasowyModyfikator == null)
                WstawModyfikator(modyfikator, klasa.KindOfObjectUnit.StartPosition);
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienWMetodzie(string modyfikator, Method metoda)
        {
            var dotychczasowyModyfikator =
                SzukajDotychczasowegoModyfikatora(metoda.Modyfikatory);

            if (dotychczasowyModyfikator == null)
            {
                WstawModyfikator(
                    modyfikator,
                    metoda.ReturnType.StartPosition);
            }
            else
                ZmienModyfikator(modyfikator, dotychczasowyModyfikator);
        }

        private void ZmienModyfikator(
            string modyfikator,
            Modifier dotychczasowyModyfikator)
        {
            dokument.Remove(
                dotychczasowyModyfikator.StartPosition.Row,
                dotychczasowyModyfikator.StartPosition.Column,
                dotychczasowyModyfikator.EndPosition.Row,
                dotychczasowyModyfikator.EndPosition.Column);
            dokument.InsertInPlace(
                modyfikator,
                dotychczasowyModyfikator.StartPosition.Row,
                dotychczasowyModyfikator.StartPosition.Column);
        }

        private void WstawModyfikator(string modyfikator, PlaceInFile polozenie)
        {
            dokument.InsertInPlace(
                modyfikator + " ",
                polozenie.Row,
                polozenie.Column);
        }

        private Modifier SzukajDotychczasowegoModyfikatora(
            IEnumerable<Modifier> aktualneModyfikatory)
        {
            var dotychczasowyModyfikator =
                aktualneModyfikatory
                    .Where(o => modyfikatory.Any(m => m == o.Name))
                        .FirstOrDefault();

            return dotychczasowyModyfikator;
        }
    }
}