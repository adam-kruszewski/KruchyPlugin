using System.IO;
using System.Text;
using KruchyCompany.KruchyPlugin1.CodeBuilders;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyCompany.KruchyPlugin1.Extensions;

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

            var nazwaPlikuImplementacji = nazwaKlasy + ".cs";
            var nazwaPlikuInterfejsu = "I" + nazwaKlasy + ".cs";

            var sciezkaDoProjektu = solution.AktualnyProjekt.SciezkaDoKatalogu;
            var pelnaSciezkaDoPlikuImplementacji =
                Path.Combine(sciezkaDoProjektu, "Validation", "Impl", nazwaPlikuImplementacji);
            var pelnaSciezkaDoPlikuInterfejsu =
                Path.Combine(sciezkaDoProjektu, "Validation", nazwaPlikuInterfejsu);

            Path
                .Combine(sciezkaDoProjektu, "Validation")
                    .DodajJesliTrzebaKatalogImpl();

            File.WriteAllText(
                pelnaSciezkaDoPlikuImplementacji,
                zawartoscImplementacji,
                Encoding.UTF8);
            File.WriteAllText(
                pelnaSciezkaDoPlikuInterfejsu,
                zawartoscInterfejsu,
                Encoding.UTF8);
            solution.AktualnyProjekt.DodajPlik(pelnaSciezkaDoPlikuImplementacji);
            solution.AktualnyProjekt.DodajPlik(pelnaSciezkaDoPlikuInterfejsu);
            var solutionExplorer = new SolutionExplorerWrapper(solution);
            solutionExplorer.OtworzPlik(pelnaSciezkaDoPlikuInterfejsu);
            solutionExplorer.OtworzPlik(pelnaSciezkaDoPlikuImplementacji);
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