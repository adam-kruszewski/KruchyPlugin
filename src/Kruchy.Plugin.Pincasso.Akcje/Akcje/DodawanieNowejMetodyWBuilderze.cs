using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyCodeBuilders.Builders;
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
            if (solution.CurrentFile == null || !solution.CurrentFile.JestWBuilderze())
                return;

            var dokument = solution.CurentDocument;
            var parsowane = Parser.Parse(dokument.GetContent());

            var metodaBuilder =
                new MethodBuilder()
                    .AddModifier("public")
                    .WithName(nazwaMetody)
                    .WithReturnType(
                        parsowane
                            .FindDefinedItemByLineNumber(dokument.GetCursorLineNumber()).Name)
                    .AddLine("return this;");

            var numerLiniiWstawiania = dokument.GetCursorLineNumber();
            dokument.InsertInLine(
                metodaBuilder.Build(ConstsForCode.DefaultIndentForMethod),
                numerLiniiWstawiania);

            var zawartoscLinii = dokument.GetLineContent(numerLiniiWstawiania);
            dokument.SetCursor(numerLiniiWstawiania, zawartoscLinii.Length);
        }
    }
}
