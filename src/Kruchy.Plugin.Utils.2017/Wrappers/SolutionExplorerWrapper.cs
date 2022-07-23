using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Kruchy.Plugin.Utils.Wrappers;

namespace Kruchy.Plugin.Utils._2017.Wrappers
{
#pragma warning disable VSTHRD010
    public class SolutionExplorerWrapper : ISolutionExplorerWrapper
    {
        private readonly ISolutionWrapper solution;
        private readonly DTE2 dte;

        private UIHierarchy SolutionExplorer
        {
            get
            {
                return dte.ToolWindows.SolutionExplorer;
            }
        }

        private UIHierarchyItem WezelGlowny
        {
            get { return SolutionExplorer.UIHierarchyItems.Item(1); }
        }

        private SolutionExplorerWrapper(ISolutionWrapper solution, DTE2 dte)
        {
            this.solution = solution;
            this.dte = dte;
        }

        public static ISolutionExplorerWrapper DajDlaSolution(
            ISolutionWrapper solution,
            DTE2 dte)
        {
            return new SolutionExplorerWrapper(solution, dte);
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

            var rozwijane = new List<UIHierarchyItems>();
            if (!UIHItem.UIHierarchyItems.Expanded)
            {
                UIHItem.UIHierarchyItems.Expanded = true;
                rozwijane.Add(UIHItem.UIHierarchyItems);
            }
            Podrozuj(UIHItem.UIHierarchyItems, rozwijane);
            foreach (var items in rozwijane)
                items.Expanded = false;
        }

        private void Podrozuj(
            UIHierarchyItems uIHierarchyItems, List<UIHierarchyItems> rozwijane)
        {
            if (!uIHierarchyItems.Expanded)
            {
                uIHierarchyItems.Expanded = true;
                rozwijane.Add(uIHierarchyItems);
            }

            for (int i = 1; i <= uIHierarchyItems.Count; i++)
            {
                var item = uIHierarchyItems.Item(i);
                var projectItem = item.Object as ProjectItem;
                Podrozuj(item.UIHierarchyItems, rozwijane);
            }
        }

        public void OpenFile(string sciezka)
        {
            //ZaladujElementyUI();
            dte.ItemOperations.OpenFile(sciezka);
        }

        public void OpenFile(IFileWrapper plik)
        {
            //ZaladujElementyUI();
            dte.ItemOperations.OpenFile(plik.FullPath);
        }

        public void SelectPath(string sciezka)
        {
            var wezlyProjektow = SzukajWezlowProjektow(3);
            if (Directory.Exists(sciezka))
            {
                var info = new FileInfo(sciezka);
                var pelna = info.FullName;
                var projekt =
                    wezlyProjektow
                        .Where(o => ProjektWKtorymJestSciezka(pelna, o))
                            .FirstOrDefault();
                if (projekt != null)
                {
                    var katalogProjektu = DajKatalogWezlaProjektu(projekt);
                    var reszta = pelna.Substring(katalogProjektu.Length);

                    var czesci =
                        reszta.Split(
                            Path.DirectorySeparatorChar)
                                .Where(o => o != "")
                                    .ToArray();
                    OdznaczZaznaczone();

                    UIHierarchyItem znalezionyWezel = ZnajdzWezelDlaReszty(projekt, czesci);
                    if (znalezionyWezel != null)
                    {
                        znalezionyWezel.Select(vsUISelectionType.vsUISelectionTypeSetCaret);
                        znalezionyWezel.Select(vsUISelectionType.vsUISelectionTypeToggle);
                        znalezionyWezel.Select(vsUISelectionType.vsUISelectionTypeSelect);
                        znalezionyWezel.UIHierarchyItems.Expanded = true;
                    }
                }
            }
        }

        private void OdznaczZaznaczone()
        {
            foreach (EnvDTE.UIHierarchyItem item in SolutionExplorer.SelectedItems as object[])
            {
                item.Select(vsUISelectionType.vsUISelectionTypeToggle);
            }
        }

        private bool ProjektWKtorymJestSciezka(string pelna, UIHierarchyItem o)
        {
            return
                pelna
                    .ToLower()
                        .StartsWith(DajKatalogWezlaProjektu(o).ToLower());
        }

        private UIHierarchyItem ZnajdzWezelDlaReszty(
            UIHierarchyItem projekt, string[] czesci)
        {
            if (czesci.Length == 0)
                return null;
            var nazwa = czesci.First().ToLower();
            if (!projekt.UIHierarchyItems.Expanded)
                projekt.UIHierarchyItems.Expanded = true;

            for (int i = 1; i <= projekt.UIHierarchyItems.Count; i++)
            {
                var item = projekt.UIHierarchyItems.Item(i);
                if (item.Name.ToLower() == nazwa)
                {
                    if (czesci.Length == 1)
                        return item;
                    else
                    {
                        var noweCzesci = new string[czesci.Length - 1];
                        for (int j = 0; j < czesci.Length - 1; j++)
                            noweCzesci[j] = czesci[j + 1];
                        return ZnajdzWezelDlaReszty(item, noweCzesci);
                    }
                }
            }
            return null;
        }

        private string DajKatalogWezlaProjektu(UIHierarchyItem wezelProjektu)
        {
            var projekt = wezelProjektu.Object as Project;

            if (projekt == null)
                throw new ApplicationException("Coś poszło nie tak z projektem");

            var info = new FileInfo(projekt.FullName);
            return info.DirectoryName;
        }

        private bool WSciezce(UIHierarchyItem wezel, string sciezka)
        {
            var projectItem = wezel.Object as ProjectItem;
            if (projectItem != null)
            {
                var nazwaPliku = projectItem.FileNames[0];
                if (nazwaPliku.EndsWith("\\"))
                    nazwaPliku = nazwaPliku.Substring(0, nazwaPliku.Length - 1);
                if (nazwaPliku == sciezka)
                    return true;
            }
            else
            {
                string aktualnaSciezka = BudujSciezke(wezel);
                if (aktualnaSciezka == sciezka)
                    return true;
            }
            return false;
        }

        private string BudujSciezke(UIHierarchyItem wezel)
        {
            if (wezel.Name == "KontaktyWatku")
            {
                var h = wezel.Collection.Parent as UIHierarchyItem;
            }

            var itemsNaSciezce = new List<UIHierarchyItem>();
            var aktualna = wezel;
            while (aktualna != null)
            {
                var p = aktualna.Object as Project;
                if (p != null)
                {
                    if (p.FullName.ToLower().EndsWith(".csproj"))
                    {
                        var fi = new FileInfo(p.FullName);
                        var elementySciezki = new List<string>();
                        elementySciezki.Add(fi.DirectoryName);
                        itemsNaSciezce.Reverse();
                        foreach (UIHierarchyItem item in itemsNaSciezce)
                            elementySciezki.Add(item.Name);
                        var wynik = string.Join("" + Path.PathSeparator, elementySciezki.ToArray());
                        return wynik;
                    }
                    else
                        return string.Empty;
                }
                itemsNaSciezce.Add(aktualna);
                aktualna = aktualna.Collection.Parent as UIHierarchyItem;
            }
            return string.Empty;
        }

        private List<UIHierarchyItem> SzukajWezlowProjektow(int maxGlebokosc)
        {
            var wynik = new List<UIHierarchyItem>();
            UIHierarchyItem solutionItem = SolutionExplorer.UIHierarchyItems.Item(1);

            wynik.AddRange(SzukajWezlowProjektow(solutionItem, maxGlebokosc));
            return wynik;
        }

        private IEnumerable<UIHierarchyItem> SzukajWezlowProjektow(
            UIHierarchyItem item, int maxGlebokosc)
        {
            var inicjalneRozwiniecie = item.UIHierarchyItems.Expanded;
            if (!item.UIHierarchyItems.Expanded)
            {
                item.UIHierarchyItems.Expanded = true;
                item.UIHierarchyItems.Expanded = inicjalneRozwiniecie;
            }

            for (int i = 1; i <= item.UIHierarchyItems.Count; i++)
            {
                UIHierarchyItem aktItem = item.UIHierarchyItems.Item(i);
                if (JestProjektem(aktItem))
                    yield return aktItem;
                else
                {
                    if (maxGlebokosc > 0)
                        foreach (var p in SzukajWezlowProjektow(aktItem, maxGlebokosc - 1))
                            yield return p;
                }
            }
        }

        private bool JestProjektem(UIHierarchyItem item)
        {
            var project = item.Object as Project;
            if (project != null && project.FullName.EndsWith(".csproj"))
                return true;
            return false;
        }

        public ISelectionWrapper GetSelection()
        {
            return new SelectionWrapper(this.dte);
        }
    }
}