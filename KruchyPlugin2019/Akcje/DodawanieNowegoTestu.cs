using KrucheBuilderyKodu.Builders;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Extensions;
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
            var konfiguracja = Konfiguracja.GetInstance(solution);

            var builder =
                new MetodaBuilder()
                    .ZNazwa(nazwaTestu)
                    .DodajModyfikator("public")
                    .DodajAtrybut(new AtrybutBuilder().ZNazwa(DajNazweAtrybutu(konfiguracja)));

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
            dokument.DodajUsingaJesliTrzeba(DajUsingaDoDodania(konfiguracja));
        }

        private static string DajNazweAtrybutu(Konfiguracja konfiguracja)
        {
            switch (konfiguracja.Testy().NazwaBiblioteki)
            {
                case "NUnit":
                    return "Test";
                default:
                    return "TestMethod";
            }
        }

        private static string DajUsingaDoDodania(Konfiguracja konfiguracja)
        {
            switch (konfiguracja.Testy().NazwaBiblioteki)
            {
                case "NUnit":
                    return "NUnit.Framework";
                default:
                    return "Microsoft.VisualStudio.TestTools.UnitTesting";
            }
        }
    }
}