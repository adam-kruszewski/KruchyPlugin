using System.IO;
using System.Text;
using KruchyCompany.KruchyPlugin1.Utils;
using KruchyCompany.KruchyPlugin1.Extensions;
using System.Windows;
using KrucheBuilderyKodu.Builders;
using KruchyParserKodu.ParserKodu;

namespace KruchyCompany.KruchyPlugin1.Akcje
{
    class GenerowanieKlasyWalidatora
    {
        private readonly SolutionWrapper solution;

        public GenerowanieKlasyWalidatora(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        public void Generuj(string nazwaKlasy)
        {
            var nazwaKlasyWalidowanej =
                solution.AktualnyPlik.NazwaBezRozszerzenia;

            var parsowane = Parser.Parsuj(solution.AktualnyDokument.DajZawartosc());
            nazwaKlasyWalidowanej = parsowane.DefiniowaneObiekty[0].Nazwa;

            var zawartoscImplementacji =
                GenerujZawartoscImplementacji(
                    nazwaKlasy,
                    nazwaKlasyWalidowanej,
                    parsowane.Namespace);

            var zawartoscInterfejsu =
                GenerujZawartoscInterfejsu(
                    nazwaKlasy,
                    nazwaKlasyWalidowanej,
                    parsowane.Namespace);

            var nazwaPlikuImplementacji = nazwaKlasy + ".cs";
            var nazwaPlikuInterfejsu = "I" + nazwaKlasy + ".cs";

            var sciezkaDoProjektu = solution.AktualnyProjekt.SciezkaDoKatalogu;
            var pelnaSciezkaDoPlikuImplementacji =
                Path.Combine(sciezkaDoProjektu, "Validation", "Impl", nazwaPlikuImplementacji);
            var pelnaSciezkaDoPlikuInterfejsu =
                Path.Combine(sciezkaDoProjektu, "Validation", nazwaPlikuInterfejsu);

            if (File.Exists(pelnaSciezkaDoPlikuImplementacji))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoPlikuImplementacji);
                return;
            }
            if (File.Exists(pelnaSciezkaDoPlikuInterfejsu))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoPlikuInterfejsu);
                return;
            }

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

        private string GenerujZawartoscImplementacji(
            string nazwaKlasy,
            string nazwaKlasyWalidowanej,
            string usingDlaDomainObiektu)
        {
            var klasa =
                new ClassBuilder()
                    .ZNazwa(nazwaKlasy)
                    .ZNadklasa("PincassoValidator<" + nazwaKlasyWalidowanej + ">")
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
                    .DodajUsing(usingDlaDomainObiektu)
                    .DodajUsing("Pincasso.Core.Base")
                    .ZObiektem(klasa);

            return plikClass.Build();
        }

        private string DajNamespaceImplementacji()
        {
            return solution.AktualnyPlik.Projekt.Nazwa + ".Validatation.Impl";
        }

        private string GenerujZawartoscInterfejsu(
            string nazwaKlasy,
            string nazwaKlasyWalidowanej,
            string usingDlaDomainObiektu)
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
                    .DodajUsing(usingDlaDomainObiektu)
                    .ZObiektem(interfejs);

            return plikClass.Build();
        }

        private string DajNamespaceInterfejsu()
        {
            return solution.AktualnyPlik.Projekt.Nazwa + ".Validatation";
        }

    }
}