using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;
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
                parsowane.FindMethodByLineNumber(dokument.GetCursorLineNumber());

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
            var metoda = parsowane.FindMethodByLineNumber(dokument.GetCursorLineNumber());

            if (!metoda.ReturnType.Name.StartsWith("Task"))
            {
                var wiersz = metoda.ReturnType.StartPosition.Row;
                var slowoDoWstawienia = "Task";
                if (metoda.ReturnType.Name != "void")
                {
                    dokument.InsertInPlace(
                        ">",
                        metoda.ReturnType.EndPosition.Row,
                        metoda.ReturnType.EndPosition.Column);
                    slowoDoWstawienia += "<";
                }
                else
                {
                    var typZwracany = metoda.ReturnType;
                    dokument.Remove(typZwracany.StartPosition.Row, typZwracany.StartPosition.Column,
                        typZwracany.EndPosition.Row, typZwracany.EndPosition.Column);
                }

                dokument.InsertInPlace(
                    slowoDoWstawienia,
                    metoda.ReturnType.StartPosition.Row,
                    metoda.ReturnType.StartPosition.Column);
            }
        }

        private void DodajAsyncModyfikator(Method metoda)
        {
            if (!metoda.ZawieraModyfikator("async"))
            {
                if (metoda.Modyfikatory.Any())
                {
                    var koniecModyfikatorow = metoda.Modyfikatory.Max(o => o.EndPosition.Column);
                    var ostatni = metoda.Modyfikatory.Last();
                    dokument.InsertInPlace("async ", ostatni.EndPosition.Row, ostatni.EndPosition.Column + 1);
                }
                else
                {
                    dokument.InsertInPlace("async ", metoda.StartPosition.Row, metoda.StartPosition.Column);
                }
            }
        }
    }
}
