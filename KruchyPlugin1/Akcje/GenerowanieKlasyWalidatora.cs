using KruchyCompany.KruchyPlugin1.CodeBuilders;
using KruchyCompany.KruchyPlugin1.Utils;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieKlasyWalidatora
    {
        private readonly SolutionWrapper solution;

        public GenerowanieKlasyWalidatora(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj(string nazwaKlasy, string nazwaKlasyWalidowanej)
        {
            var zawartoscImplementacji =
                GenerujZawartoscImplementacji(nazwaKlasy, nazwaKlasyWalidowanej);

            var zawartoscInterfejsu =
                GenerujZawartoscInterfejsu(nazwaKlasy, nazwaKlasyWalidowanej);
        }

        private string GenerujZawartoscImplementacji(string nazwaKlasy, string nazwaKlasyWalidowanej)
        {
            var klasa =
                new ClassBuilder()
                    .ZNazwa(nazwaKlasy)
                    .DodajInterfejs("I" + nazwaKlasy);

            var metodaCreateRules =
                new MetodaBuilder()
                    .ZNazwa("CreateRules")
                    .DodajModyfikator("protected")
                    .DodajModyfikator("override")
                    .ZTypemZwracanym("ValidationRulesSet")
                    .DodajParametr(nazwaKlasyWalidowanej, "entity")
                    .DodajParametr("IValidationResultListener", "validationListener")
                    .DodajLinie("return base.CreateRules(entity, validationListener);");
            klasa.DodajMetode(metodaCreateRules);

            var plikClass =
                new PlikClassBuilder()
                    .WNamespace(DajNamespaceImplementacji())
                    .DodajUsing("Piatka.Infrastructure.Validation")
                    .ZObiektem(klasa);

            return plikClass.Build();
        }

        private string DajNamespaceImplementacji()
        {
            return solution.AktualnyPlik.Projekt.Nazwa + "Validatation.Impl";
        }

        private string GenerujZawartoscInterfejsu(string nazwaKlasy, string nazwaKlasyWalidowanej)
        {
            var interfejs =
                new InterfejsBuilder()
                    .ZModyfikatorem("public")
                    .ZNazwa("I" + nazwaKlasy)
                    .ZNadklasa("IValidator<" + nazwaKlasyWalidowanej + ">");

            var plikClass =
                new PlikClassBuilder()
                    .DodajUsing("Piatka.Infrastructure.Validation")
                    .WNamespace(DajNamespaceInterfejsu())
                    .ZObiektem(interfejs);

            return plikClass.Build();
        }

        private string DajNamespaceInterfejsu()
        {
            return solution.AktualnyPlik.Projekt.Nazwa + "Validatation";
        }

    }
}