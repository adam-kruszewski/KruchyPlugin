using KrucheBuilderyKodu.Builders;
using KruchyCompany.KruchyPlugin1.Extensions;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieNowejMetodyWBuilderze
    {
        private readonly SolutionWrapper solution;

        public DodawanieNowejMetodyWBuilderze(
            SolutionWrapper solution)
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
