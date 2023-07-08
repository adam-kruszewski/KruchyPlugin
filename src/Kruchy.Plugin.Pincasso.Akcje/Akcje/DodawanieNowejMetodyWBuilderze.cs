using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using KruchyParserKodu.ParserKodu.Models;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    public class DodawanieNowejMetodyWBuilderze
    {
        private readonly ISolutionWrapper solution;

        public DodawanieNowejMetodyWBuilderze(
            ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Dodaj(string nazwaMetody)
        {
            if (solution.AktualnyPlik == null || !solution.AktualnyPlik.JestWBuilderze())
                return;

            var dokument = solution.AktualnyDokument;
            var parsowane = Parser.Parsuj(dokument.GetContent());

            var metodaBuilder =
                new MetodaBuilder()
                    .DodajModyfikator("public")
                    .ZNazwa(nazwaMetody)
                    .ZTypemZwracanym(
                        parsowane
                            .FindDefinedItemByLineNumber(dokument.GetCursorLineNumber()).Name)
                    .DodajLinie("return this;");

            var numerLiniiWstawiania = dokument.GetCursorLineNumber();
            dokument.InsertInLine(
                metodaBuilder.Build(StaleDlaKodu.WciecieDlaMetody),
                numerLiniiWstawiania);

            var zawartoscLinii = dokument.GetLineContent(numerLiniiWstawiania);
            dokument.SetCursor(numerLiniiWstawiania, zawartoscLinii.Length);
        }
    }
}
