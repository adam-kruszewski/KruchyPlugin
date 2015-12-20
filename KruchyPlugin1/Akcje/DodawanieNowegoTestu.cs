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
            var dokument = solution.AktualnyDokument;
            if (dokument == null)
                return;

            var numerLinii = dokument.DajNumerLiniiKursora();
            var trescMetody = builder.Build(StaleDlaKodu.WciecieDlaMetody).TrimEnd();
            dokument.WstawWLinii(trescMetody, numerLinii);
            dokument.DodajUsingaJesliTrzeba("NUnit.Framework");
        }
    }
}