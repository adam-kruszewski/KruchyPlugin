using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace KruchyCompany.KruchyPlugin1.Utils
{
    public class SolutionWrapper
    {
        private readonly DTE2 dte;

        public SolutionWrapper(DTE2 dte)
        {
            this.dte = dte;
        }

        public string PelnaNazwa
        {
            get { return dte.Solution.FullName; }
        }

        public string Nazwa
        {
            get
            {
                var fi = new FileInfo(PelnaNazwa);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
        }

        public string Katalog
        {
            get
            {
                var fi = new FileInfo(PelnaNazwa);
                return fi.DirectoryName;
            }
        }

        public DTE2 DTE { get { return dte; } }

        public IList<ProjektWrapper> Projekty
        {
            get
            {
                var wynik = new List<ProjektWrapper>();
                var solution = dte.Solution;
                for (int i = 1; i <= solution.Count; i++)
                {
                    var p = solution.Projects.Item(i);
                    if (p.FullName.ToLower().EndsWith(".csproj"))
                        wynik.Add(new ProjektWrapper(p));
                    else
                        SzukajProjektow(p, wynik);
                }
                return wynik;
            }
        }

        private void SzukajProjektow(Project p, List<ProjektWrapper> wynik)
        {
            for ( int i = 1; i <= p.ProjectItems.Count; i++)
            {
                var item = p.ProjectItems.Item(i);
                if (item.Object is Project)
                    wynik.Add(new ProjektWrapper(item.Object as Project));
            }
        }

        public ProjektWrapper ZnajdzProjktDlaPliku(string nazwa)
        {
            throw new System.NotImplementedException();
        }

        public PlikWrapper AktualnyPlik
        {
            get { return new PlikWrapper(dte.ActiveDocument); }
        }

        public ProjektWrapper ZnajdzProjekt(string nazwa)
        {
            return Projekty.Where(o => o.Nazwa == nazwa).FirstOrDefault();
        }

        public PlikWrapper OtworzPlik(PlikWrapper plik)
        {
            return null;
        }

        public PlikWrapper OtworzPlik(string sciezka)
        {
            UIHierarchy UIH = dte.ToolWindows.SolutionExplorer;
            UIHierarchyItem UIHItem = UIH.UIHierarchyItems.Item(1);

            if (!UIHItem.UIHierarchyItems.Expanded)
                UIHItem.UIHierarchyItems.Expanded = true;
            Podrozuj(UIHItem.UIHierarchyItems);
            return null;
        }

        private void Podrozuj(UIHierarchyItems uIHierarchyItems)
        {
            if (!uIHierarchyItems.Expanded)
                uIHierarchyItems.Expanded = true;

            for (int i = 1; i <= uIHierarchyItems.Count; i++)
            {
                var item = uIHierarchyItems.Item(i);
                Podrozuj(item.UIHierarchyItems);
            }
        }


        UIHierarchyItem FindHierarchyItem(UIHierarchyItems items, object item)
        {
            //
            // Enumerating children recursive would work, but it may be slow on large solution.
            // This tries to be smarter and faster
            //

            Stack s = new Stack();
            CreateItemsStack(s, item);

            UIHierarchyItem last = null;
            while (s.Count != 0)
            {
                if (!items.Expanded)
                    items.Expanded = true;
                if (!items.Expanded)
                {
                    //bug: expand dont always work...
                    UIHierarchyItem parent = ((UIHierarchyItem)items.Parent);
                    parent.Select(vsUISelectionType.vsUISelectionTypeSelect);
                    dte.ToolWindows.SolutionExplorer.DoDefaultAction();
                }

                object o = s.Pop();

                last = null;
                foreach (UIHierarchyItem child in items)
                    if (child.Object == o)
                    {
                        last = child;
                        items = child.UIHierarchyItems;
                        break;
                    }
            }

            return last;
        }

        void CreateItemsStack(Stack s, object item)
        {
            if (item is ProjectItem)
            {
                ProjectItem pi = (ProjectItem)item;
                s.Push(pi);
                CreateItemsStack(s, pi.Collection.Parent);
            }
            else if (item is Project)
            {
                Project p = (Project)item;
                s.Push(p);
                if (p.ParentProjectItem != null)
                {
                    //top nodes dont have solution as parent, but is null 
                    CreateItemsStack(s, p.ParentProjectItem);
                }
            }
            else if (item is Solution)
            {
                //doesnt seem to ever happend... 
                Solution sol = (Solution)item;
            }
            else
            {
                throw new Exception("unknown item");
            }
        }
    }
}
