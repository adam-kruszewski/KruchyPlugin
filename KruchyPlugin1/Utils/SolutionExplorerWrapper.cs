using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    class SolutionExplorerWrapper
    {
        private readonly SolutionWrapper solution;
        private UIHierarchy SolutionExplorer
        {
            get
            {
                return solution.DTE.ToolWindows.SolutionExplorer;
            }
        }

        private UIHierarchyItem WezelGlowny
        {
            get { return SolutionExplorer.UIHierarchyItems.Item(1); }
        }

        public SolutionExplorerWrapper(SolutionWrapper solution)
        {
            this.solution = solution;
        }

        private IList<UIHierarchyItem> WszystkieWezly()
        {
            var wynik = new List<UIHierarchyItem>();
            ZaladujElementyUI();

            OdwiedzWezly(WezelGlowny, wynik);
            return wynik;
        }

        private void OdwiedzWezly(
            UIHierarchyItem wezel,
            List<UIHierarchyItem> wynik)
        {
            wynik.Add(wezel);
            for (int i = 1; i <= wezel.UIHierarchyItems.Count; i++)
            {
                var dziecko = wezel.UIHierarchyItems.Item(i);
                wynik.Add(dziecko);
                OdwiedzWezly(dziecko, wynik);
            }
        }

        private void ZaladujElementyUI()
        {
            UIHierarchyItem UIHItem = SolutionExplorer.UIHierarchyItems.Item(1);

            if (!UIHItem.UIHierarchyItems.Expanded)
                UIHItem.UIHierarchyItems.Expanded = true;
            Podrozuj(UIHItem.UIHierarchyItems);
        }

        private void Podrozuj(UIHierarchyItems uIHierarchyItems)
        {
            if (!uIHierarchyItems.Expanded)
                uIHierarchyItems.Expanded = true;

            for (int i = 1; i <= uIHierarchyItems.Count; i++)
            {
                var item = uIHierarchyItems.Item(i);
                var projectItem = item.Object as ProjectItem;
                if (projectItem != null)
                {
                    var n = projectItem.FileNames[0];
                    Console.WriteLine(projectItem.Name);
                }
                Podrozuj(item.UIHierarchyItems);
            }
        }

        public void OtworzPlik(string sciezka)
        {
            ZaladujElementyUI();
            //var item = SolutionExplorer.GetItem();
            //item.Select(vsUISelectionType.vsUISelectionTypeSetCaret);
        }

        public void OtworzPlik(PlikWrapper plik)
        {
            ZaladujElementyUI();
            solution.DTE.ItemOperations.OpenFile(plik.SciezkaPelna);
            OtworzPlik(plik.SciezkaPelna);
        }

        public void UstawSieNaMiejscu(string sciezka)
        {
            var wezel =
                WszystkieWezly()
                    .Where(o => WSciezce(o, sciezka))
                        .FirstOrDefault();
            if (wezel != null)
                wezel.Select(vsUISelectionType.vsUISelectionTypeSetCaret);
            else
                throw new ApplicationException(
                    "Nie udało się ustawić dla " + sciezka);
        }

        private bool WSciezce(UIHierarchyItem wezel, string sciezka)
        {
            var projectItem = wezel.Object as ProjectItem;
            if (projectItem != null && projectItem.FileNames[0] == sciezka)
                return true;
            return false;
        }
    }
}
