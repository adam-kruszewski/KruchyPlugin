using KruchyCompany.KruchyPlugin1.CodeBuilders;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class DodawanieNowegoTestu
    {
        private readonly SolutionWrapper solution;

        public DodawanieNowegoTestu(SolutionWrapper solution)
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