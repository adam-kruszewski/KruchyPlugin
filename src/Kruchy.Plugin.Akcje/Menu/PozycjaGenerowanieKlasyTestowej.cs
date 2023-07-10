using System;
using System.Collections.Generic;
using Kruchy.Plugin.Akcje.Akcje;
using Kruchy.Plugin.Akcje.Interfejs;
using Kruchy.Plugin.Akcje.KonfiguracjaPlugina;
using Kruchy.Plugin.Utils.Extensions;
using Kruchy.Plugin.Utils.Menu;
using Kruchy.Plugin.Utils.UI;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Akcje.Menu
{
    public class PozycjaGenerowanieKlasyTestowej : IPozycjaMenu
    {
        private readonly ISolutionExplorerWrapper solutionExplorer;
        private readonly ISolutionWrapper solution;

        public PozycjaGenerowanieKlasyTestowej(
            ISolutionWrapper solution,
            ISolutionExplorerWrapper solutionExplorer)
        {
            this.solution = solution;
            this.solutionExplorer = solutionExplorer;
        }

        public uint MenuCommandID
        {
            get { return PkgCmdIDList.cmdidZrobKlaseTestowa; }
        }

        public IEnumerable<WymaganieDostepnosci> Wymagania
        {
            get
            {
                yield return WymaganieDostepnosci.PlikCs;
                yield return WymaganieDostepnosci.Projekt;
                yield return WymaganieDostepnosci.Modul;
            }
        }

        public void Execute(object sender, EventArgs args)
        {
            INewTestClassWindow dialogNew = UIObjects.FactoryInstance.Get<INewTestClassWindow>();

            dialogNew.Konfiguracja = Konfiguracja.GetInstance(solution);

            dialogNew.GetDirectoryFromModuleFunc = GetDireectoryFromModule;

            var nazwaObiektu = solution.NazwaObiektuAktualnegoPliku();

            dialogNew.TestedInterface= nazwaObiektu;
            if (!dialogNew.TestedInterface.StartsWith("I"))
                dialogNew.TestedInterface = "I" + dialogNew.TestedInterface;

            dialogNew.ClassName = nazwaObiektu + "Tests";
            if (nazwaObiektu.StartsWith("I") && char.IsUpper(nazwaObiektu[1]))
                dialogNew.ClassName = nazwaObiektu.Substring(1) + "Tests";

            UIObjects.ShowWindowModal(dialogNew);

            if (dialogNew.Cancelled)
                return;

            new GenerowanieKlasyTestowej(solution, solutionExplorer)
                .Generuj(
                    dialogNew.ClassName,
                    dialogNew.TestType,
                    dialogNew.TestedInterface,
                    dialogNew.Directory);
        }

        private string GetDireectoryFromModule()
        {
            var projectDirectoryPath = solution.CurrentProject.DirectoryPath;

            var fileDirectoryPath = solution.CurrentFile.Directory;

            var result = fileDirectoryPath.Replace(projectDirectoryPath, "");

            return result.TrimStart(new char[] { '/', '\\'});
        }
    }
}
