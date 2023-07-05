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
                var wiersz = metoda.TypZwracany.Poczatek.Row;
                var slowoDoWstawienia = "Task";
                if (metoda.TypZwracany.Nazwa != "void")
                {
                    dokument.InsertInPlace(
                        ">",
                        metoda.TypZwracany.Koniec.Row,
                        metoda.TypZwracany.Koniec.Column);
                    slowoDoWstawienia += "<";
                }
                else
                {
                    var typZwracany = metoda.TypZwracany;
                    dokument.Remove(typZwracany.Poczatek.Row, typZwracany.Poczatek.Column,
                        typZwracany.Koniec.Row, typZwracany.Koniec.Column);
                }

                dokument.InsertInPlace(
                    slowoDoWstawienia,
                    metoda.TypZwracany.Poczatek.Row,
                    metoda.TypZwracany.Poczatek.Column);
            }
        }

        private void DodajAsyncModyfikator(Metoda metoda)
        {
            if (!metoda.ZawieraModyfikator("async"))
            {
                if (metoda.Modyfikatory.Any())
                {
                    var koniecModyfikatorow = metoda.Modyfikatory.Max(o => o.Koniec.Column);
                    var ostatni = metoda.Modyfikatory.Last();
                    dokument.InsertInPlace("async ", ostatni.Koniec.Row, ostatni.Koniec.Column + 1);
                }
                else
                {
                    dokument.InsertInPlace("async ", metoda.Poczatek.Row, metoda.Poczatek.Column);
                }
            }
        }
    }
}
