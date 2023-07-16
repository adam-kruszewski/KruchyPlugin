using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using KruchyCodeBuilders.Builders;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Akcje
{
    class GenerowanieKlasService
    {
        private readonly ISolutionWrapper solution;
        private readonly ISolutionExplorerWrapper solutionExplorer;

        public GenerowanieKlasService(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public void Generuj(
            IFileWrapper aktualnyPlik,
            string nazwaKlasyService,
            bool obaWKataloguImpl)
        {
            var aktualny = solution.CurrentFile;
            var projekt = aktualny.Project;

            if (aktualny == null)
                throw new ApplicationException("Nie ma otwartego pliku");

            var nazwaPlikuImplementacji = nazwaKlasyService + ".cs"; ;
            var nazwaPlikuInterfejsu = "I" + nazwaKlasyService + ".cs";

            var pelnaSciezkaDoImplementacji =
                Path.Combine(
                    projekt.SciezkaDoServiceImpl(), nazwaPlikuImplementacji);

            string pelnaSciezkaDoInterfejsu;
            if (!obaWKataloguImpl)
                pelnaSciezkaDoInterfejsu =
                    Path.Combine(
                        projekt.SciezkaDoService(), nazwaPlikuInterfejsu);
            else
                pelnaSciezkaDoInterfejsu =
                    Path.Combine(
                        projekt.SciezkaDoServiceImpl(), nazwaPlikuInterfejsu);

            if (File.Exists(pelnaSciezkaDoImplementacji))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoImplementacji);
                return;
            }
            if (File.Exists(pelnaSciezkaDoInterfejsu))
            {
                MessageBox.Show("Plik już istnieje " + pelnaSciezkaDoInterfejsu);
                return;
            }

            File.WriteAllText(
                pelnaSciezkaDoImplementacji,
                GenerujPlikImplementacji(nazwaKlasyService, projekt),
                Encoding.UTF8);
            File.WriteAllText(
                pelnaSciezkaDoInterfejsu,
                GenerujPlikInterfejsu(nazwaKlasyService, projekt, obaWKataloguImpl),
                Encoding.UTF8);

            var plikImpl = projekt.AddFile(pelnaSciezkaDoImplementacji);
            var plikInt = projekt.AddFile(pelnaSciezkaDoInterfejsu);

            solutionExplorer.OpenFile(plikInt.FullPath);
            solutionExplorer.OpenFile(plikImpl.FullPath);
        }

        private string GenerujPlikImplementacji(
            string nazwaKlasyService,
            IProjectWrapper projekt)
        {
            var klasaBuilder =
                new ClassBuilder()
                    .WithModifier("")
                    .WithName(nazwaKlasyService)
                    .AddInterface("I" + nazwaKlasyService);

            var plikBuilder =
                new FileWithCodeBuilder()
                    .WithObject(klasaBuilder)
                    .InNamespace(GenerujNamespaceImpl(projekt));

            var zawartosc = plikBuilder.Build();
            return zawartosc;
        }

        private string GenerujPlikInterfejsu(
            string nazwaKlasyService,
            IProjectWrapper projekt,
            bool obaWImpl)
        {
            var interfaceBuilder =
                new InterfaceBuilder()
                    .WithName("I" + nazwaKlasyService)
                    .WithModifier("public");

            var plikClassBuilder =
                new FileWithCodeBuilder()
                    .WithObject(interfaceBuilder);
            if (!obaWImpl)
                plikClassBuilder.InNamespace(GenerujNamespace(projekt));
            else
                plikClassBuilder.InNamespace(GenerujNamespaceImpl(projekt));

            var zawartosc = plikClassBuilder.Build();
            return zawartosc;
        }

        private string GenerujNamespace(IProjectWrapper projekt)
        {
            return projekt.Name + ".Services";
        }

        private string GenerujNamespaceImpl(IProjectWrapper projekt)
        {
            return GenerujNamespace(projekt) + ".Impl";
        }
    }
}
