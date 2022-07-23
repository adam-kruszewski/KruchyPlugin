using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.Linq;

namespace Kruchy.Plugin.Akcje.Akcje
{
    public class ZmianaNaAsync
    {
        private readonly IDocumentWrapper dokument;

        public ZmianaNaAsync(
            IDocumentWrapper dokument)
        {
            this.dokument = dokument;
        }

        public void ZamienNaAsyncMethod()
        {
            var parsowane = Parser.Parsuj(dokument.GetContent());
            var metoda =
                parsowane.SzukajMetodyWLinii(dokument.GetCursorLineNumber());

            if (metoda != null)
            {
                DodajAsyncModyfikator(metoda);

                DodajTaskDoTypuZwracanegoJesliTrzeba();

                dokument.DodajUsingaJesliTrzeba("System.Threading.Tasks");
            }
        }

        private void DodajTaskDoTypuZwracanegoJesliTrzeba()
        {
            var parsowane = Parser.Parsuj(dokument.GetContent());
            var metoda = parsowane.SzukajMetodyWLinii(dokument.GetCursorLineNumber());

            if (!metoda.TypZwracany.Nazwa.StartsWith("Task"))
            {
                var wiersz = metoda.TypZwracany.Poczatek.Wiersz;
                var slowoDoWstawienia = "Task";
                if (metoda.TypZwracany.Nazwa != "void")
                {
                    dokument.InsertInPlace(
                        ">",
                        metoda.TypZwracany.Koniec.Wiersz,
                        metoda.TypZwracany.Koniec.Kolumna);
                    slowoDoWstawienia += "<";
                }
                else
                {
                    var typZwracany = metoda.TypZwracany;
                    dokument.Remove(typZwracany.Poczatek.Wiersz, typZwracany.Poczatek.Kolumna,
                        typZwracany.Koniec.Wiersz, typZwracany.Koniec.Kolumna);
                }

                dokument.InsertInPlace(
                    slowoDoWstawienia,
                    metoda.TypZwracany.Poczatek.Wiersz,
                    metoda.TypZwracany.Poczatek.Kolumna);
            }
        }

        private void DodajAsyncModyfikator(Metoda metoda)
        {
            if (!metoda.ZawieraModyfikator("async"))
            {
                if (metoda.Modyfikatory.Any())
                {
                    var koniecModyfikatorow = metoda.Modyfikatory.Max(o => o.Koniec.Kolumna);
                    var ostatni = metoda.Modyfikatory.Last();
                    dokument.InsertInPlace("async ", ostatni.Koniec.Wiersz, ostatni.Koniec.Kolumna + 1);
                }
                else
                {
                    dokument.InsertInPlace("async ", metoda.Poczatek.Wiersz, metoda.Poczatek.Kolumna);
                }
            }
        }
    }
}
