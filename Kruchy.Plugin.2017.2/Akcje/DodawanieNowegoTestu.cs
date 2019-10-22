using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Utils.Wrappers;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieNowegoTestu
    {
        private readonly ISolutionWrapper solution;

        public DodawanieNowegoTestu(ISolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void DodajNowyTest(string nazwaTestu)
        {
            var builder =
                new MetodaBuilder()
                    .ZNazwa(nazwaTestu)
                    .DodajModyfikator("public")
                    .DodajAtrybut(new AtrybutBuilder().ZNazwa("Test"));

            builder.DodajLinie("//arrange");
            builder.DodajLinie("");
            builder.DodajLinie("//act");
            builder.DodajLinie("");
            builder.DodajLinie("//assert");
            var dokument = solution.AktualnyDokument;
            if (dokument == null)
                return;

            var numerLinii = dokument.DajNumerLiniiKursora();
            var trescMetody = builder.Build(StaleDlaKodu.WciecieDlaMetody).TrimEnd();
            dokument.WstawWLinii(trescMetody, numerLinii);
            dokument.UstawKursosDlaMetodyDodanejWLinii(numerLinii + 2);
            dokument.DodajUsingaJesliTrzeba("NUnit.Framework");
        }
    }
}