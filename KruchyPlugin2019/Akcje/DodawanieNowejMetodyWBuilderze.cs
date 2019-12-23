using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieNowejMetodyWBuilderze
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
            var parsowane = Parser.Parsuj(dokument.DajZawartosc());

            var metodaBuilder =
                new MetodaBuilder()
                    .DodajModyfikator("public")
                    .ZNazwa(nazwaMetody)
                    .ZTypemZwracanym(
                        parsowane
                            .SzukajObiektuWLinii(dokument.DajNumerLiniiKursora()).Nazwa)
                    .DodajLinie("return this;");

            var numerLiniiWstawiania = dokument.DajNumerLiniiKursora();
            dokument.WstawWLinii(
                metodaBuilder.Build(StaleDlaKodu.WciecieDlaMetody),
                numerLiniiWstawiania);

            var zawartoscLinii = dokument.DajZawartoscLinii(numerLiniiWstawiania);
            dokument.UstawKursor(numerLiniiWstawiania, zawartoscLinii.Length);
        }
    }
}
