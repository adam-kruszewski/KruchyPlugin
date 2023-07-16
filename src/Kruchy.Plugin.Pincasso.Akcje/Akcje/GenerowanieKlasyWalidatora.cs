using KruchyCodeBuilders.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Kruchy.Plugin.Pincasso.Akcje.Akcje
{
    class GenerowanieKlasyWalidatora
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieKlasyWalidatora(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(string nazwaKlasy)
        {
            var nazwaKlasyWalidowanej =
                solution.CurrentFile.NameWithoutExtension;

            var parsowane = Parser.Parse(solution.CurentDocument.GetContent());
            nazwaKlasyWalidowanej = parsowane.DefinedItems[0].Name;

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

            var sciezkaDoProjektu = solution.CurrentProject.DirectoryPath;
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
            solution.CurrentProject.AddFile(pelnaSciezkaDoPlikuImplementacji);
            solution.CurrentProject.AddFile(pelnaSciezkaDoPlikuInterfejsu);

            solutionExplorer.OpenFile(pelnaSciezkaDoPlikuInterfejsu);
            solutionExplorer.OpenFile(pelnaSciezkaDoPlikuImplementacji);
        }

        private string GenerujZawartoscImplementacji(
            string nazwaKlasy,
            string nazwaKlasyWalidowanej,
            string usingDlaDomainObiektu)
        {
            var klasa =
                new ClassBuilder()
                    .WithName(nazwaKlasy)
                    .WithSuperClass("PincassoValidator<" + nazwaKlasyWalidowanej + ">")
                    .AddInterface("I" + nazwaKlasy);

            var metodaCreateRules =
                new MethodBuilder()
                    .WithName("CreateRules")
                    .AddModifier("protected")
                    .AddModifier("override")
                    .WithReturnType("ValidationRulesSet")
                    .AddParameter(nazwaKlasyWalidowanej, "entity")
                    .AddParameter("IValidationResultListener", "validationListener")
                    .AddLine("return base.CreateRules(entity, validationListener);");
            klasa.AddMethod(metodaCreateRules);

            var plikClass =
                new FileWithCodeBuilder()
                    .InNamespace(DajNamespaceImplementacji())
                    .AddUsing("Piatka.Infrastructure.Validation")
                    .AddUsing(usingDlaDomainObiektu)
                    .AddUsing("Pincasso.Core.Base")
                    .WithObject(klasa);

            return plikClass.Build();
        }

        private string DajNamespaceImplementacji()
        {
            return solution.CurrentFile.Project.Name + ".Validation.Impl";
        }

        private string GenerujZawartoscInterfejsu(
            string nazwaKlasy,
            string nazwaKlasyWalidowanej,
            string usingDlaDomainObiektu)
        {
            var interfejs =
                new InterfaceBuilder()
                    .WithModifier("public")
                    .WithName("I" + nazwaKlasy)
                    .WithSuperClass("IValidator<" + nazwaKlasyWalidowanej + ">");

            var plikClass =
                new FileWithCodeBuilder()
                    .AddUsing("Piatka.Infrastructure.Validation")
                    .InNamespace(DajNamespaceInterfejsu())
                    .AddUsing(usingDlaDomainObiektu)
                    .WithObject(interfejs);

            return plikClass.Build();
        }

        private string DajNamespaceInterfejsu()
        {
            return solution.CurrentFile.Project.Name + ".Validation";
        }

    }
}
